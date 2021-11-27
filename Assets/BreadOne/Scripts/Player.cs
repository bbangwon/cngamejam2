using UnityEngine;
using UniRx;

namespace cngamejam
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        int maxHp;

        ReactiveProperty<int> currentHp = new ReactiveProperty<int>();
        ReadOnlyReactiveProperty<int> CurrentHP => currentHp.ToReadOnlyReactiveProperty();

        private void Awake()
        {
            currentHp.Value = maxHp;
        }

        public void Damage()
        {
            if (currentHp.Value > 0)
                currentHp.Value--;
        }
    }

}