using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
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
    private CancellationTokenSource _ctSlowHealth;
    private UniTask _slowHealthUniTask;
    private CancellationTokenSource _ctWatchCamera;

    public void Activate()
    {
        gameObject.SetActive(true);

        _ctWatchCamera?.Cancel();
        _ctWatchCamera?.Dispose();
        _ctWatchCamera = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

        WatchInCameraAsync(_ctWatchCamera.Token).Forget();
    }

    public void Hide()
    {
        #region Coroutine
        _ctSlowHealth?.Cancel();
        _ctSlowHealth?.Dispose();

        _ctWatchCamera?.Cancel();
        _ctWatchCamera?.Dispose();


        #endregion
        _currentPercent = 1;

        gameObject.SetActive(false);
    }

    public void ChangeHealthBar(float percentOfHealth)
    {
        _settedPercent = percentOfHealth;
        _health.value = _settedPercent;

        if (_slowHealthUniTask.Status == UniTaskStatus.Pending)
            return;

        _ctSlowHealth?.Cancel();
        _ctSlowHealth?.Dispose();
        _ctSlowHealth = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

        _slowHealthUniTask = ChangeHealthAsync(_ctSlowHealth.Token);
    }

    public async UniTask ChangeHealthAsync(CancellationToken ct)
    {
        do
        {
            _currentPercent -= Time.deltaTime * (_speedSlowerHealth / 100);
            _slowerHealth.value = _currentPercent;

            await UniTask.Yield(PlayerLoopTiming.Update, ct);

        } while (_currentPercent >= _settedPercent);
    }

    public async UniTaskVoid WatchInCameraAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            transform.rotation = Quaternion.Euler(new Vector3(45, 0, 0));
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
    }

    private IEnumerator ChangeHealth()
    {
        do
        {
            _currentPercent -= Time.deltaTime * (_speedSlowerHealth / 100);
            _slowerHealth.value = _currentPercent;

            yield return null;

        } while (_currentPercent >= _settedPercent);

        //_ctSlowHealth = null;
    }

}
