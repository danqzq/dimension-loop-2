using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Dan.Level
{
    [System.Serializable]
    public class GameMode
    {
        public string name;
        public Color color;
        public UnityEvent onGameModeStart;
        public UnityEvent onGameModeEnd;
        public GameObject tutorialText;
    }
    
    public class GameManager : MonoBehaviour
    {
        public static GameManager Obj;

        [SerializeField] private int _maxBoxHealth = 5;
        [SerializeField] private Image _fadeOut, _fadeIn;
        [SerializeField] private TextMeshProUGUI _timerText, _scoreText;
        [SerializeField] private GameObject _targetPointPrefab;

        [SerializeField] private SpriteRenderer _boxDamage;
        [SerializeField] private Sprite[] _boxDamageSprites;
        
        [SerializeField] private ParticleSystem[] _boxParticles;
        
        [SerializeField] private Box _box;
        
        [SerializeField] private GameMode[] _gameModes;
        [SerializeField] private SpriteRenderer[] _fadeInBoxSprites;

        [SerializeField] private RectTransform _wave;
        [SerializeField] private AudioSource[] _gameModeSongs;

        [SerializeField] private Saying[] _sayings;
        [SerializeField] private GameObject _boxTutorials;

        private bool _isTargetEnabled;
 
        private ModifierManager _modifierManager;
        private PortalManager _portalManager;
        
        private GameMode _currentGameMode;

        private List<GameMode> _gameModesSet;

        private GameObject _currentBox;
        private int _currentBoxHealth;
        
        private TargetPoint _currentTargetPoint;
        
        private int _score;
        private int _boxesBroken;
        private float _waveTargetYPosition;
        
        private Player _player;

        private DateTime _timeSinceLevelStart;

        private void Awake()
        {
            Obj = this;
            _modifierManager = GetComponent<ModifierManager>();
            _portalManager = FindObjectOfType<PortalManager>();
            
            _currentGameMode = _gameModes[0];
            _gameModesSet = new List<GameMode>(_gameModes[1..]);
            
            _player = FindObjectOfType<Player>();

            _fadeOut.DOFade(0f, 0.5f);
            
            DOTween.SetTweensCapacity(512, 128);
        }

        private void Start()
        {
            _currentBoxHealth = _maxBoxHealth;
            
            _timeSinceLevelStart =DateTime.Now;
        }
        
        public void ShowTarget()
        {
            _isTargetEnabled = true;
            ResetTargetPoint(true);
        }

        public void StartTimer() => StartCoroutine(TimerCoroutine());
        
        private void OnTargetPointReached()
        {
            ResetTargetPoint();
            if (_currentBoxHealth > 1) ResetBox();
            _score++;
            _scoreText.text = _score.ToString();
            _sayings.ForEach(x => x.gameObject.SetActive(false));
            if (_sayings.Any(x => x.atTargetScore == _score))
            {
                _sayings.First(x => x.atTargetScore == _score).gameObject.SetActive(true);
                _sayings.First(x => x.atTargetScore == _score).Call();
            }
            if (_score > 50)
            {
                FindObjectOfType<Boss>(true).gameObject.SetActive(true);
            }
            this.DoAfter(3f, () => _waveTargetYPosition = _score);
        }
        
        private void ResetTargetPoint(bool forceActivate = false)
        {
            if (_currentTargetPoint != null)
            {
                Destroy(_currentTargetPoint.gameObject);
            }
            var t = Instantiate(_targetPointPrefab, _box.transform);
            var choice = Random.Range(0, 4);
            float RandomVal() => Random.Range(-2f, 2f);

            if (_currentGameMode.name == "Teleport")
            {
                choice = _portalManager.CurrentActivePortal is 0 or 2 ? Random.Range(2, 4) : Random.Range(0, 2);
            }
            
            t.transform.localPosition = choice switch
            {
                0 => new Vector3(RandomVal(), 3.4f, 0),
                1 => new Vector3(RandomVal(), -3.4f, 0),
                2 => new Vector3(3.4f, RandomVal(), 0),
                _ => new Vector3(-3.4f, RandomVal(), 0)
            };
            t.transform.localEulerAngles = choice switch
            {
                0 or 1 => new Vector3(0, 0, 0),
                2 or _ => new Vector3(0, 0, 90),
            };
            (_currentTargetPoint = t.GetComponent<TargetPoint>()).Init(OnTargetPointReached);
            if (forceActivate) _currentTargetPoint.IsActivated = true;
        }

        private IEnumerator TimerCoroutine()
        {
            for (int i = 10; i > 1; i--)
            {
                _timerText.text = i.ToString();
                _timerText.color = i <= 3 ? Color.yellow : Color.white;
                yield return new WaitForSeconds(1);
            }
            
            _currentGameMode.onGameModeEnd.Invoke();
            _currentGameMode.tutorialText.SetActive(false);

            _currentBoxHealth = _maxBoxHealth;
            if (_gameModesSet.Count > 0)
            {
                _currentGameMode = _gameModesSet.SelectRandom();
                _gameModesSet.Remove(_currentGameMode);
            }
            else
            {
                _currentGameMode = _gameModes[0];
                _gameModesSet = new List<GameMode>(_gameModes.Except(new [] {_currentGameMode}));
            }
            _fadeInBoxSprites[0].transform.parent.localScale = Vector3.one * 2;
            _fadeInBoxSprites.ForEach(x => x.DOColor(_currentGameMode.color, 1f));
            _timerText.text = "1";
            _player.CanTeleport = false;
            
            yield return _fadeInBoxSprites[0].transform.parent.DOScale(1f, 1f).SetEase(Ease.InOutSine).WaitForCompletion();
            
            this.DoAfter(1f, () => _player.CanTeleport = true);
            
            _fadeInBoxSprites.ForEach(x => x.color = Color.clear);
            _box.SetColor(_currentGameMode.color);
            _currentGameMode.onGameModeStart.Invoke();
            if (_score < 20)
            {
                _boxTutorials.SetActive(true);
                _currentGameMode.tutorialText.SetActive(true);
            }
            else
            {
                _boxTutorials.SetActive(false);
            }
            var colors = _box.GetColors();
            _boxParticles.ForEach((x, i) =>
            {
                var main = x.main;
                main.startColor = colors[i];
            });
            foreach (var song in _gameModeSongs)
            {
                song.DOFade(0f, 1f);
            }
            var s = _gameModeSongs.First(x => x.name == _currentGameMode.name);
            s.DOKill();
            s.DOFade(1f, 1f);
            _boxDamage.sprite = _boxDamageSprites[4];
            
            _modifierManager.NextModifier();

            StartCoroutine(TimerCoroutine());
        }
        
        public void HitBox(out Color color)
        {
            _currentBoxHealth--;
            if (_currentBoxHealth <= 0) 
                ResetBox();
            color = _box.GetColors()[0];
            _boxDamage.sprite = _boxDamageSprites[_currentBoxHealth];
            _boxDamage.transform.eulerAngles = new Vector3(0, 0, (0, 90).Random());
            SFXManager.Play("hit");
        }

        private void ResetBox()
        {
            _boxesBroken++;
            _sayings.ForEach(x =>
            {
                if (x.atTargetScore == 0)
                    x.gameObject.SetActive(false);
            });
            if (_sayings.Any(x => x.atScore == _boxesBroken))
            {
                _sayings.First(x => x.atScore == _boxesBroken).gameObject.SetActive(true);
                _sayings.First(x => x.atScore == _boxesBroken).Call();
            }
            _currentBoxHealth = _maxBoxHealth;
            if (_isTargetEnabled) ResetTargetPoint();
            CameraShake.StartShake(3f, 0.5f);
            SFXManager.Play("explode");
            foreach (var boxParticle in _boxParticles)
            {
                boxParticle.Play();
            }

            _box.transform.localScale = Vector3.zero;
            _box.transform.rotation = Quaternion.Euler(0, 0, -90);
            _box.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetDelay(0.25f);
            _box.transform.DORotate(Vector3.zero, 0.5f).SetEase(Ease.OutBack).SetDelay(0.25f).OnComplete(() =>
            {
                if (_currentTargetPoint != null)
                {
                    _currentTargetPoint.IsActivated = true;
                }
            });
        }

        private void Update()
        {
            var anchoredPosition = _wave.anchoredPosition;
            anchoredPosition.x += -125 * Time.deltaTime;
            _wave.anchoredPosition = anchoredPosition;
            _wave.anchoredPosition = Vector2.Lerp(anchoredPosition, 
                new Vector2(anchoredPosition.x, _waveTargetYPosition * 10f - 500), 10f * Time.deltaTime);
            
            if (_wave.anchoredPosition.x > -65) return;
            
            var position = _wave.anchoredPosition;
            position = new Vector2(140, position.y);
            _wave.anchoredPosition = position;
        }

        public void GameOver()
        {
            if (PlayerPrefs.GetInt("Score") < _score)
            {
                PlayerPrefs.SetInt("Score", _score);
                var time = DateTime.Now - _timeSinceLevelStart;
                PlayerPrefs.SetString("Time", time.ToString("mm\\:ss"));
            }

            _fadeIn.DOFade(1f, 0.9f);
            StartCoroutine(SwitchScene());
        }

        public void Win()
        {
            if (PlayerPrefs.GetInt("Score") < _score)
            {
                PlayerPrefs.SetInt("Score", _score);
                var time = DateTime.Now - _timeSinceLevelStart;
                PlayerPrefs.SetString("Time", time.ToString("mm\\:ss"));
            }
            PlayerPrefs.SetInt("Win", 1);
            _fadeIn.DOFade(1f, 0.9f);
            StartCoroutine(SwitchScene());
        }

        private IEnumerator SwitchScene()
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("Ending");
        }
    }
}
