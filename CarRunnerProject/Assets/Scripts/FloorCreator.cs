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
    [SerializeField] private CarMover _carMover;
    [SerializeField] private EnemySpawner _enemySpawner;


    private List<Transform> _platformsPool = new();



    private void Start()
    {
        CreatePlatforms();
        _enemySpawner.SpawnWave(_platformsPool[1].position);
    }

    private void CreatePlatforms()
    {
        for (int i = 0; i < countPlatformsForPool; i++)
        {
            var platform = Instantiate(platformPrefab, ParentForPlatforms);
            platform.position = Vector3.zero;
            _platformsPool.Add(platform);
            SetPlatformPosition(platform);
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
        Debug.Log("SetPlatforms");
        Transform currentLastPlatform = _platformsPool[0];

        foreach (var platformFromPool in _platformsPool)
        {
            if (currentLastPlatform.position.z < platformFromPool.position.z)
                currentLastPlatform = platformFromPool;
        }

        if (currentLastPlatform != platform)
            platform.position = currentLastPlatform.position + Vector3.forward * distanceBetweenPlatforms;
    }
}
