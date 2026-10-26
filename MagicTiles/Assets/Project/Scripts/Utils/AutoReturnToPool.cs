using UnityEngine;

public class AutoReturnToPool : MonoBehaviour
{
    public string poolTag;
    public float delay = 0.4f;

    void OnEnable() => Invoke(nameof(ReturnObj), delay);

    void OnDisable() => CancelInvoke();

    private void ReturnObj() => SimplePooler.Instance.ReturnToPool(poolTag, gameObject);
}