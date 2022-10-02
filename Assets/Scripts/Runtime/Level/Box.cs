using UnityEngine;

namespace Dan.Level
{
    public class Box : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _top, _bottom, _left, _right;
        
        public void SetColor(Color color)
        {
            _top.color = color;
            _bottom.color = color;
            _left.color = color;
            _right.color = color;
        }
        
        public Color[] GetColors() => new[] {_top.color, _bottom.color, _left.color, _right.color};
    }
}
