using UnityEngine;
using UniRx;
using Spine.Unity;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Events;

namespace cngamejam
{
    [RequireComponent(typeof(CharacterController2D))]
    public class Player : MonoSingleton<Player>
    {

        public enum States
        {
            NONE,
            IDLE,
            MOVE,
            ATTACK,
            DUCK,
            HIT,
            DEAD
        }

        public enum Directions
        {
            Left,
            Right
        }

        [SerializeField]
        int maxHp;

        [SerializeField]
        int maxCaveSkills;

        public UnityEvent<int> RemoveHP = new UnityEvent<int>();
        int currentHp;
        public int CurrentHp => currentHp;

        public UnityEvent<int> RemoveCaveSkill = new UnityEvent<int>();
        int caveSkills;
        public int CaveSkills => caveSkills;

        ReactiveProperty<int> catchedEnemys = new ReactiveProperty<int>();
        public ReadOnlyReactiveProperty<int> CatchedEnemys => catchedEnemys.ToReadOnlyReactiveProperty();

        public UnityEvent OnDie = new UnityEvent();
        public bool IsDie => State == States.DEAD;

        [SerializeField]
        GameObject cavePrefab;

        [SerializeField]
        float caveSpawnX;

        [SerializeField]
        float caveDespawnX;

        CharacterController2D characterController;

        SkeletonAnimation skeletonAnimation;

        public States State { get; private set; } = States.NONE;

        [SerializeField]
        AnimationReferenceAsset[] animationReferenceAssets;

        public float MoveValuePerFrame => characterController.MoveValuePerFrame;

        public Directions Direction => (skeletonAnimation.Skeleton.ScaleX > 0) ? Directions.Left : Directions.Right;

        [SerializeField]
        float attackRange;

        

        private void OnGUI()
        {
            if(Direction == Directions.Left)
            {
                Debug.DrawRay(transform.position, (transform.right * -1f) * attackRange);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.right * attackRange);
            }
        }

        private void Awake()
        {
            currentHp = maxHp;
            caveSkills = maxCaveSkills;

            characterController = GetComponent<CharacterController2D>();
            skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        }

        private void Start()
        {
            Idle();
        }

        

        public void Damage()
        {


            if (currentHp > 0)
            {
                SoundManager.Instance.Play("07_hit_voice_pc");
                currentHp--;
                RemoveHP.Invoke(currentHp);
                
            }

            EffectManager.Instance.SpawnAttackEffect(transform.position).Forget();

            if (currentHp == 0)
                Dead().Forget();
        }

        private void Update()
        {
            if (State == States.DEAD)
                return;

            if(State == States.IDLE || State == States.MOVE)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    Attack().Forget();
                }

                if(Input.GetButtonDown("Fire2"))
                {
                    SpawnCave().Forget();
                }
            }

            UpdateAnimState();

            Vector3 cam_position = Camera.main.transform.position;
            cam_position.x = transform.position.x;
            Camera.main.transform.position = cam_position;

            SetFlip(characterController.MoveDirection);

            //Dead Zone...
            if (transform.position.y < -8f)
            {
                Dead().Forget();
            }
        }

        private void UpdateAnimState()
        {
            if (State != States.ATTACK && State != States.HIT && State != States.DUCK)
            {
                if (characterController.MoveValuePerFrame != 0)
                {
                    Move();
                }
                else
                {
                    Idle();
                }
            }
        }

        

        void Idle()
        {
            if(State != States.IDLE)
            {
                State = States.IDLE;
                PlayLoopAnimation("idle");
            }
        }

        void Move()
        {
            if(State != States.MOVE)
            {
                State = States.MOVE;
                PlayLoopAnimation("move");
            }
        }

        void Duck()
        {
            if(State != States.DUCK)
            {
                State = States.DUCK;
                PlayLoopAnimation("duck");
            }
        }

        async void AttackSound()
        {
            SoundManager.Instance.Play("04_atk_pc");
            await UniTask.Delay(400);
            SoundManager.Instance.Play("04_atk_pc");
        }

        async UniTask Attack()
        {
            if(State != States.ATTACK)
            {
                State = States.ATTACK;

                AttackSound();
                await PlayAnimation("attack");


                //Attack 처리                
                RaycastHit2D raycastHit = Physics2D.Raycast(transform.position,
                    transform.right * ((Direction == Directions.Left) ? -1f : 1f),
                    attackRange, 1 << LayerMask.NameToLayer("Enemy"));

                if(raycastHit)
                {
                    
                    Villain villain = raycastHit.collider.gameObject.GetComponent<Villain>();
                    if (villain != null)
                    {
                        bool isDead = villain.GetDemage();
                        if(isDead)
                        {
                            //catchedEnemys.Value++;
                            catchedEnemys.Value = Spawner.Instance.KillCount;
                        }
                    }
                        
                }
            }
        }

        async UniTask SpawnCave()
        {
            if(State == States.IDLE || State == States.MOVE)
            {
                if (caveSkills > 0)
                {
                    caveSkills--;

                    skeletonAnimation.GetComponent<MeshRenderer>().sortingLayerName = "Effect";
                    skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = 11;

                    await EffectManager.Instance.SpawnSkillEffect();

                    skeletonAnimation.GetComponent<MeshRenderer>().sortingLayerName = "Player";
                    skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = 0;


                    RemoveCaveSkill.Invoke(caveSkills);
                    SoundManager.Instance.Play("09_skill_train");

                    Duck();
                    GameObject cave = Instantiate(cavePrefab);
                    cave.transform.position = new Vector3(caveSpawnX, 0f, 0f);

                    await UniTask.WaitUntil(() => {
                        if(cave != null)
                        {
                            return cave.transform.position.x <= caveDespawnX;
                        }
                        return true;

                    });
                    if (cave != null)
                        Destroy(cave);

                    catchedEnemys.Value = Spawner.Instance.KillCount;
                    //catchedEnemys.Value += Spawner.Instance.CurSpawnCount;
                    Idle();
                }
            }

        }

        async UniTask Hit()
        {
            if(State != States.HIT)
            {
                State = States.HIT;
                await PlayAnimation("hit");
            }
        }

        async UniTask Dead()
        {
            if(State != States.DEAD)
            {
                State = States.DEAD;
                characterController.Interactable = false;   
                await PlayAnimation("dead", false);

                OnDie?.Invoke();
            }
         }

        Spine.Animation GetAnimation(string animName) => animationReferenceAssets.FirstOrDefault(anim => anim.name == animName)?.Animation ?? null;

        void PlayLoopAnimation(string animName)
        {
            var anim = GetAnimation(animName);
            skeletonAnimation.state.SetAnimation(0, anim, true);
        }

        async UniTask PlayAnimation(string animName, bool gotoIdle = true)
        {
            var anim = GetAnimation(animName);
            skeletonAnimation.state.SetAnimation(0, anim, false);

            await UniTask.Delay(TimeSpan.FromSeconds(anim.Duration).Milliseconds);
            if (gotoIdle && State != States.DEAD)
                Idle();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == "DeadZone")
            {
                characterController.Interactable = false;
                Dead().Forget();
            }
        }

        public void SetFlip(float horizontal)
        {
            if (horizontal != 0)
            {
                skeletonAnimation.Skeleton.ScaleX = horizontal > 0 ? -1f : 1f;
            }
        }
    }

}