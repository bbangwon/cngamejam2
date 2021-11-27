using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    static Spawner instance;
    public static Spawner Instance { get => instance; }

    public float spawnRange = 5f;
    
    public int KillCount // 10마리당 스폰 1초씩 감소
    {
        get => KillCount;
        set
        {
            KillCount += value;
            CurSpawnCount--;
            MakeHard();
        }
    }

    public int MaxSpawnCount = 1; // 10마리당 1씩 증가, 최대 10마리
    public int CurSpawnCount = 0;
    public int spawnSeconds = 10; // 최소 1초
    public int VillainMaxHP = 1; // 50마리당 1씩 증가, 최대 10

    public int Level = 1; // 레벨에 따른 빌런 타입 증가

    [SerializeField] Transform TrainParent;
    [SerializeField] GameObject[] SpawnPrefabs;

    void MakeHard()
    {
        int count = 1 + KillCount / 10;
    }

    public GameObject SpawnOrNull(Vector2 playerPos)
    {
        if (CurSpawnCount >= MaxSpawnCount)
            return null;

        int index = Random.Range(0, Level);

        float spawn_x = Random.Range(-spawnRange + playerPos.x, spawnRange + playerPos.x);

        Villain.ESpawnType spawnType = (Villain.ESpawnType)Random.Range(0, 2);
        float spawn_y = spawnType == Villain.ESpawnType.Downstairs ? 0f : 10f;

        Vector2 spawnPos = new Vector2(spawn_x, spawn_y);

        GameObject go = Instantiate(SpawnPrefabs[index], spawnPos, Quaternion.identity);
        CurSpawnCount++;
        return go;
    }
}
