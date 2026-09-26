using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private Transform _car;
    [SerializeField] private float _speed;
    [SerializeField] private float _currentSpeed;
    [SerializeField] private int _maxHealth;



    [SerializeField] private HealthBar _healthBar;
    private HealthSystem _healthSystem;
    public event Action OnDeath;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _healthSystem = new HealthSystem(_maxHealth);
        _healthSystem.OnHealthChanged += ChangeHealth;
        _healthSystem.OnDeath += () => OnDeath?.Invoke();
    }

    private void Update()
    {
        _car.transform.position = _car.transform.position + Vector3.forward * _currentSpeed * Time.deltaTime;
    }

    private void ChangeHealth()
    {
        _healthBar.ChangeHealthBar((float)_healthSystem.CurrentHealth / (float)_healthSystem.MaxHealth);

    }

    public Vector3 GetCarPosition()
    {
        return _car.transform.position;
    }

    public void DealDamage(int damage)
    {
        _healthSystem.ChangeHealth(damage);
    }
    
    public void ResetHealth()
    {
        _healthSystem.ChangeHealth(+_healthSystem.MaxHealth);
    }

    public void StartCar()
    {
        _currentSpeed = _speed;
    }

    public void StopCar()
    {
        _currentSpeed = 0;
    }

}
