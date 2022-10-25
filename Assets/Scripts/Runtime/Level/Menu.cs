using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Dan
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _pressStartText;
        [SerializeField] private Animator _animator;

        [SerializeField] private AudioMixerGroup _audioMixerGroup;
        [SerializeField] private TextMeshProUGUI[] _audioTexts;

        [SerializeField] private bool _resetSettings;
        
        private bool _pressedStart;

        private void Start()
        {
            _pressStartText.DOFade(0f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            if (!_resetSettings) return;
            PlayerPrefs.SetInt("Music", 1);
            PlayerPrefs.SetInt("SFX", 1);
        }

        public void Play()
        {
            _pressedStart = true;
            
            GetComponent<AudioSource>().Play();

            _animator.enabled = true;
            _pressStartText.DOKill();
            Destroy(_pressStartText.gameObject);
            
            _audioTexts.ForEach(x => x.transform.parent.gameObject.SetActive(false));
            
            this.DoAfter(1f, () => SceneManager.LoadScene("SampleScene"));
        }
        
        public void ToggleMusic()
        {
            PlayerPrefs.SetInt("Music", PlayerPrefs.GetInt("Music") == 1 ? 0 : 1);
            _audioMixerGroup.audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetInt("Music") == 0 ? -80f : 0f);
            _audioTexts[0].alpha = PlayerPrefs.GetInt("Music") == 1 ? 0f : 1f;
        }

        public void ToggleSFX()
        {
            PlayerPrefs.SetInt("SFX", PlayerPrefs.GetInt("SFX") == 1 ? 0 : 1);
            _audioMixerGroup.audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetInt("SFX") == 0 ? -80f : 0f);
            _audioTexts[1].alpha = PlayerPrefs.GetInt("SFX") == 1 ? 0f : 1f;
        }
    }
}
