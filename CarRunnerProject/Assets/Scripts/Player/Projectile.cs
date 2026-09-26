using System;
using UnityEngine;

public class Projectile : MonoBehaviour, IDisposable, IPooledObject
{
    private int _damage;
    private Vector3 direction;
    [SerializeField] private float _speed;

    public event Action<IPooledObject> ReturnToPoolAction;

    public GameObject GameObject => gameObject;

    public void Initialize(int damage, Vector3 direction)
    {
        this._damage = damage;
        this.direction = direction;
    }

    private void Update()
    {
        transform.position += direction * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            enemy.ChangeHealth(-_damage);
            Dispose();
        }
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }

}
