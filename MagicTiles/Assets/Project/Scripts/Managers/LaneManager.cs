using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public static LaneManager Instance { get; private set; }

    [Header("Visual Settings")]
    public GameObject linePrefab;
    public int numberOfLanes = 4;

    public float[] LanePositions { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        SetupLanes();
    }

    private void SetupLanes()
    {
        if (Camera.main == null) return;

        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Camera.main.aspect;
        float laneWidth = screenWidth / numberOfLanes;
        float startX = -(screenWidth / 2f);

        LanePositions = new float[numberOfLanes];

        for (int i = 0; i < numberOfLanes; i++)
        {
            LanePositions[i] = startX + (laneWidth * i) + (laneWidth / 2f);

            if (i > 0 && linePrefab != null)
            {
                float lineX = startX + (laneWidth * i);
                GameObject line = Instantiate(linePrefab, new Vector3(lineX, 0f, 0f), Quaternion.identity, this.transform);
                line.transform.localScale = new Vector3(line.transform.localScale.x, screenHeight + 2f, 1f);
            }
        }

        SetLanesActive(false);
    }

    public void SetLanesActive(bool activeState)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(activeState);
        }
    }
}