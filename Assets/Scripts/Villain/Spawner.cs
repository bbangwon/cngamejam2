using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cngamejam;

public class Spawner : MonoBehaviour
{
    static Spawner instance;
    public static Spawner Instance { get => instance; }

    public float spawnRange = 20f;

    int killCount;
    public int KillCount // 10마리당 스폰 1초씩 감소
    {
        get => killCount;
        set
        {
            killCount += value;
            CurSpawnCount--;
            MakeHard();
        }
    }

    public int MaxSpawnCount = 1; // 10마리당 1씩 증가, 최대 10마리
    public int CurSpawnCount = 0;
    public int SpawnSeconds = 10; // 최소 1초
    public int VillainMaxHP = 1; // 50마리당 1씩 증가, 최대 5

    public int Level = 1; // 레벨에 따른 빌런 타입 증가

    float spawnWaitTime = 0;

    [SerializeField] Transform TrainParent;
    //[SerializeField] GameObject Player;
    [SerializeField] GameObject[] SpawnPrefabs;
    [SerializeField] bool TestMode;

    void Awake()
    {
        instance = this;
        
        if (TestMode)
            TestSpanwerSetting();
    }

    void Update()
    {
        if (Player.Instance.State == Player.States.DEAD)
            return;

        spawnWaitTime += Time.deltaTime;
        if (spawnWaitTime >= SpawnSeconds)
        {
            spawnWaitTime = 0;

            Villain villain = SpawnOrNull(Player.Instance.transform.position);
        }
    }

    public void TestSpanwerSetting()
    {
        MaxSpawnCount = 10;
        SpawnSeconds = 3;
        VillainMaxHP = 1;
    }

    void MakeHard()
    {
        MaxSpawnCount = Mathf.Max(1 + KillCount / 10, 10);
        SpawnSeconds = Mathf.Max(1, 10 - KillCount / 10);
        VillainMaxHP = Mathf.Min(1 + KillCount / 50, 5);
    }

    public Villain SpawnOrNull(Vector2 playerPos)
    {
        if (CurSpawnCount >= MaxSpawnCount)
            return null;

        float spawn_x = Random.Range(-spawnRange + playerPos.x, spawnRange + playerPos.x);

        Villain.ESpawnType spawnType = (Villain.ESpawnType)Random.Range(0, 2);
        float spawn_y = spawnType == Villain.ESpawnType.Downstairs ? -10f : 10f;

        Vector2 spawnPos = new Vector2(spawn_x, spawn_y);

        int index = Random.Range(0, 2);

        Villain villain = null;
        if (TrainParent != null)
            villain = Instantiate(SpawnPrefabs[index], spawnPos, Quaternion.identity, TrainParent).GetComponent<Villain>();
        else
            villain = Instantiate(SpawnPrefabs[index], spawnPos, Quaternion.identity).GetComponent<Villain>();

        if (villain == null)
            return null;

        villain.spawnType = spawnType;
        //villain.SetPlayer(Player);
        villain.Init();

        EditorDebug.Log("[스폰] SpawnOrNull, Spawn New Villain");
        CurSpawnCount++;
        return villain;
    }
}
