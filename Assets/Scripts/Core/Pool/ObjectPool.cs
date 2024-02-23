using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//where T : RecycleObject  // T 타입은 RecycleObject이거나 RecycleObject를 상속받은 클래스만 가능하다.
public class ObjectPool<T> : MonoBehaviour where T : RecycleObject
{
    /// <summary>
    /// 풀에서 관리할 오브젝트의 프리팹
    /// </summary>
    public GameObject originalPrefab;

    /// <summary>
    /// 풀의 크기. 처음에 생성하는 오브젝트의 개수
    /// </summary>
    public int poolSize = 4;

    /// <summary>
    /// T타입으로 지정된 오브젝트의 배열. 생성된 모든 오브젝트가 있는 배열.
    /// </summary>
    T[] pool;

    /// <summary>
    /// 현재 사용가능한(비활성화되어있는) 오브젝트들을 관리하는 큐
    /// </summary>
    Queue<T> readyQueue;

    public void Initialize()
    {
        if( pool == null )  
        {
            pool = new T[poolSize];             
            readyQueue = new Queue<T>(poolSize);   

            GenerateObjects(0, poolSize, pool);
        }
        else
        {
            foreach( T obj in pool ) 
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    public T GetObject(Vector3? position = null, Vector3? eulerAngle = null)
    {
        if (readyQueue.Count > 0)        
        {
            T comp = readyQueue.Dequeue(); 
            comp.transform.position = position.GetValueOrDefault(); 
            comp.transform.rotation = Quaternion.Euler(eulerAngle.GetValueOrDefault());  
            comp.gameObject.SetActive(true);
            OnGetObject(comp);           
            return comp;             
        }
        else
        {
            ExpandPool();                        
            return GetObject(position, eulerAngle);
        }
    }

    protected virtual void OnGetObject(T component)
    {
    }

    void ExpandPool()
    {

        Debug.LogWarning($"{gameObject.name} 풀 사이즈 증가. {poolSize} -> {poolSize * 2}");

        int newSize = poolSize * 2;         
        T[] newPool = new T[newSize];       
        for(int i = 0; i<poolSize; i++)     
        {
            newPool[i] = pool[i];
        }

        GenerateObjects(poolSize, newSize, newPool);    
        
        pool = newPool;        
        poolSize = newSize;    
    }

    void GenerateObjects(int start, int end, T[] results)
    {
        for (int i = start; i < end; i++)
        {
            GameObject obj = Instantiate(originalPrefab, transform);
            obj.name = $"{originalPrefab.name}_{i}";   

            T comp = obj.GetComponent<T>();
            comp.onDisable += () => readyQueue.Enqueue(comp);  

            results[i] = comp;      
            obj.SetActive(false);   
        }
    }
}
