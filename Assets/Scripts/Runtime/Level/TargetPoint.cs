using System;
using UnityEngine;

namespace Dan.Level
{
    public class TargetPoint : MonoBehaviour
    {
        private Action _onTargetReached;
        
        public void Init(Action onTargetReached)
        {
            _onTargetReached = onTargetReached;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _onTargetReached?.Invoke();
            }
        }
    }
}