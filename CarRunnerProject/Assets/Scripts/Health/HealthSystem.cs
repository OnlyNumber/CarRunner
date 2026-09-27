using System;
using UnityEngine;

public class HealthSystem
{
    private int _currentHealth;
    public int CurrentHealth
    {
        set
        {
            _currentHealth = value;

            if (_currentHealth > MaxHealth)
                _currentHealth = MaxHealth;

            if (_currentHealth < 0)
                _currentHealth = 0;

            OnHealthChanged?.Invoke();
        }
        
        get => _currentHealth;
    }

    public int MaxHealth
    {
        private set;
        get;
    }

    public event Action OnHealthChanged;
    public event Action OnDeath;

    public HealthSystem(int MaxHealth = 10)
    {
        this.MaxHealth = MaxHealth;
        CurrentHealth = MaxHealth;

        OnHealthChanged += Death;

    }

    public void ChangeHealth(int changeHalth)
    {
        CurrentHealth += changeHalth;
    }

    private void Death()
    {
        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

}
