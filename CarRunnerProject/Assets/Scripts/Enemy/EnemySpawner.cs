using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;

    private ObjectPool<IPooledObject> _enemiesPool;

    private HashSet<IPooledObject> _allEnemies = new();

    private Transform _target;

    private void Awake()
    {
        _enemiesPool = new ObjectPool<IPooledObject>(CreateEnemy, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, maxSize: 200);
    }

    public void SetTargetForEnemies(Transform target)
    {
        _target = target;
    }

    public void SpawnWave(Vector3 position, float spawnRadius, int countOfEnemiesPerWave)
    {
        for (int i = 0; i < countOfEnemiesPerWave; i++)
        {
            var enemy = _enemiesPool.Get();
            (enemy as Enemy).Initialize(_target);
            enemy.GameObject.transform.position = position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));
            _allEnemies.Add(enemy);
        }
    }

    #region Pool
    IPooledObject CreateEnemy()
    {
        var enemy = Instantiate(enemyPrefab);
        enemy.ReturnToPoolAction += _enemiesPool.Release;
        return enemy;
    }

    void OnTakeFromPool(IPooledObject enemy)
    {
        enemy.GameObject.SetActive(true);
    }

    void OnReturnedToPool(IPooledObject enemy)
    {
        enemy.GameObject.SetActive(false);
    }

    void OnDestroyPoolObject(IPooledObject enemy)
    {
        Destroy(enemy.GameObject);
    }
    #endregion

    public void ReturnAllEnemies()
    {
        foreach (var item in _allEnemies)
        {
            if (item.GameObject.activeInHierarchy)
                item.ReturnToPool();
        }
    }
}
