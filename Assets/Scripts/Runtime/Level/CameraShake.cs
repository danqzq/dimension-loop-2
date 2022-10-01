using Cinemachine;
using UnityEngine;

namespace Dan.Level
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _cvCamera;
        
        private bool _isShaking;
        private float _shakeTimer, _shakeIntensity, _shakeAmount;

        private CinemachineBasicMultiChannelPerlin _basicMultiChannelPerlin;

        private static CameraShake _instance;

        private void Awake()
        {
            _instance = this;
            _basicMultiChannelPerlin = _cvCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        
        public static void StartShake(float intensity, float amount)
            => _instance.Shake(intensity, amount);

		private void Update()
        {
            if (!_isShaking) return;
            if (_shakeAmount > 0)
            {
                _shakeAmount -= Time.deltaTime;
                return;
            }
            
            var t = 1 - _shakeAmount / _shakeTimer;
            _basicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(_shakeIntensity, 0, t);
            
            if (_basicMultiChannelPerlin.m_AmplitudeGain <= 0) 
                _isShaking = false;
        }

        private void Shake(float intensity, float amount)
        {
            _basicMultiChannelPerlin.m_AmplitudeGain = intensity;
            _shakeIntensity = intensity;
            _shakeAmount = amount;
            _shakeTimer = amount;
            _isShaking = true;
        }
    }
}