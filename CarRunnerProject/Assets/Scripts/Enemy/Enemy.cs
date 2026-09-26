using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Enemy : MonoBehaviour, IDisposable, IPooledObject
{

    #region  Stats
    [SerializeField] private float _wanderRadius;
    [SerializeField] private Vector2 _wanderWaiting;

    [SerializeField] private float _reactionRadius;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _rotationSpeed = 60;

    [SerializeField] private int _damage;
    [SerializeField] private int _health;

    #endregion

    #region  Components
    [SerializeField] private UnitAnimator _unitAnimator;
    [SerializeField] private UnitMovement _unitMovement;
    [SerializeField] private HealthBar _healthBar;
    #endregion

    private HealthSystem _healthSystem;

    private Transform _testTarget;
    private bool _isMovingToTarget = false;
    private UnitAnimator.StateAnimation _lastAnimation;

    public GameObject GameObject => gameObject;

    public event Action<IPooledObject> ReturnToPoolAction;

    private CancellationTokenSource _ctCurrentState;

    private CancellationTokenSource _ctWaitAfterHit;

    public void Initialize(Transform target)
    {
        _healthSystem = new HealthSystem(_health);

        _healthSystem.OnHealthChanged += ActivateHealthBar;
        _healthSystem.OnHealthChanged += ChangeHealth;

        _healthSystem.OnDeath += Death;

        _testTarget = target;

        _ctCurrentState = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        WanderingAsync(_ctCurrentState.Token).Forget();

    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _testTarget.position) < _reactionRadius && !_isMovingToTarget)
        {
            _ctCurrentState?.Cancel();
            _ctCurrentState?.Dispose();
            _ctCurrentState = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

            MoveToTargetAsync(_ctCurrentState.Token).Forget();
            _isMovingToTarget = true;
        }

    }

    private void ActivateHealthBar()
    {
        if (_healthSystem.CurrentHealth <= 0)
            return;

        _healthBar.Activate();

        _healthSystem.OnHealthChanged -= ActivateHealthBar;
    }

    private void ChangeHealth()
    {
        if (_healthBar == null || _healthSystem == null)
            return;

        _healthBar.ChangeHealthBar((float)_healthSystem.CurrentHealth / (float)_healthSystem.MaxHealth);

        if (_unitAnimator.CurrentAnimaion != UnitAnimator.StateAnimation.Hitted)
            _lastAnimation = _unitAnimator.CurrentAnimaion;


        _ctWaitAfterHit?.Cancel();
        _ctWaitAfterHit?.Dispose();
        _ctWaitAfterHit = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

        BackToAnimationAsync(_ctWaitAfterHit.Token).Forget();

        _unitAnimator.SetAnimation(UnitAnimator.StateAnimation.Hitted);
    }


    public void ChangeHealth(int damage)
    {
        if (_healthSystem == null)
            return;

        _healthSystem.ChangeHealth(damage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var carController = other.GetComponentInParent<CarController>();
            carController.DealDamage(-_damage);
            Death();
        }
    }

    private void Death()
    {
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        Dispose();
        ReturnToPoolAction?.Invoke(this);
    }

    public void Dispose()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnHealthChanged -= ActivateHealthBar;
            _healthSystem.OnHealthChanged -= ChangeHealth;
            _healthSystem.OnDeath -= Death;
        }

        _healthSystem = null;

        _ctCurrentState?.Cancel();
        _ctCurrentState?.Dispose();
        _ctCurrentState = null;

        _ctWaitAfterHit?.Cancel();
        _ctWaitAfterHit?.Dispose();
        _ctWaitAfterHit = null;

        _isMovingToTarget = false;

    }


    private async UniTaskVoid WanderingAsync(CancellationToken ct)
    {

        while (!ct.IsCancellationRequested)
        {
            _unitAnimator.SetAnimation(UnitAnimator.StateAnimation.Idle);
            await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(_wanderWaiting.x, _wanderWaiting.y)), cancellationToken: ct);

            float x = UnityEngine.Random.Range(-_wanderRadius, _wanderRadius);
            float y = UnityEngine.Random.Range(-_wanderRadius, _wanderRadius);

            Vector3 wanderPosition = transform.position + new Vector3(x, 0, y);

            _unitAnimator.SetAnimation(UnitAnimator.StateAnimation.Move);

            do
            {
                _unitMovement.MoveToTarget(wanderPosition, _speed, _rotationSpeed);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);


            } while (Vector3.Distance(transform.position, wanderPosition) > 0.5f);


        }
    }

    private async UniTaskVoid MoveToTargetAsync(CancellationToken ct)
    {
        _unitAnimator.SetAnimation(UnitAnimator.StateAnimation.Move);

        while (!ct.IsCancellationRequested)
        {
            _unitMovement.MoveToTarget(_testTarget.position, _speed, _rotationSpeed);
            await UniTask.Yield(PlayerLoopTiming.Update, ct);


        }
    }

    private async UniTaskVoid BackToAnimationAsync(CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_unitAnimator.GetCurrentAnimatorClipInfo().clip.length), cancellationToken: ct);

        _unitAnimator.SetAnimation(_lastAnimation);

    }
}
