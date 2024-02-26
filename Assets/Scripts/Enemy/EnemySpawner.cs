using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : RecycleObject
{
    Coroutine spawnCoroutine;

    public GameObject enemyPrefab;
    [Header("스폰포인트 지정")]
    public Transform spawnPoint;
    [Header("스폰 시간")]
    public float spawnInterval = 1.0f;

    /// <summary>
    /// 일정 구역안에 랜덤한 위치에서 스폰되게 좌표를 지정해둠
    /// </summary>
    Vector3 mapMin = new Vector3(-40, 0, -40);
    Vector3 mapMax = new Vector3(40, 0, 40);

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
