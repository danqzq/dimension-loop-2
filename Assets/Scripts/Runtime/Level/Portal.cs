using UnityEngine;

namespace Dan.Level
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private Transform _otherPortal;
        [SerializeField] private bool _changeX;
        [SerializeField] private bool _changeY;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<Player>(out var player)) return;
            if (Time.time - player.LastTeleportTime < 0.125f) return;
            
            player.LastTeleportTime = Time.time;
            
            var destination = _otherPortal.position;
            
            var t = other.transform;
            var position = t.position;
            
            var newPos = new Vector2(_changeX ? destination.x : position.x,
                _changeY ? destination.y : position.y);
            
            t.position = newPos;
        }
    }
}
