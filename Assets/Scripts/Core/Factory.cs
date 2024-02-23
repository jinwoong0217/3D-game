using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolObjectType
{
    EnemySpawner = 0,
}

public class Factory : Singleton<Factory>
{
    EnemySpawnerPool enemySpawnerPool;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        enemySpawnerPool = GetComponentInChildren<EnemySpawnerPool>();
        if(enemySpawnerPool != null ) enemySpawnerPool.Initialize();
    }

    public GameObject GetObject(PoolObjectType type, Vector3? position = null, Vector3? euler = null)
    {
        GameObject result = null;
        switch (type)
        {
            case PoolObjectType.EnemySpawner:
                result = enemySpawnerPool.GetObject(position, euler).gameObject;
                break;
        }

        return result;
    }

    public EnemySpawner GetEnemySpawner()
    {
        return enemySpawnerPool.GetObject();        
    }

    public EnemySpawner GetEnemySpawner(Vector3 position, float angle = 0.0f)
    {
        return enemySpawnerPool.GetObject(position, angle * Vector3.forward);
    }
}