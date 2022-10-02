using UnityEngine;

namespace Dan.Level
{
    public class ScrollEffect : MonoBehaviour
    {
        [SerializeField] private float _scrollSpeed = 0.5f;
        [SerializeField] private float _minX, _maxX;
        
        private void Update()
        {
            transform.localPosition += Vector3.left * _scrollSpeed * Time.deltaTime;
            
            if (transform.localPosition.x >= _minX) return;
            
            var t = transform;
            var position = t.localPosition;
            position = new Vector3(_maxX, position.y, position.z);
            t.localPosition = position;
        }
    }
}