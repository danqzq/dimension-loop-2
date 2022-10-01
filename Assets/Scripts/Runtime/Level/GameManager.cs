using System.Collections;
using TMPro;
using UnityEngine;

namespace Dan.Level
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Obj;
        
        [SerializeField] private TextMeshProUGUI _timerText, _scoreText;
        [SerializeField] private GameObject _targetPointPrefab;
        
        [SerializeField] private GameObject[] _boxes;

        [SerializeField] private SpriteRenderer _boxDamage;
        [SerializeField] private Sprite[] _boxDamageSprites;

        private GameObject _currentBox;
        private int _currentBoxHealth;
        
        private TargetPoint _currentTargetPoint;
        
        private int _score;

        private void Awake()
        {
            Obj = this;
        }

        private void Start()
        {
            StartCoroutine(TimerCoroutine());
            
            _score = -1;
            _currentBoxHealth = 3;
            _currentBox = _boxes[0];
            _currentBox.SetActive(true);
            
            OnTargetPointReached();
        }
        
        private void OnTargetPointReached()
        {
            if (_currentTargetPoint != null)
            {
                Destroy(_currentTargetPoint.gameObject);
            }
            var t = Instantiate(_targetPointPrefab, transform.position, Quaternion.identity);
            var choice = Random.Range(0, 4);
            float RandomVal() => Random.Range(-2f, 2f);
            t.transform.position = choice switch
            {
                0 => new Vector3(RandomVal(), 3.4f, 0),
                1 => new Vector3(RandomVal(), -3.4f, 0),
                2 => new Vector3(3.4f, RandomVal(), 0),
                _ => new Vector3(-3.4f, RandomVal(), 0)
            };
            t.transform.eulerAngles = choice switch
            {
                0 or 1 => new Vector3(0, 0, 0),
                2 or _ => new Vector3(0, 0, 90),
            };
            (_currentTargetPoint = t.GetComponent<TargetPoint>()).Init(OnTargetPointReached);
            _score++;
            _scoreText.text = _score.ToString();
        }

        private IEnumerator TimerCoroutine()
        {
            for (int i = 10; i > 0; i--)
            {
                _timerText.text = i.ToString();
                yield return new WaitForSeconds(1);
            }
            
            _currentBoxHealth = 3;
            _currentBox.SetActive(false);
            _currentBox = _boxes[Random.Range(0, _boxes.Length)];
            _currentBox.SetActive(true);
            _boxDamage.sprite = _boxDamageSprites[3];

            StartCoroutine(TimerCoroutine());
        }
        
        public void HitBox()
        {
            _currentBoxHealth--;
            if (_currentBoxHealth <= 0)
            {
                _currentBoxHealth = 3;
                OnTargetPointReached();
                CameraShake.StartShake(1f, 1f);
            }
            _boxDamage.sprite = _boxDamageSprites[_currentBoxHealth];
            var choice = Random.Range(0, 4);
            _boxDamage.transform.eulerAngles = choice switch
            {
                0 or 1 => new Vector3(0, 0, 0),
                2 or _ => new Vector3(0, 0, 90),
            };
        }
    }
}
