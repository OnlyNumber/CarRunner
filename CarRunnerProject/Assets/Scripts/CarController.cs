using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private Transform _car;
    [SerializeField] private float _speed;

    [SerializeField] private HealthBar _healthBar;
    private HealthSystem _healthSystem;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _healthSystem = new HealthSystem(100);

        _healthSystem.OnHealthChanged += ChangeHealth;

    }

    private void Update()
    {
        _car.transform.position = _car.transform.position + Vector3.forward * _speed * Time.deltaTime;
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
    

}
