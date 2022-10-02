using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Dan.Level
{
    [System.Serializable]
    public class Modifier
    {
        public string name;
        public GameObject objectToSpawn;
        public GameObject warning;
        public float spawnRate = 1f;
    }
    
    public class ModifierManager : MonoBehaviour
    {
        [SerializeField] private Modifier[] _modifiers;
        
        public static ModifierManager Obj;
        
        public bool IsEnabled { get; set; }

        private List<Modifier> _modifiersList;
        private Modifier _currentModifier;
        
        private void Start()
        {
            Obj = this;
            _currentModifier = _modifiers[0];
            _modifiersList = new List<Modifier>(_modifiers[1..]);
        }

        public void NextModifier()
        {
            if (!IsEnabled) return;
            if (_modifiersList.Count == 0)
            {
                _modifiersList = new List<Modifier>(_modifiers[1..]);
            }
            
            _currentModifier = _modifiersList.SelectRandom();
            _modifiersList.Remove(_currentModifier);
            StartCoroutine(ModifierCoroutine());
        }

        private IEnumerator ModifierCoroutine()
        {
            var time = 0f;
            var timesActivated = 0;
            while (time < 7f)
            {
                time += Time.deltaTime;
                yield return null;
                var spawnTime = 1f / _currentModifier.spawnRate;
                if (time - timesActivated * spawnTime < spawnTime) continue;
                timesActivated++;
                float RandomVal() => Random.Range(-1, 2) * 2;

                switch (_currentModifier.name)
                {
                    case "Explosions":
                        SpawnExplosion(new Vector2(RandomVal(), RandomVal()));
                        break;
                    case "Spears":
                        SpawnSpear(RandomVal());
                        break;
                    case "Hands":
                        SpawnHand(RandomVal());
                        break;
                }
            }
        }

        public static void HandModifier() => Obj.StartCoroutine(Obj.HandModifierCoroutine());
        
        public IEnumerator HandModifierCoroutine()
        {
            var time = 0f;
            var timesActivated = 0;
            while (time < 7f)
            {
                time += Time.deltaTime;
                yield return null;
                var modifier = _modifiers[0];
                var spawnTime = 1f / modifier.spawnRate;
                if (time - timesActivated * spawnTime < spawnTime) continue;
                timesActivated++;
                float RandomVal() => Random.Range(-1, 2) * 2;
                var x = RandomVal();

                for (int i = -2; i < 3; i += 2)
                {
                    var position = new Vector2(x, i);
                    var warning = Instantiate(modifier.warning, position, Quaternion.identity);
                    var t = warning.GetComponentInChildren<TextMeshPro>().transform;
                    t.localScale = Vector3.zero;
                    t.DOScale(0.375f, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
                        t.DOScale(0f, 0.5f).SetEase(Ease.InOutSine).SetDelay(0.75f)
                            .OnComplete(() => Destroy(warning)));
                }
                var choice = Random.Range(0, 2) == 1;
                var pos = new Vector2(x, choice ? -10 : 10);
                this.DoAfter(1.25f, () =>
                {
                    var obj = Instantiate(modifier.objectToSpawn, pos, Quaternion.identity).transform;
                    var scale = obj.transform.localScale;
                    scale.x *= choice ? -1 : 1;
                    obj.localScale = scale;
                    obj.DORotate(Vector3.forward * -1800, 2f).SetEase(Ease.Linear);
                    obj.DOMoveY(choice ? 1.75f : -1.75f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                        obj.DOMove(pos, 0.5f).SetEase(Ease.InOutSine).SetDelay(1f).OnComplete(() => 
                            Destroy(obj.gameObject)));
                });
            }
        }

        private void SpawnExplosion(Vector2 position)
        {
            var warning = Instantiate(_currentModifier.warning, position, Quaternion.identity);
            warning.GetComponentInChildren<SpriteRenderer>().transform.DOLocalRotate(Vector3.forward * 180f,
                1.5f).SetEase(Ease.Linear);
            var t = warning.GetComponentInChildren<TextMeshPro>().transform;
            t.localScale = Vector3.zero;
            t.DOScale(0.375f, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
                t.DOScale(0f, 0.5f).SetEase(Ease.InOutSine).SetDelay(1f)
                    .OnComplete(() => Destroy(warning)));
            this.DoAfter(1.75f, () =>
                Instantiate(_currentModifier.objectToSpawn, position, Quaternion.identity));
        }
        
        private void SpawnSpear(float y)
        {
            for (int i = -2; i < 3; i += 2)
            {
                var position = new Vector2(i, y);
                var warning = Instantiate(_currentModifier.warning, position, Quaternion.identity);
                var t = warning.GetComponentInChildren<TextMeshPro>().transform;
                t.localScale = Vector3.zero;
                t.DOScale(0.375f, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
                    t.DOScale(0f, 0.5f).SetEase(Ease.InOutSine).SetDelay(1f)
                        .OnComplete(() => Destroy(warning)));
            }
            var choice = Random.Range(0, 2) == 1;
            var pos = new Vector2(choice ? -10 : 10, y);
            this.DoAfter(1.75f, () =>
            {
                var obj = Instantiate(_currentModifier.objectToSpawn, pos, Quaternion.identity).transform;
                var scale = obj.localScale;
                scale.x *= choice ? -1 : 1;
                obj.localScale = scale;
                obj.DOMoveX(choice ? 1.75f : -1.75f, 1f).SetEase(Ease.Linear).OnComplete(() =>
                    obj.DOMove(pos, 0.5f).SetEase(Ease.InOutSine).SetDelay(1.25f).OnComplete(() =>
                        Destroy(obj.gameObject)));
            });
        }
        
        private void SpawnHand(float x)
        {
            for (int i = -2; i < 3; i += 2)
            {
                var position = new Vector2(x, i);
                var warning = Instantiate(_currentModifier.warning, position, Quaternion.identity);
                var t = warning.GetComponentInChildren<TextMeshPro>().transform;
                t.localScale = Vector3.zero;
                t.DOScale(0.375f, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
                    t.DOScale(0f, 0.5f).SetEase(Ease.InOutSine).SetDelay(0.75f)
                        .OnComplete(() => Destroy(warning)));
            }
            var choice = Random.Range(0, 2) == 1;
            var pos = new Vector2(x, choice ? -10 : 10);
            this.DoAfter(1.25f, () =>
            {
                var obj = Instantiate(_currentModifier.objectToSpawn, pos, Quaternion.identity).transform;
                var scale = obj.transform.localScale;
                scale.x *= choice ? -1 : 1;
                obj.localScale = scale;
                obj.DORotate(Vector3.forward * -1800, 2f).SetEase(Ease.Linear);
                obj.DOMoveY(choice ? 1.75f : -1.75f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                    obj.DOMove(pos, 0.5f).SetEase(Ease.InOutSine).SetDelay(1f).OnComplete(() => 
                        Destroy(obj.gameObject)));
            });
        }
    }
}