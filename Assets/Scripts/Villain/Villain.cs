using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class Villain : MonoBehaviour
{
    public enum EAttackType
    {
        Short,
        Long,
    }

    public enum ESpawnType
    {
        Upstairs,
        Downstairs,
    }

    public enum EVillainAction
    {
        Spawn,
        Approach,
        Attack,
        Dead,
    }

    #region Inspector Setting
    [SerializeField] float MoveSpeed = 1f;
    [SerializeField] float rayDistance = 1f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float jumpPower = 5f;
     
    #endregion Inspector Setting

    [SerializeField] EAttackType attackType;
    public ESpawnType spawnType;
    public EVillainAction action;


    public int HP;
    public int Power = 1;

    Rigidbody2D rb;
    Collider2D coll;

    [SerializeField]
    bool isJump = false;
    
    [SerializeField]
    GameObject player; // 임시 더미 값, 참조 얻어와야 한다.

    private void Awake()
    {
        player = GameObject.Find("Player");
        Init();
    }

    void FixedUpdate()
    {
        if (isJump)
        {
            CheckGround();
        }
        else
        {
            if (action == EVillainAction.Spawn)
            {
                Spawn();
            }
            else if (action == EVillainAction.Approach)
            {
                Approach();
            }
            else if (action == EVillainAction.Attack)
            {
                Attack();
            }
        }
    }

    public void Init()
    {
        rb ??= GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        coll ??= GetComponent<Collider2D>();
        coll.enabled = false;

        HP = Spawner.Instance.VillainMaxHP;
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        isJump = false;
        ChangeAction(EVillainAction.Spawn);
    }

    public void ChangeAction(EVillainAction nextAction)
    {
        EditorDebug.LogFormat("[빌런] ChangeAction, {0} -> {1}", action, nextAction);
        action = nextAction;
    }

    public void Spawn()
    {
        if (spawnType == ESpawnType.Downstairs)
        {
            transform.Translate(Vector2.up * Time.deltaTime * MoveSpeed);
            if (transform.position.y > 1.4f) // 임의값 하드코딩
                SpawnFinish();
        }
        else // Upstairs
        {
            transform.Translate(Vector2.down * Time.deltaTime * MoveSpeed);
            if (transform.position.y < 1.6f) // 임의값 하드코딩
                SpawnFinish();
        }

    }

    void SpawnFinish()
    {
        ChangeAction(EVillainAction.Approach);
        coll.enabled = true;
        rb.gravityScale = 1f;
    }

    public void Approach()
    {
        if (player == null)
            return;

        if (IsPlayer())
            ChangeAction(EVillainAction.Attack);

        if (IsTrain())
        {
            Walk();
        }
        else
        {
            Jump();
        }
    }

    public void Attack()
    {
        if (IsPlayer() == false)
            ChangeAction(EVillainAction.Approach);


    }

    public void GetDemage()
    {
        HP--;

        if (HP <= 0)
            Dead();
    }

    void Dead()
    {
        ChangeAction(EVillainAction.Dead);
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        //Destroy(gameObject, 1f); // 임시로 1초 // dead 시간 동안 대기
    }

    bool IsTrain()
    {
        Vector3 moveDir = transform.position.x > player.transform.position.x ? Vector3.left : Vector3.right;

        Ray2D ray = new Ray2D(transform.position + moveDir * rayDistance, Vector2.down);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);
        //RaycastHit2D[] hits = Physics2D.RaycastAll(coll.GetBottomPos(), Vector2.down, rayDistance);

        bool isTarget = false;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.CompareTag("UpFloor"))
            {
                isTarget = true;
                break;
            }
        }

        //if (isTarget == false) EditorDebug.LogError("air");
        return isTarget;
    }

    bool IsPlayer()
    {
        Vector2 moveDir = transform.position.x > player.transform.position.x ? Vector2.left : Vector2.right;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, moveDir, attackRange);

        bool isTarget = false;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.CompareTag("Player"))
            {
                isTarget = true;
                break;
            }
        }

        //if (isTarget) EditorDebug.LogError("player");
        return isTarget;
    }

    void Walk()
    {
        Vector3 moveDir = transform.position.x > player.transform.position.x ? Vector3.left : Vector3.right;

        //rb.MovePosition(transform.position + moveDir * MoveSpeed * Time.deltaTime);
        transform.Translate(moveDir * MoveSpeed * Time.deltaTime);
        EditorDebug.Log("[빌런] Walk");
    }

    void Jump()
    {
        EditorDebug.Log("[빌런] Jump");
        isJump = true;
        Vector2 jumpDir = transform.position.x > player.transform.position.x ? new Vector2(-1, 1) : new Vector2(1, 1);
        rb.AddForce(jumpDir * jumpPower, ForceMode2D.Impulse);
    }

    void CheckGround()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(coll.GetBottomPos(), 0.1f);
        if (colls.FirstOrDefault(coll => coll.CompareTag("UpFloor")))
            isJump = false;
    }

    void OnDrawGizmosSelected()
    {
        if (player == null)
            return;

        Vector3 moveDir = transform.position.x > player.transform.position.x ? Vector3.left : Vector3.right;

        if (coll != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + moveDir * rayDistance, transform.position + moveDir * rayDistance - new Vector3(0, coll.bounds.extents.y, 0));
            //Gizmos.DrawLine(transform.position, transform.position + ray_offset + moveDir * rayDistance);
            //Gizmos.DrawLine(coll.GetBottomPos(), coll.GetBottomPos() + moveDir * rayDistance);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + moveDir * attackRange);
    }
}
