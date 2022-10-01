using System;
using UnityEngine;

namespace Dan.Level
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _delayBetweenShots = 0.25f;
        [SerializeField] private float _shootRange = 5f, _shootForce = 100f;
        
        public float LastTeleportTime { get; set; }

        private float _lastShotTime;

        private Vector2? _touchPosition, _liftPosition;

        private Rigidbody2D _rb;
        private LineRenderer _lr;
        
        private Camera _camera;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _lr = GetComponentInChildren<LineRenderer>();
            
            _camera = Camera.main;
        }

        public void OnPointerDown()
        {
            if (Time.time - _lastShotTime < _delayBetweenShots)
                return;
            
            _touchPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        }
        
        public void OnPointerUp()
        {
            if (!_touchPosition.HasValue) return;
            
            _liftPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            var direction = Vector2.ClampMagnitude(_touchPosition.Value - _liftPosition!.Value, _shootRange);
            _rb.AddForce(direction * _shootForce);
            
            _lastShotTime = Time.time;
            
            _touchPosition = null;
        }
        
        private void Update()
        {
            var position = transform.position;
            position.z = 0;
            _lr.SetPosition(0, position);
            if (_touchPosition.HasValue)
            {
                var targetPos = position + Vector3.ClampMagnitude(_touchPosition.Value -
                    (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition), _shootRange);
                targetPos.z = 0;
                _lr.SetPosition(1, targetPos);
            }
            else
            {
                _lr.SetPosition(1, position);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Box") && _rb.velocity.magnitude > 4)
            {
                GameManager.Obj.HitBox();
            }
        }
    }
}
