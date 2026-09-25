using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;

    private IObjectPool<Enemy> enemiesPool;

    private void Awake()
    {
        enemiesPool = new ObjectPool<Enemy>(CreateEnemy, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, maxSize: 200);
    }

    public void SpawnWave(Vector3 position, float spawnRadius, int countOfEnemiesPerWave)
    {
        for (int i = 0; i < countOfEnemiesPerWave; i++)
        {
            var enemy = enemiesPool.Get();
            enemy.transform.position = position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));
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
