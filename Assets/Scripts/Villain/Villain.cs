using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using Spine.Unity;
using cngamejam;
using Cysharp.Threading.Tasks;

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
    [Header("Settings")]
    [SerializeField] float MoveSpeed = 1f;
    [SerializeField] float rayDistance = 1f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackDelay = 1f;
    [SerializeField] float jumpPower = 5f;
    [SerializeField] Vector2 jumpVector = new Vector2(1f, 1f);
     
    #endregion Inspector Setting

    [Header("Data")]
    [SerializeField] EAttackType attackType;
    public ESpawnType spawnType;
    public EVillainAction action;


    public int HP;
    public int Power = 1;

    SkeletonAnimation spineAnim;
    Rigidbody2D rb;
    Collider2D coll;
    float attackWaitTime = 0;

    [SerializeField] bool isJump = false;

    [SerializeField] bool isAttack = false;
    
    [SerializeField]
    GameObject player; // ???? ???? ??, ???? ???????? ????.

    private void Awake()
    {
        player ??= GameObject.Find("Player");
        //Init();
    }

    void Update()
    {
        attackWaitTime += Time.deltaTime;
        if (attackWaitTime >= attackDelay)
        {
            attackWaitTime = 0;
            isAttack = false;
        }
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
            else if (action == EVillainAction.Attack && isAttack == false)
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

        spineAnim ??= GetComponentInChildren<SkeletonAnimation>();

        HP = Spawner.Instance.VillainMaxHP;
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        isJump = false;
        jumpVector = jumpVector.normalized;
        attackWaitTime = 0;

        ChangeAction(EVillainAction.Spawn);
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public void ChangeAction(EVillainAction nextAction)
    {
        EditorDebug.LogFormat("[????] ChangeAction, {0} -> {1}", action, nextAction);
        action = nextAction;

        switch(action)
        {
            case EVillainAction.Spawn:
                if (spawnType == ESpawnType.Downstairs)
                    ChangeSpineAnim("spawn_under", true);
                else
                    ChangeSpineAnim("move", true);
                break;

            case EVillainAction.Approach:
                ChangeSpineAnim("move", true);
                break;

            //case EVillainAction.Attack:
            //    ChangeSpineAnim("attack", false);
            //    break;

            case EVillainAction.Dead:
                ChangeSpineAnim("dead", false);
                break;

        }
    }

    void ChangeSpineAnim(string animationName, bool loop)
    {
        spineAnim.state.SetAnimation(0, animationName, loop);
    }

    public void Spawn()
    {
        if (spawnType == ESpawnType.Downstairs)
        {
            transform.Translate(Vector2.up * Time.deltaTime * MoveSpeed);
            if (transform.position.y > 1.4f) // ?????? ????????
                SpawnFinish();
        }
        else // Upstairs
        {
            transform.Translate(Vector2.down * Time.deltaTime * MoveSpeed);
            if (transform.position.y < 1.6f) // ?????? ????????
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

        Vector3 attackDir = transform.position.x > player.transform.position.x ? Vector3.left : Vector3.right;
        if (attackDir == Vector3.right)
            spineAnim.transform.localScale = new Vector3(-1f, 1f);
        else
            spineAnim.transform.localScale = new Vector3(1f, 1f);

        isAttack = true;
        ChangeSpineAnim("attack", false);
    }

    public bool GetDemage()
    {
        Debug.Log("GetDamage");
        HP--;
        EffectManager.Instance.SpawnAttackEffect(transform.position).Forget();

        if (HP <= 0)
        {
            Dead();
            return true;
        }
        else
        {
            ChangeSpineAnim("hit", false);
        }
        return false;
    }

    void Dead()
    {
        ChangeAction(EVillainAction.Dead);
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        //Destroy(gameObject, 1f); // ?????? 1?? // dead ???? ???? ????
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

    private void OnGUI()
    {
        Vector2 moveDir = transform.position.x > player.transform.position.x ? Vector2.left : Vector2.right;
        Debug.DrawRay(transform.position, moveDir * attackRange);
    }

    bool IsPlayer()
    {
        Vector2 moveDir = transform.position.x > player.transform.position.x ? Vector2.left : Vector2.right;

        RaycastHit2D hits = Physics2D.Raycast(transform.position, moveDir, attackRange, 1 << LayerMask.NameToLayer("Player"));

        if(hits)
        {
            Debug.Log(hits.collider.name);
            return true;
        }

        return false;

        /*
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
        */
    }

    void Walk()
    {
        Vector3 moveDir = transform.position.x > player.transform.position.x ? Vector3.left : Vector3.right;

        if (moveDir == Vector3.right)
            spineAnim.transform.localScale = new Vector3(-1f, 1f);
        else
            spineAnim.transform.localScale = new Vector3(1f, 1f);

        //rb.MovePosition(transform.position + moveDir * MoveSpeed * Time.deltaTime);
        transform.Translate(moveDir * MoveSpeed * Time.deltaTime);
        EditorDebug.Log("[????] Walk");
    }

    void Jump()
    {
        EditorDebug.Log("[????] Jump");
        isJump = true;
        Vector2 jumpDir = transform.position.x > player.transform.position.x ? jumpVector * new Vector2(-1f, 1f)  : jumpVector;
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
