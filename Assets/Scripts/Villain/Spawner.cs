using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cngamejam;

public class Spawner : MonoBehaviour
{
    static Spawner instance;
    public static Spawner Instance { get => instance; }

    [SerializeField] int killCount;
    public int KillCount // 10������ ���� 1�ʾ� ����
    {
        get => killCount;
        set
        {
            killCount = value;
            CurSpawnCount--;
            MakeHard();
        }
    }

    public int MaxSpawnCount = 1; // 10������ 1�� ����, �ִ� 10����
    public int CurSpawnCount = 0;
    public int SpawnSeconds = 10; // �ּ� 1��
    public int VillainMaxHP = 1; // 50������ 1�� ����, �ִ� 5

    [Header("Instpactor Settings")]
    public float spawnRange = 20f;

    public int FirstSpawnCount = 1;
    public int LastSpawnCount = 10;
    public int CountPerKill = 10;

    public int FirstSpawnSeconds = 7;
    public int LastSpawnSeconds = 1;
    public int SpawnPerKill = 10;

    public int FirstVillainHP = 1;
    public int LastVillainHP = 5;
    public int HPPerKill = 50;


    //public int Level = 1; // ������ ���� ���� Ÿ�� ����

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
        else
        {
            MakeHard();
        }
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
        MaxSpawnCount = 1;
        SpawnSeconds = 3;
        VillainMaxHP = 10;
    }

    void MakeHard()
    {
        MaxSpawnCount = Mathf.Min(FirstSpawnCount + KillCount / CountPerKill, LastSpawnCount);
        SpawnSeconds = Mathf.Max(LastSpawnSeconds, FirstSpawnSeconds - KillCount / SpawnPerKill);
        VillainMaxHP = Mathf.Min(FirstVillainHP + KillCount / HPPerKill, LastVillainHP);
    }

    public Villain SpawnOrNull(Vector2 playerPos)
    {
        if (CurSpawnCount >= MaxSpawnCount)
            return null;

        float spawn_x = Random.Range(-spawnRange + playerPos.x, spawnRange + playerPos.x);

        Villain.ESpawnType spawnType = (Villain.ESpawnType)Random.Range(0, 2);
        float spawn_y = spawnType == Villain.ESpawnType.Downstairs ? -7f : 7f;

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

        EditorDebug.Log("[����] SpawnOrNull, Spawn New Villain");
        CurSpawnCount++;
        return villain;
    }
}
