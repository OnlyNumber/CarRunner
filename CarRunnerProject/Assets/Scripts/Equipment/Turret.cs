using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private Joystick _joystick;
    [SerializeField] private Transform _turret;
    [SerializeField] private Transform _firePoint;

    [SerializeField] private Projectile _projectile;
    private GamePool _projectilePool = new();

    [SerializeField] private float _timeBetweenShoots;
    [SerializeField] private int _damage;

    private float _currentAttackTime;


    private void Awake()
    {
        _projectilePool.Initialize(_projectile);
    }

    void Update()
    {
        float x = _joystick.Horizontal;
        float y = _joystick.Vertical;

        if (x != 0.0f || y != 0.0f)
        {
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

            _turret.rotation = Quaternion.Euler(0, -(angle - 90), 0);

            if (_currentAttackTime >= _timeBetweenShoots)
            {
                Shoot(new Vector3(x, 0, y).normalized);
                _currentAttackTime = 0;
            }
        }

        _currentAttackTime += Time.deltaTime;
    }

    public void Shoot(Vector3 direction)
    {
        var projectile = _projectilePool.Get();

        projectile.GameObject.transform.position = _firePoint.position;
        (projectile as Projectile).Initialize(_damage, direction);
    }

}
