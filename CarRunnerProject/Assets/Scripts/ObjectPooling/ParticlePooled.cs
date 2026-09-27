using System;
using UnityEngine;

public class ParticlePooled : MonoBehaviour, IPooledObject
{
    public GameObject GameObject => gameObject;

    public event Action<IPooledObject> ReturnToPoolAction;

    [SerializeField] private ParticleSystem _particleSystem; 

    public void Activate()
    {
        _particleSystem.Play();
    }

    public void ReturnToPool()
    {
        ReturnToPoolAction?.Invoke(this);
    }

    void OnParticleSystemStopped()
    {
        ReturnToPool();
    }
}
