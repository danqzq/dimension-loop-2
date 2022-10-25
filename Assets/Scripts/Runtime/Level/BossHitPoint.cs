using DG.Tweening;
using UnityEngine;

namespace Dan.Level
{
    public class BossHitPoint : MonoBehaviour
    {
        [SerializeField] private Vector3 _initPos;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void ResetPos()
        {
            transform.position = _initPos;
            transform.localEulerAngles = Vector3.zero;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            Boss.TakeDamage();
            _spriteRenderer.DOColor(Color.red, 0.1f).OnComplete(() => _spriteRenderer.DOColor(Color.white, 0.1f));
        }
    }
}