using UnityEngine;
using DG.Tweening;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance { get; private set; }

    public SpriteRenderer bgSpriteRenderer;
    public Sprite menuBackground;
    public Sprite[] gameplayBackgrounds;
    public float changeInterval = 25f;
    public float fadeDuration = 0.4f;

    private int currentBgIndex = 0;
    private float timer = 0f;
    private bool isTrackingTime = false;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Update()
    {
        if (!isTrackingTime || gameplayBackgrounds == null || gameplayBackgrounds.Length <= 1) return;

        timer += Time.deltaTime;
        if (timer >= changeInterval)
        {
            timer = 0f;
            SwitchToNextGameplayBackground();
        }
    }

    public void SetMenuBackground()
    {
        isTrackingTime = false;
        timer = 0f;
        if (menuBackground != null) ChangeBackgroundSmoothly(menuBackground);
    }

    public void StartGameplayBackgrounds()
    {
        currentBgIndex = 0;
        timer = 0f;
        isTrackingTime = true;

        if (gameplayBackgrounds != null && gameplayBackgrounds.Length > 0)
            ChangeBackgroundSmoothly(gameplayBackgrounds[0]);
    }

    private void SwitchToNextGameplayBackground()
    {
        if (gameplayBackgrounds == null || gameplayBackgrounds.Length == 0) return;

        currentBgIndex = (currentBgIndex + 1) % gameplayBackgrounds.Length;
        ChangeBackgroundSmoothly(gameplayBackgrounds[currentBgIndex]);
    }

    private void ChangeBackgroundSmoothly(Sprite newSprite)
    {
        if (bgSpriteRenderer == null || newSprite == null) return;

        Sequence fadeSeq = DOTween.Sequence();
        fadeSeq.Append(bgSpriteRenderer.DOFade(0f, fadeDuration));
        fadeSeq.AppendCallback(() =>
        {
            bgSpriteRenderer.sprite = newSprite;
            ScaleBackgroundToFitScreen();
        });
        fadeSeq.Append(bgSpriteRenderer.DOFade(1f, fadeDuration));
    }

    private void ScaleBackgroundToFitScreen()
    {
        if (bgSpriteRenderer.sprite == null || Camera.main == null) return;

        float cameraHeight = Camera.main.orthographicSize * 2f;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        Vector2 spriteSize = bgSpriteRenderer.sprite.bounds.size;
        float scaleX = cameraWidth / spriteSize.x;
        float scaleY = cameraHeight / spriteSize.y;

        float finalScale = Mathf.Max(scaleX, scaleY);
        bgSpriteRenderer.transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}