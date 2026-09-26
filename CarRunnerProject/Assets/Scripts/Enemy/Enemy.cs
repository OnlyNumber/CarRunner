using System;
using System.Collections;
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

    private Coroutine _currentState;
    private Transform _testTarget;
    private bool _isMovingToTarget = false;
    private UnitAnimator.StateAnimation _lastAnimation;

    public GameObject GameObject => gameObject;

    public event Action<IPooledObject> ReturnToPoolAction;

    public void Initialize(Transform target)
    {
        _healthSystem = new HealthSystem(_health);

        _healthSystem.OnHealthChanged += ActivateHealthBar;
        _healthSystem.OnHealthChanged += ChangeHealth;

        _healthSystem.OnDeath += Death;

        _testTarget = target;

        _currentState = StartCoroutine(Wandering());

    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _testTarget.position) < _reactionRadius && !_isMovingToTarget)
        {
            StopCoroutine(_currentState);
            _currentState = StartCoroutine(MoveToTarget());
            _isMovingToTarget = true;
        }

    }


    private IEnumerator MoveToTarget()
    {
        do
        {
            _unitAnimator.SetAnimation(UnitAnimator.StateAnimation.Move);
            _unitMovement.MoveToTarget(_testTarget.position, _speed, _rotationSpeed);
            yield return null;

        } while (true);
    }

    private IEnumerator Wandering()
    {
        do
        {
            _unitAnimator.SetAnimation(UnitAnimator.StateAnimation.Idle);
            yield return new WaitForSeconds(UnityEngine.Random.Range(_wanderWaiting.x, _wanderWaiting.y));

            float x = UnityEngine.Random.Range(-_wanderRadius, _wanderRadius);
            float y = UnityEngine.Random.Range(-_wanderRadius, _wanderRadius);

            Vector3 wanderPosition = transform.position + new Vector3(x, 0, y);

            _unitAnimator.SetAnimation(UnitAnimator.StateAnimation.Move);

            do
            {
                _unitMovement.MoveToTarget(wanderPosition, _speed, _rotationSpeed);
                yield return null;

            } while (Vector3.Distance(transform.position, wanderPosition) > 0.5f);


        } while (true);
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

        _lastAnimation = _unitAnimator.CurrentAnimaion;
        StartCoroutine(BackToAnimation());
        _unitAnimator.SetAnimation(UnitAnimator.StateAnimation.Hitted);
    }

    private IEnumerator BackToAnimation()
    {
        yield return new WaitForSeconds(_unitAnimator.GetCurrentAnimatorClipInfo().clip.length);
        _unitAnimator.SetAnimation(_lastAnimation);

    }

    public void ChangeHealth(int damage)
    {
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

    [ContextMenu("ReturnToPool")]
    public void ReturnToPool()
    {
        Dispose();
        ReturnToPoolAction?.Invoke(this);
    }

    public void Dispose()
    {
        _healthSystem.OnHealthChanged -= ActivateHealthBar;
        _healthSystem.OnHealthChanged -= ChangeHealth;
        _healthSystem.OnDeath -= Death;


        _healthSystem = null;

        if (_currentState != null)
            StopCoroutine(_currentState);

        _currentState = null;
    }
}
