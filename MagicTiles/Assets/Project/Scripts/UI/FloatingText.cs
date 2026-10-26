using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    public TextMeshPro ratingText;
    public string poolTag = "FloatingText";
    public float duration = 0.5f;

    public void Setup(string rating, Color ratingColor)
    {
        if (ratingText == null) ratingText = GetComponent<TextMeshPro>();

        ratingText.text = rating.ToUpper();
        ratingText.color = ratingColor;

        transform.DOKill();
        ratingText.DOKill();

        transform.localScale = Vector3.zero;
        ratingText.alpha = 1f;

        transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(duration * 0.6f);
        seq.Append(ratingText.DOFade(0f, 0.2f));

        seq.OnComplete(() => SimplePooler.Instance.ReturnToPool(poolTag, gameObject));
    }
}