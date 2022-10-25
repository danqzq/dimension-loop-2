using System;
using UnityEngine;

namespace Dan.Level
{
    public class TargetPoint : MonoBehaviour
    {
        public bool IsActivated { get; set; }
        
        [SerializeField] private GameObject _particleEffect;

        private Action _onTargetReached;

        public void Init(Action onTargetReached)
        {
            _onTargetReached = onTargetReached;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsActivated || !other.gameObject.CompareTag("Player")) return;
            var particle = Instantiate(_particleEffect, transform.position, Quaternion.identity);
            Destroy(particle, 5f);
            _onTargetReached?.Invoke();
        }
    }
}