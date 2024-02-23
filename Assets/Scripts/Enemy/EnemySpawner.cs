using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : RecycleObject
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 1.0f;

    private Coroutine spawnCoroutine;

    private Vector3 mapMin = new Vector3(-40, 0, -40);
    private Vector3 mapMax = new Vector3(40, 0, 40);

    protected override void OnEnable()
    {
        base.OnEnable();
        spawnCoroutine = StartCoroutine(SpawnEnemyCoroutine());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(mapMin.x, mapMax.x),
                mapMin.y,
                Random.Range(mapMin.z, mapMax.z)
            );

            Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
        }
    }
}
