using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Projectile : MonoBehaviour, IDisposable, IPooledObject
{
    [SerializeField] private TrailRenderer _trailRenderer;

    private int _damage;
    private Vector3 _direction;
    [SerializeField] private float _speed;

    [SerializeField] private float _projectileLifetime;
    private float _currentLifetime = 0;


    public event Action<IPooledObject> ReturnToPoolAction;
    public GameObject GameObject => gameObject;

    private CancellationTokenSource _cts;


    public void Initialize(int damage, Vector3 direction)
    {
        this._damage = damage;
        this._direction = direction;
        _currentLifetime = 0;

        _trailRenderer.Clear();
        _trailRenderer.emitting = true;

        _cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        
        FlyingAsync(_cts.Token).Forget();
    }

    private async UniTaskVoid FlyingAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            float time = Time.deltaTime;

            transform.position += _direction * _speed * time;
            _currentLifetime += time;

            if (_currentLifetime > _projectileLifetime)
            {
                ReturnToPool();
                return;
            }

            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            enemy.ChangeHealth(-_damage);
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
        if (!gameObject.activeInHierarchy)
            return;

        Dispose();
        ReturnToPoolAction?.Invoke(this);
    }


    public void Dispose()
    {
        _trailRenderer.emitting = false;
        _direction = Vector3.zero;

        _cts?.Cancel();
        _cts?.Dispose();
    }

}
