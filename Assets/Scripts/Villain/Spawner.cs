using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    static Spawner instance;
    public static Spawner Instance { get => instance; }

    public float spawnRange = 5f;
    
    public int KillCount // 10������ ���� 1�ʾ� ����
    {
        get => KillCount;
        set
        {
            KillCount += value;
            CurSpawnCount--;
            MakeHard();
        }
    }

    public int MaxSpawnCount = 1; // 10������ 1�� ����, �ִ� 10����
    public int CurSpawnCount = 0;
    public int spawnSeconds = 10; // �ּ� 1��
    public int VillainMaxHP = 1; // 50������ 1�� ����, �ִ� 10

    public int Level = 1; // ������ ���� ���� Ÿ�� ����

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
