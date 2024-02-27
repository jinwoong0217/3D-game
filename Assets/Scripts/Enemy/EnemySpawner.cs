using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : RecycleObject
{
    /// <summary>
    /// 적 스폰에 사용할 코루틴 
    /// </summary>
    Coroutine spawnCoroutine;  

    /// <summary>
    /// 적의 프리팹
    /// </summary>
    public GameObject enemyPrefab;
    [Header("스폰포인트 지정")]
    public Transform spawnPoint;
    [Header("스폰 간격")]
    public float spawnInterval = 1.0f;

    /// <summary>
    /// 일정 구역안에 랜덤한 위치에서 스폰되게 좌표를 지정해둠
    /// </summary>
    Vector3 mapMin = new Vector3(-40, 0, -40);
    Vector3 mapMax = new Vector3(40, 0, 40);

    /// <summary>
    /// 스크립트가 활성화 될 때 코루틴을 시작함
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        spawnCoroutine = StartCoroutine(SpawnEnemyCoroutine());
    }

    /// <summary>
    /// 스크립트가 비활성화 될 때 코루틴을 정지
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    /// <summary>
    /// 적 스폰 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemyCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);  // 스폰 간격만큼 대기 
            SpawnEnemy();  // 스폰 
        }
    }

    /// <summary>
    /// 랜덤한 위치에 적을 스폰하는 함수  
    /// </summary>
    private void SpawnEnemy()
    {
        if (enemyPrefab != null)  // 적 프리팹이 되있으면 
        { 
            Vector3 randomPosition = new Vector3(  // 랜덤한 위치에 적 생성  
                Random.Range(mapMin.x, mapMax.x),
                mapMin.y,
                Random.Range(mapMin.z, mapMax.z)
            );

            Instantiate(enemyPrefab, randomPosition, Quaternion.identity);  //적 프리팹 생성 
        }
    }
}
