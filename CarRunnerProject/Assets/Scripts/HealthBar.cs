using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slowerHealth;
    [SerializeField] private Slider _health;
    [Tooltip("Percents per second")][SerializeField] private float _speedSlowerHealth = 15;

    private float _currentPercent = 1;
    private float _settedPercent;
    private Coroutine _slowHealthCoroutine;
    private Coroutine _watchInCameraCoroutine;


    public void Activate()
    {
        gameObject.SetActive(true);
        _watchInCameraCoroutine = StartCoroutine(WatchInCamera());
    }

    public void Hide()
    {
        #region Coroutine
        if (_slowHealthCoroutine != null)
            StopCoroutine(_slowHealthCoroutine);

        _slowHealthCoroutine = null;

        if (_watchInCameraCoroutine != null)
            StopCoroutine(_watchInCameraCoroutine);

        _watchInCameraCoroutine = null;
        #endregion
        _currentPercent = 1;

        gameObject.SetActive(false);
    }

    public void ChangeHealthBar(float percentOfHealth)
    {
        _settedPercent = percentOfHealth;
        _health.value = _settedPercent;

        if (_slowHealthCoroutine == null)
            _slowHealthCoroutine = StartCoroutine(ChangeHealth());
    }

    private IEnumerator ChangeHealth()
    {
        do
        {
            _currentPercent -= Time.deltaTime * (_speedSlowerHealth / 100);
            _slowerHealth.value = _currentPercent;

            yield return null;

        } while (_currentPercent >= _settedPercent);
        
        _slowHealthCoroutine = null;
    }

    private IEnumerator WatchInCamera()
    {
        do
        {
            transform.rotation = Quaternion.Euler(new Vector3(45, 0, 0));
            yield return null;

        } while (true);

    }



}
