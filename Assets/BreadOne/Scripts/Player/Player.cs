using UnityEngine;
using UniRx;

namespace cngamejam
{
    [RequireComponent(typeof(CharacterController2D))]
    public class Player : MonoSingleton<Player>
    {
        [SerializeField]
        int maxHp;

        ReactiveProperty<int> currentHp = new ReactiveProperty<int>();
        ReadOnlyReactiveProperty<int> CurrentHP => currentHp.ToReadOnlyReactiveProperty();

        CharacterController2D characterController;

        SpriteRenderer spriteRenderer;

        public float MoveValuePerFrame => characterController.MoveValuePerFrame;

        private void Awake()
        {
            currentHp.Value = maxHp;

            characterController = GetComponent<CharacterController2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Damage()
        {
            if (currentHp.Value > 0)
                currentHp.Value--;
        }

        private void Update()
        {
            Vector3 cam_position = Camera.main.transform.position;
            cam_position.x = transform.position.x;
            Camera.main.transform.position = cam_position;

            //flip..
            spriteRenderer.flipX = characterController.MoveDirection >= 0f;

            //Dead Zone...
            if (transform.position.y < -8f)
            {
                Dead();
            }
        }

        void Dead()
        {
            
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == "DeadZone")
            {
                characterController.Interactable = false;
            }
        }
    }

}