using UnityEngine;

public class ScreenScaler : MonoBehaviour
{
    [Header("References")]
    public Transform spawner;
    public Transform bottomLine;

    [Header("Settings")]
    public float bottomOffset = 1f;

    private Camera mainCam;
    private float screenWidth;
    private float screenHeight;

    void Awake()
    {
        mainCam = Camera.main;
        AdjustScreenAndLayout();
    }

    public void AdjustScreenAndLayout()
    {
        if (mainCam == null || spawner == null || bottomLine == null) return;

        screenHeight = mainCam.orthographicSize * 2f;
        screenWidth = screenHeight * mainCam.aspect;

        float bottomY = (-screenHeight / 2f) + bottomOffset;
        bottomLine.position = new Vector3(0, bottomY, 0);

        Transform visual = bottomLine.transform.Find("Visual");
        if (visual != null)
        {
            visual.localScale = new Vector2(screenWidth + 2f, 0.1f);
        }

        BoxCollider2D bottomCol = bottomLine.GetComponent<BoxCollider2D>();
        if (bottomCol != null)
        {
            bottomCol.size = new Vector2(screenWidth + 2f, 0.5f);
        }

        float laneWidth = screenWidth / 4f;
        Transform[] lanes = new Transform[spawner.childCount];
        for (int i = 0; i < spawner.childCount; i++)
        {
            lanes[i] = spawner.GetChild(i);
        }

        float startX = (-screenWidth / 2f) + (laneWidth / 2f);
        for (int i = 0; i < lanes.Length; i++)
        {
            lanes[i].position = new Vector3(startX + (i * laneWidth), (screenHeight / 2f) + 2f, 0);
        }
    }
}