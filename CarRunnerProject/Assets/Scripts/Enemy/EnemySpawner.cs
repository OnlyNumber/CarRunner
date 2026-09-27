using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region EnemiesPool
    [SerializeField] private Enemy _enemyPrefab;
    private GamePool _enemiesPool = new();
    #endregion

    #region ParticlesPool
    [SerializeField] private ParticlePooled _particlesPrefab;
    private GamePool _particlesPool = new();
    #endregion


    private HashSet<IPooledObject> _allEnemies = new();

    private Transform _target;

    private void Awake()
    {
        _enemiesPool.Initialize(_enemyPrefab);
        _particlesPool.Initialize(_particlesPrefab);
    }

    public void SetTargetForEnemies(Transform target)
    {
        _target = target;
    }

    public async UniTaskVoid SpawnWaveAsync(Vector3 position, float spawnRadius, int countOfEnemiesPerWave, CancellationToken ct = default)
    {
        var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(ct, this.GetCancellationTokenOnDestroy()).Token;

        try
        {
            for (int i = 0; i < countOfEnemiesPerWave; i++)
            {
                var enemy = _enemiesPool.Get();

                if (enemy is Enemy enemyComponent)
                {
                    enemyComponent.Initialize(_target, _particlesPool);
                }

                Vector3 randomOffset = new Vector3(
                    UnityEngine.Random.Range(-spawnRadius, spawnRadius),
                    0f,
                    UnityEngine.Random.Range(-spawnRadius, spawnRadius)
                );

                enemy.GameObject.transform.position = position + randomOffset;
                _allEnemies.Add(enemy);

                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: linkedToken);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Спавн хвилі скасовано.");
        }
    }

    public void ReturnAllEnemies()
    {
        foreach (var item in _allEnemies)
        {
            if (item.GameObject.activeInHierarchy)
                item.ReturnToPool();
        }
    }
}
