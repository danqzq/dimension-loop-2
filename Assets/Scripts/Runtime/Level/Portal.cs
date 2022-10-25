using DG.Tweening;
using UnityEngine;

namespace Dan.Level
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private Transform _otherPortal;
        [SerializeField] private bool _changeX;
        [SerializeField] private bool _changeY;

        [SerializeField] private Vector2 _teleportOffset;
            
        [SerializeField] private ParticleSystem _particleSystem;

        [SerializeField] private Transform _platform;

        private void Start()
        {
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>().transform;
            spriteRenderer.localPosition += Vector3.down * 5;
            spriteRenderer.DOLocalMove(Vector3.zero, 0.5f);

            if (_platform == null) return;
            var scale = _platform.localScale;
            scale.y = 0;
            _platform.localScale = scale;
            _platform.DOScaleY(0.4f, 0.5f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<Player>(out var player)) return;
            if (Time.time - player.LastTeleportTime < 0.075f || !player.CanTeleport) return;

            var destination = _otherPortal.position;
            
            var t = other.transform;
            var position = t.position;
            _particleSystem.transform.position = position + transform.up * -0.5f;
            _particleSystem.Play();
            
            var newPos = new Vector2(_changeX ? destination.x : position.x,
                _changeY ? destination.y : position.y) + _teleportOffset;
            
            player.Teleport(newPos);
        }
    }
}
