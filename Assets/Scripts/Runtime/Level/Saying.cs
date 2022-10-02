using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Dan.Level
{
    public class Saying : MonoBehaviour
    {
        public UnityEvent OnSaid;
        public int atTargetScore;
        public int atScore;
        
        public void Call() =>OnSaid?.Invoke();
    }
}