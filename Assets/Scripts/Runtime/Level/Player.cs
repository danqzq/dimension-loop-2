using DG.Tweening;
using UnityEngine;

namespace Dan.Level
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private int _health;
        [SerializeField] private GameObject[] _healthObjects;
        
        [SerializeField] private float _delayBetweenShots = 0.25f;
        [SerializeField] private float _shootRange = 5f, _shootForce = 100f;
        
        [SerializeField] private GameObject _collisionEffect;

        private static Player _instance;
        
        public float LastTeleportTime { get; private set; }
        
        public bool CanTeleport { get; set; }

        private float _lastShotTime, _lastHitTime;

        private Vector2? _touchPosition, _liftPosition;

        private Rigidbody2D _rb;
        private LineRenderer _lr;
        private TrailRenderer _trail;
        private SpriteRenderer _sr;
        
        private Camera _camera;
        
        public bool ShouldNotTouchBox { get; set; }

        private void Awake()
        {
            _instance = this;
            _health = 5;
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _lr = GetComponentInChildren<LineRenderer>();
            _trail = GetComponentInChildren<TrailRenderer>();
            _sr = GetComponent<SpriteRenderer>();
            
            _camera = Camera.main;
            CanTeleport = true;
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
            var t = transform;
            var position = t.position;
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
            
            t.position = new Vector3(
                Mathf.Abs(position.x) > 7 ? 0 : position.x,
                Mathf.Abs(position.y) > 7 ? 0 : position.y);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Box") || _rb.velocity.magnitude < 5) return;

            GameManager.Obj.HitBox(out var color);
            CameraShake.StartShake(1f, 0.5f);
            var effect = Instantiate(_collisionEffect, other.GetContact(0).point, Quaternion.identity);
            var main = effect.GetComponent<ParticleSystem>().main;
            main.startColor = color;
            Destroy(effect, 1f);
            
            if (ShouldNotTouchBox)
                TakeDamage();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Spear"))
            {
                TakeDamage();
            }
        }

        public void Teleport(Vector2 position)
        {
            transform.position = position;
            LastTeleportTime = Time.time;
            _trail.Clear();
            SFXManager.Play("teleport");
        }
        
        public static void TakeDamage()
        {
            _instance.TakeDamageInternal();
        }
        
        private void TakeDamageInternal()
        {
            if (Time.time - _lastHitTime < 1f) return;
            _health--;
            
            for (int i = 0; i < _healthObjects.Length; i++) 
                _healthObjects[i].SetActive(i < _health);
            
            CameraShake.StartShake(1f, 0.5f);
            SFXManager.Play("hit");

            _sr.DOColor(Color.red, 0.2f).SetLoops(4, LoopType.Yoyo).OnComplete(() => _sr.DOColor(Color.white, 0.2f));
            _lastHitTime = Time.time;
            
            if (_health <= 0)
            {
                GameManager.Obj.GameOver();
                _sr.enabled = false;
                _trail.enabled = false;
            }
        }
    }
}
