using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dan.Level
{
    public class Boss : MonoBehaviour
    {
        private static Boss _instance;

        [SerializeField] private GameObject _dan;
        
        [SerializeField] private BossHitPoint _leftEye, _rightEye;
        
        [SerializeField] private Image _healthBar;
        
        private int _health;
        private float _targetHealthValue;

        private void Awake()
        {
            _instance = this;
            this.DoAfter(9f, () =>
            {
                _healthBar.transform.parent.GetComponent<CanvasGroup>().DOFade(1f, 1f);
                ModifierManager.HandModifier();
                this.DoAfter(6f, SummonEyes);
            });
        }

        private void SummonEyes()
        {
            _dan.SetActive(true);
            GetComponent<Animator>().enabled = false;
            this.DoAfter(1f, () =>
            {
                _leftEye.gameObject.SetActive(true);
                _rightEye.gameObject.SetActive(true);
                _leftEye.GetComponent<SpriteRenderer>().color = Color.white;
                _rightEye.GetComponent<SpriteRenderer>().color = Color.white;
                _leftEye.ResetPos();
                _rightEye.ResetPos();
                _dan.SetActive(false);
                this.DoAfter(3f, GoBack);
            });
        }
        
        private void GoBack()
        {
            _leftEye.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f);
            _rightEye.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).OnComplete(() =>
            {
                _leftEye.gameObject.SetActive(false);
                _rightEye.gameObject.SetActive(false);
                this.DoAfter(3f, SummonEyes);
            });
        }

        private void Start()
        {
            _health = 25;
        }

        public static void TakeDamage()
        {
            _instance.TakeDamage(1);
        }
        
        public void TakeDamage(int damage)
        {
            _health -= damage;
            _healthBar.fillAmount = _targetHealthValue;

            if (_health <= 0)
            {
                GameManager.Obj.Win();
            }
        }

        private void Update()
        {
            _targetHealthValue = Mathf.Lerp(_targetHealthValue, _health / 25f, Time.deltaTime * 5);
        }
    }
}
