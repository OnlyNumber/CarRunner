using UnityEngine;
using UnityEngine.Pool;

public class GamePool
{
    private const string Pools_Place = "===POOLS===";

    private IPooledObject _pooledObjectPrefab;
    private ObjectPool<IPooledObject> _objectPool;
    private Transform placeForPooledObject;

    public void Initialize(IPooledObject pooledObject)
    {
        _pooledObjectPrefab = pooledObject;
        _objectPool = new ObjectPool<IPooledObject>(CreatePooledObject, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, maxSize: 200);

        var place = GameObject.Find(Pools_Place).transform;
        placeForPooledObject = new GameObject(_pooledObjectPrefab.GameObject.name + "Pool").transform;
        placeForPooledObject.SetParent(place);
    }

    public IPooledObject Get()
    {
        var pooledObject = _objectPool.Get();
        pooledObject.GameObject.transform.SetParent(placeForPooledObject);
        return pooledObject;
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
