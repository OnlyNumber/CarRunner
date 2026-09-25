using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IDisposable
{
    //[SerializeField] private Collider _enemyCollider;
    [SerializeField] private GameObject _enemyModel;

    [SerializeField] private float _reactionRadius;
    [SerializeField] private float _speed;

    [SerializeField] private int damage;

    [SerializeField] private HealthBar _healthBar;
    private HealthSystem _healthSystem;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _healthSystem = new HealthSystem(15);

        _healthSystem.OnHealthChanged += ActivateHealthBar;
        _healthSystem.OnHealthChanged += ChangeHealth;

        _healthSystem.OnDeath += Death;

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
            carController.DealDamage(-damage);
            Death();
        }
    }

    private void Death()
    {
        Dispose();
        //Change to return pool 
        Destroy(gameObject);
    }

    public void Dispose()
    {
        _healthSystem.OnHealthChanged -= ActivateHealthBar;
        _healthSystem.OnHealthChanged -= ChangeHealth;

        _healthSystem = null;
    }
}
