using System;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour, IDisposable, IPooledObject
{
    private int _damage;
    private Vector3 direction;
    [SerializeField] private float _speed;

    [SerializeField] private float _projectileLifetime;
    private float _currentLifetime = 0;


    public event Action<IPooledObject> ReturnToPoolAction;
    public GameObject GameObject => gameObject;

    private Coroutine currentFlyingCoroutine;

    public void Initialize(int damage, Vector3 direction)
    {
        this._damage = damage;
        this.direction = direction;
        _currentLifetime = 0;

        currentFlyingCoroutine = StartCoroutine(Flying());

    }

    private IEnumerator Flying()
    {
        do
        {
            yield return null;
            float time = Time.deltaTime;

            transform.position += direction * _speed * time;
            _currentLifetime += time;

            if (_currentLifetime > _projectileLifetime)
                ReturnToPool();


        } while (true);
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
        Dispose();
        ReturnToPoolAction?.Invoke(this);
    }


    public void Dispose()
    {
        direction = Vector3.zero;
        if (currentFlyingCoroutine != null)
            StopCoroutine(currentFlyingCoroutine);

        currentFlyingCoroutine = null;
        //Destroy(gameObject);
    }

}
