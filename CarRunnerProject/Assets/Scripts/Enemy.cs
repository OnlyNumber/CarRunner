using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IDisposable
{
    [SerializeField] private Collider _enemyCollider;
    [SerializeField] private GameObject _enemyModel;

    [SerializeField] private float _reactionRadius;
    [SerializeField] private float _speed;

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

    }

    [ContextMenu("Dealdamage")]
    private void DealDamage()
    {
        _healthSystem.ChangeHealth(-5);
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
        _healthBar.ChangeHealthBar((float)_healthSystem.CurrentHealth / (float)_healthSystem.MaxHealth);

    }

    public void Dispose()
    {
        _healthSystem = null;

        _healthSystem.OnHealthChanged -= ActivateHealthBar;
        _healthSystem.OnHealthChanged -= ChangeHealth;
    }
}
