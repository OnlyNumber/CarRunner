using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class FloorCreator : MonoBehaviour
{
    [SerializeField] private Transform platformPrefab;
    [SerializeField] private int countPlatformsForPool;
    [SerializeField] private float distanceBetweenPlatforms;

    [SerializeField] private Transform ParentForPlatforms;

    //Add Zenject
    [SerializeField] private CarController _carMover;
    [SerializeField] private EnemySpawner _enemySpawner;


    private List<Transform> _platformsPool = new();



    private void Start()
    {
        CreatePlatforms();
        MovePlatformsToStart();
    }

    private void CreatePlatforms()
    {
        for (int i = 0; i < countPlatformsForPool; i++)
        {
            var platform = Instantiate(platformPrefab, ParentForPlatforms);
            platform.position = Vector3.zero;
            _platformsPool.Add(platform);
        }
    }


    private void Update()
    {
        foreach (var platform in _platformsPool)
        {
            if (_carMover.GetCarPosition().z > platform.transform.position.z
            && Vector3.Distance(_carMover.GetCarPosition(), platform.transform.position) >= distanceBetweenPlatforms)
                SetPlatformPosition(platform);
        }


    }

    private void SetPlatformPosition(Transform platform)
    {
        Transform currentLastPlatform = _platformsPool[0];

        foreach (var platformFromPool in _platformsPool)
            if (currentLastPlatform.position.z < platformFromPool.position.z)
                currentLastPlatform = platformFromPool;
        

        if (currentLastPlatform != platform)
        {
            platform.position = currentLastPlatform.position + Vector3.forward * distanceBetweenPlatforms;
            _enemySpawner.SpawnWave(platform.position, 4, 10);
        }
    }

    public void MovePlatformsToStart()
    {
        foreach (var platform in _platformsPool)
            platform.position = Vector3.zero;

        foreach (var platform in _platformsPool)
            SetPlatformPosition(platform);
    }
}
