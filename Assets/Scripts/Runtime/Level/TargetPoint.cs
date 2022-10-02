using System;
using UnityEngine;

namespace Dan.Level
{
    public class TargetPoint : MonoBehaviour
    {
        public bool IsActivated { get; set; }
        
        [SerializeField] private GameObject _particleEffect;

        private Action _onTargetReached;

        private void Start()
        {
            
        }

        public void Init(Action onTargetReached)
        {
            _onTargetReached = onTargetReached;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsActivated) return;
            if (other.gameObject.CompareTag("Player"))
            {
                var particle = Instantiate(_particleEffect, transform.position, Quaternion.identity);
                Destroy(particle, 5f);
                _onTargetReached?.Invoke();
            }
        }
    }
}