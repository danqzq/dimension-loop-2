using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dan
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _pressStartText;
        [SerializeField] private Animator _animator;
        
        private bool _pressedStart;

        private void Start()
        {
            _pressStartText.DOFade(0f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        
        private void Update()
        {
            if (!Input.anyKeyDown || _pressedStart) return;
            
            _pressedStart = true;
            
            _animator.enabled = true;
            _pressStartText.DOKill();
            Destroy(_pressStartText.gameObject);
            
            this.DoAfter(1f, () => SceneManager.LoadScene("SampleScene"));
        }
    }
}
