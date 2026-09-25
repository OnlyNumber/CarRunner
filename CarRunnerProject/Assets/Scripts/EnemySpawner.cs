using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;

    [SerializeField] private int CountOfEnemiesPerWave;
    [SerializeField] private float SpawnRadius;

    private IObjectPool<Enemy> enemiesPool;

    private void Awake()
    {
        enemiesPool = new ObjectPool<Enemy>(CreateEnemy, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, maxSize: 200);
    }

    public void SpawnWave(Vector3 position)
    {
        for (int i = 0; i < CountOfEnemiesPerWave; i++)
        {
            var enemy = enemiesPool.Get();
            enemy.transform.position = position + new Vector3(Random.Range(-SpawnRadius, SpawnRadius), Random.Range(-SpawnRadius, SpawnRadius));
        }
    }

    #region Pool
    Enemy CreateEnemy()
    {
        return Instantiate(enemyPrefab);
    }

    void OnTakeFromPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
    }

    void OnReturnedToPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    void OnDestroyPoolObject(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
    #endregion

}
