using UnityEngine;
using UnityEngine.Pool;

public class GamePool
{
    private IPooledObject _pooledObjectPrefab;
    private ObjectPool<IPooledObject> _objectPool;

    public void Initialize(IPooledObject pooledObject)
    {
        _pooledObjectPrefab = pooledObject;
        _objectPool = new ObjectPool<IPooledObject>(CreatePooledObject, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, maxSize: 200);
        
    }

    public IPooledObject Get()
    {
        return _objectPool.Get();
    }

    public void Release(IPooledObject pooledObject)
    {
        _objectPool.Release(pooledObject);
    }

    private IPooledObject CreatePooledObject()
    {
        var objectPool = GameObject.Instantiate(_pooledObjectPrefab.GameObject).GetComponent<IPooledObject>();
        objectPool.ReturnToPoolAction += _objectPool.Release;
        return objectPool;
    }

    private void OnTakeFromPool(IPooledObject pooledObject)
    {
        pooledObject.GameObject.SetActive(true);
    }

    private void OnReturnedToPool(IPooledObject pooledObject)
    {
        pooledObject.GameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(IPooledObject pooledObject)
    {
        GameObject.Destroy(pooledObject.GameObject);
    }
}
