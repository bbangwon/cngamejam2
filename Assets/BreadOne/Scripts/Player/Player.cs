using UnityEngine;
using UniRx;
using Spine.Unity;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

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

        [SerializeField]
        int maxHp;

        [SerializeField]
        int maxCaveSkills;

        ReactiveProperty<int> currentHp = new ReactiveProperty<int>();
        public ReadOnlyReactiveProperty<int> CurrentHP => currentHp.ToReadOnlyReactiveProperty();

        ReactiveProperty<int> caveSkills = new ReactiveProperty<int>();
        public ReadOnlyReactiveProperty<int> CaveSkills => caveSkills.ToReadOnlyReactiveProperty();

        ReactiveProperty<int> catchedEnemys = new ReactiveProperty<int>();
        public ReadOnlyReactiveProperty<int> CatchedEnemys => catchedEnemys.ToReadOnlyReactiveProperty();

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

        private void Awake()
        {
            currentHp.Value = maxHp;
            caveSkills.Value = maxCaveSkills;

            characterController = GetComponent<CharacterController2D>();
            skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        }

        private void Start()
        {
            Idle();
        }

        

        public void Damage()
        {
            if (currentHp.Value > 0)
                currentHp.Value--;

            if (currentHp.Value == 0)
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

        async UniTask Attack()
        {
            if(State != States.ATTACK)
            {
                State = States.ATTACK;
                await PlayAnimation("attack");
            }
        }

        async UniTask SpawnCave()
        {
            if(State == States.IDLE || State == States.MOVE)
            {
                if (caveSkills.Value > 0)
                {
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
                await PlayAnimation("dead", false);
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
            if (gotoIdle)
                Idle();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == "DeadZone")
            {
                characterController.Interactable = false;
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