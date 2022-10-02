using DG.Tweening;
using UnityEngine;

namespace Dan.Level
{
    public class ExplosionProjectile : MonoBehaviour
    {
        private void Start()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            var t = transform;
            t.localScale = Vector3.zero;
            t.DOScale(2f, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                if (Physics2D.OverlapCircle(transform.position, 0.875f, LayerMask.GetMask("Player")))
                    Player.TakeDamage();
                spriteRenderer.DOFade(0f, 0.25f).OnComplete(() => Destroy(gameObject));
            });
        }
    }
}
