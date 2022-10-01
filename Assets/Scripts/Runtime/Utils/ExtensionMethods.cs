using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dan
{
    public static class ExtensionMethods
    {
        public static void RemoveCloneName(this GameObject gameObject) => gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
        
        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<TKey, TValue> action)
        {
            foreach (var mType in dict)
                action?.Invoke(mType.Key, mType.Value);
        }
        
        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<TValue> value)
        {
            foreach (var mType in dict)
                value?.Invoke(mType.Value);
        }
        
        public static T GetOrAddComponent<T>(this GameObject tGO) where T : Component
        {
            if (null == tGO) { return null; }

            var type = typeof(T);
            var component = tGO.GetComponent(type);
            if (null == component)
            {
                component = tGO.AddComponent(type);
            }
            return component as T;
        }

        public static bool IsNullOrEmpty<T>(this T[] array) => array == null || array.Length < 1;
        
        public static bool IsNullOrEmpty<T>(this List<T> list) => list == null || list.Count < 1;
        
        public static bool IsNullOrEmpty<T>(this Queue<T> queue) => queue == null || queue.Count < 1;
        
        public static bool IsNullOrEmpty<T1, T2>(this Dictionary<T1, T2> dictionary) => dictionary == null || dictionary.Count < 1;
        
        public static bool IsNull(this GameObject go) => go == null;
        
        public static bool IsNull(this Component component) => component == null;
        
        public static T SelectRandom<T>(this T[] array)
        {
            return array.IsNullOrEmpty() ? default : array[UnityEngine.Random.Range(0, array.Length)];
        }
        
        public static T SelectRandom<T>(this List<T> list)
        {
            return list.IsNullOrEmpty() ? default : list[UnityEngine.Random.Range(0, list.Count)];
        }
        
        public static void DoAfterUnScaled(this MonoBehaviour monoBehaviour, float delay, Action action)
        {
            monoBehaviour.StartCoroutine(DoAfterUnScaledCoroutine(delay, action));

            IEnumerator DoAfterUnScaledCoroutine(float delay, Action action)
            {
                yield return new WaitForSecondsRealtime(delay);
                action?.Invoke();
            }
        }
        public static void DoAfter(this MonoBehaviour monoBehaviour, float delay, Action action)
        {
            monoBehaviour.StartCoroutine(DoAfterCoroutine(delay, action));
        }
        
        private static IEnumerator DoAfterCoroutine(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
        
        public static void DoAfter(this MonoBehaviour monoBehaviour, Func<bool> predicate, Action action)
        {
            monoBehaviour.StartCoroutine(DoAfterCoroutine(predicate, action));
        }
        
        private static IEnumerator DoAfterCoroutine(Func<bool> predicate, Action action)
        {
            yield return new WaitUntil(predicate);
            action?.Invoke();
        }

        public static IEnumerator InfiniteSpriteCycle(this SpriteRenderer spriteRenderer, Sprite[] sprites, float speed)
        {
            foreach (var sprite in sprites)
            {
                spriteRenderer.sprite = sprite;
                yield return new WaitForSeconds(1f / speed);
            }
            
            yield return InfiniteSpriteCycle(spriteRenderer, sprites, speed);
        }
        
        public static IEnumerator InfiniteSpriteCycle(this Image spriteRenderer, Sprite[] sprites, float speed)
        {
            foreach (var sprite in sprites)
            {
                spriteRenderer.sprite = sprite;
                yield return new WaitForSeconds(1f / speed);
            }
            
            yield return InfiniteSpriteCycle(spriteRenderer, sprites, speed);
        }

        public static bool TryGetChildWithName(this Transform transform, string name, out Transform result)
        {
            foreach (Transform child in transform)
            {
                if (child.name != name) continue;
                result = child;
                return true;
            }

            result = null;
            return false;
        }
        
        public static void Fade(this CanvasGroup canvasGroup, bool @in, float duration, Action onComplete = null)
        {
            canvasGroup.interactable = @in;
            canvasGroup.blocksRaycasts = @in;
            canvasGroup.DOFade(@in ? 1 : 0, duration).OnComplete(() => onComplete?.Invoke());
        }
        
        public static readonly int BlendAmount = Shader.PropertyToID("_BlendAmount");

        public static void DOFlash(this SpriteRenderer sprite, float duration = 0.2f, float value = 0.5f)
        {
            sprite.material.DOComplete();
            if (!sprite.material.HasProperty(BlendAmount)) return;
            sprite.material.SetFloat(BlendAmount, value);
            sprite.material.DOFloat(0f, BlendAmount, duration).SetEase(Ease.InOutSine);
        }
    }
}