using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;
    public GameObject nextLevelPanel;

    [Header("Menu Animation Elements")]
    public CanvasGroup gameTitleGroup;
    public CanvasGroup playButtonGroup;
    public CanvasGroup exitButtonGroup;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI finalScoreText;

    [Header("Audio Settings")]
    public AudioSource sfxSource;
    public AudioClip perfectAudio;
    public AudioClip greatAudio;
    public AudioClip missAudio;
    public AudioClip gameOverAudio;
    public AudioClip winAudio;

    public AudioSource bgmSource;

    [Header("Gameplay Settings")]
    public float perfectZone = 0.5f;
    public float greatZone = 1.0f;
    public int maxMisses = 3;

    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int CurrentMisses { get; private set; }
    public bool IsPlaying { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }

    private GameObject bottomLine;
    public float TargetLineY => bottomLine != null ? bottomLine.transform.position.y : -4f;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        bottomLine = GameObject.FindGameObjectWithTag("BottomLine");
        ShowMainMenu();
    }

    void Update()
    {
        if (IsPlaying && !IsGameOver && !IsPaused)
        {
            if (bgmSource != null && !bgmSource.isPlaying)
                TriggerNextLevel();
        }
    }

    private void ClearActiveTiles()
    {
        Tile[] activeTiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        foreach (Tile tile in activeTiles)
        {
            if (SimplePooler.Instance != null)
                SimplePooler.Instance.ReturnToPool(tile.poolTag, tile.gameObject);
            else
                Destroy(tile.gameObject);
        }
    }

    public void ShowMainMenu()
    {
        bgmSource?.Stop();
        ClearActiveTiles();

        IsPlaying = false;
        IsGameOver = false;
        IsPaused = false;

        mainMenuPanel?.SetActive(true);
        gameOverPanel?.SetActive(false);
        nextLevelPanel?.SetActive(false);

        if (scoreText != null) { scoreText.text = "0"; scoreText.gameObject.SetActive(false); }
        comboText?.gameObject.SetActive(false);
        bottomLine?.SetActive(false);

        LaneManager.Instance?.SetLanesActive(false);
        BackgroundManager.Instance?.SetMenuBackground();

        AnimateMenuEntry();
    }

    public void StartGame()
    {
        ClearActiveTiles();

        Score = 0;
        Combo = 0;
        CurrentMisses = 0;
        IsPlaying = true;
        IsGameOver = false;
        IsPaused = false;

        UpdateScoreUI();

        mainMenuPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        nextLevelPanel?.SetActive(false);

        scoreText?.gameObject.SetActive(true);
        bottomLine?.SetActive(true);

        LaneManager.Instance?.SetLanesActive(true);
        BackgroundManager.Instance?.StartGameplayBackgrounds();

        if (bgmSource != null)
        {
            bgmSource.time = 0f;
            bgmSource.Play();
        }
    }

    private void TriggerNextLevel()
    {
        IsPlaying = false;
        sfxSource?.PlayOneShot(winAudio);

        ClearActiveTiles();

        scoreText?.gameObject.SetActive(false);
        comboText?.gameObject.SetActive(false);
        bottomLine?.SetActive(false);
        LaneManager.Instance?.SetLanesActive(false);

        nextLevelPanel?.SetActive(true);
    }

    private void AnimateMenuEntry()
    {
        void SetupGroup(CanvasGroup group)
        {
            if (group != null)
            {
                group.alpha = 0;
                group.transform.localPosition -= new Vector3(0, 30f, 0);
            }
        }

        SetupGroup(gameTitleGroup);
        SetupGroup(playButtonGroup);
        SetupGroup(exitButtonGroup);

        Sequence menuFade = DOTween.Sequence();

        if (gameTitleGroup != null)
            menuFade.Append(gameTitleGroup.DOFade(1, 0.6f))
                    .Join(gameTitleGroup.transform.DOLocalMoveY(gameTitleGroup.transform.localPosition.y + 30f, 0.6f).SetEase(Ease.OutCubic));

        if (playButtonGroup != null)
            menuFade.Append(playButtonGroup.DOFade(1, 0.4f))
                    .Join(playButtonGroup.transform.DOLocalMoveY(playButtonGroup.transform.localPosition.y + 30f, 0.4f).SetEase(Ease.OutCubic));

        if (exitButtonGroup != null)
            menuFade.Append(exitButtonGroup.DOFade(1, 0.4f))
                    .Join(exitButtonGroup.transform.DOLocalMoveY(exitButtonGroup.transform.localPosition.y + 30f, 0.4f).SetEase(Ease.OutCubic));
    }

    public void QuitGame() => Application.Quit();

    public void ProcessTileHit(Transform tile)
    {
        if (!IsPlaying || IsGameOver || IsPaused) return;

        float distance = Mathf.Abs(tile.position.y - TargetLineY);
        string rating;
        int basePoints;

        if (distance <= perfectZone)
        {
            rating = "Perfect";
            basePoints = 100;
            sfxSource?.PlayOneShot(perfectAudio);
        }
        else if (distance <= greatZone)
        {
            rating = "Great";
            basePoints = 80;
            sfxSource?.PlayOneShot(greatAudio);
        }
        else
        {
            rating = "Cool";
            basePoints = 50;
        }

        Combo++;
        Score += basePoints * Combo;
        UpdateScoreUI();
        UpdateComboUI();
        GameEvents.TriggerTileHit(basePoints, rating, tile.position);
    }

    public void ProcessTileMiss(Vector3 missPos)
    {
        if (!IsPlaying || IsGameOver) return;

        Combo = 0;
        CurrentMisses++;
        comboText?.gameObject.SetActive(false);
        sfxSource?.PlayOneShot(missAudio);
        GameEvents.TriggerTileMiss(missPos);

        if (CurrentMisses >= maxMisses)
            TriggerGameOver();
        else
        {
            Camera.main?.transform.DOShakePosition(0.2f, 0.3f, 10, 90);
            bgmSource?.Pause();
            SetPauseState(true);
        }
    }

    private void TriggerGameOver()
    {
        bgmSource?.Stop();
        sfxSource?.PlayOneShot(gameOverAudio);

        IsGameOver = true;
        IsPlaying = false;

        Camera.main?.transform.DOShakePosition(0.5f, 0.8f, 15, 90);
        scoreText?.gameObject.SetActive(false);
        gameOverPanel?.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = $"SCORE: {Score:N0}";
    }

    private void UpdateScoreUI() => scoreText.text = Score.ToString("N0");

    private void UpdateComboUI()
    {
        if (comboText != null && Combo > 1)
        {
            comboText.gameObject.SetActive(true);
            comboText.text = $"x{Combo} COMBO";
            comboText.transform.DOPunchScale(Vector3.one * 0.2f, 0.15f, 5, 1f);
        }
    }

    public void SetPauseState(bool state) => IsPaused = state;

    public void ResumeGame()
    {
        IsPaused = false;
        bgmSource?.UnPause();
    }
}