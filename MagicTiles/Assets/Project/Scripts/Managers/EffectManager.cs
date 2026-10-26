using UnityEngine;
using DG.Tweening;

public class EffectManager : MonoBehaviour
{
    [Header("UI Settings")]
    public Transform fixedTextPosition;

    void OnEnable()
    {
        GameEvents.OnTileHit += PlayHitEffect;
        GameEvents.OnTileMiss += PlayMissEffect;
    }

    void OnDisable()
    {
        GameEvents.OnTileHit -= PlayHitEffect;
        GameEvents.OnTileMiss -= PlayMissEffect;
    }

    private void PlayHitEffect(int points, string rating, Vector3 tilePos)
    {
        float targetY = GameManager.Instance.TargetLineY;
        Vector3 glowPos = new Vector3(tilePos.x, targetY, 0f);
        GameObject glow = SimplePooler.Instance.SpawnFromPool("LaneGlow", glowPos, Quaternion.identity);

        if (glow != null)
        {
            SpriteRenderer glowSprite = glow.GetComponentInChildren<SpriteRenderer>();
            if (glowSprite != null)
            {
                glow.transform.DOKill();
                glowSprite.DOKill();
                glowSprite.color = new Color(1f, 1f, 1f, 0.6f);
                glow.transform.localScale = new Vector3(glow.transform.localScale.x, 0f, 1f);
                glow.transform.DOScaleY(15f, 0.15f).SetEase(Ease.OutQuad);
                glowSprite.DOFade(0f, 0.25f).SetDelay(0.05f).OnComplete(() =>
                {
                    SimplePooler.Instance.ReturnToPool("LaneGlow", glow);
                });
            }
        }

        Vector3 tapVfxPos = new Vector3(tilePos.x, tilePos.y, -2f);
        GameObject tapEff = SimplePooler.Instance.SpawnFromPool("TappedEff", tapVfxPos, Quaternion.identity);
        if (tapEff != null)
        {
            ParticleSystem[] tapPS = tapEff.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in tapPS)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play();
            }
        }

        Vector3 textSpawnPos = (fixedTextPosition != null) ? fixedTextPosition.position : tilePos + Vector3.up;
        GameObject floatVfx = SimplePooler.Instance.SpawnFromPool("TappedEff_Floating", new Vector3(textSpawnPos.x, textSpawnPos.y, -3f), Quaternion.identity);
        if (floatVfx != null)
        {
            ParticleSystem[] floatPS = floatVfx.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in floatPS)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play();
            }
        }

        GameObject textObj = SimplePooler.Instance.SpawnFromPool("FloatingText", new Vector3(textSpawnPos.x, textSpawnPos.y, -4f), Quaternion.identity);
        if (textObj != null)
        {
            FloatingText ft = textObj.GetComponent<FloatingText>();
            if (ft != null)
            {
                Color ratingColor = Color.white;
                if (rating == "Perfect" || rating == "PERFECT") ratingColor = new Color(1f, 0.8f, 0f);
                else if (rating == "Great" || rating == "GREAT") ratingColor = new Color(0.2f, 1f, 0.2f);
                else if (rating == "Cool" || rating == "COOL") ratingColor = new Color(0.2f, 0.8f, 1f);

                ft.Setup(rating, ratingColor);
            }
        }
    }

    private void PlayMissEffect(Vector3 missPos)
    {
        Vector3 textSpawnPos = (fixedTextPosition != null) ? fixedTextPosition.position : missPos + Vector3.up;
        GameObject textObj = SimplePooler.Instance.SpawnFromPool("FloatingText", textSpawnPos, Quaternion.identity);
        if (textObj != null)
        {
            FloatingText ft = textObj.GetComponent<FloatingText>();
            if (ft != null) ft.Setup("MISS", Color.red);
        }
    }
}