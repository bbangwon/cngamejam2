using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using DG.Tweening;

namespace cngamejam{

    public class InGameUIManager : MonoBehaviour
    {
        [SerializeField]
        Transform transformHearts;

        [SerializeField]
        Transform transformCaves;

        [SerializeField]
        Transform transformCatchedEnemy;

        [SerializeField]
        TextMeshProUGUI textCatchedEnemy;

        [SerializeField]
        GameObject heartPrefab;

        [SerializeField]
        GameObject cavePrefab;

        // Start is called before the first frame update
        void Start()
        {
            Player.Instance.RemoveHP.AddListener(RemoveHP);
            Player.Instance.RemoveCaveSkill.AddListener(RemoveCaveSkill);

            foreach (Transform heart in transformHearts)
            {
                Destroy(heart.gameObject);
            }

            for (int i = 0; i < Player.Instance.CurrentHp; i++)
            {
                Instantiate(heartPrefab, transformHearts);
            }

            foreach (Transform cave in transformCaves)
            {
                Destroy(cave.gameObject);
            }

            for (int i = 0; i < Player.Instance.CaveSkills; i++)
            {
                Instantiate(cavePrefab, transformCaves);
            }

            Player.Instance.CatchedEnemys.Subscribe(catchedEnemys =>
            {
                PulseCatchedEnemy();
                textCatchedEnemy.text = $"x{catchedEnemys}";
            });

        }

        async void PulseCatchedEnemy()
        {
            await transformCatchedEnemy.transform.DOScale(1.2f, 0.15f).AsyncWaitForCompletion();
            await transformCatchedEnemy.transform.DOScale(1f, 0.1f).AsyncWaitForCompletion();
        }

        void RemoveHP(int hp)
        {
            Destroy(transformHearts.GetChild(transformHearts.childCount - 1).gameObject);
        }

        void RemoveCaveSkill(int caveSkill)
        {
            Destroy(transformCaves.GetChild(transformCaves.childCount - 1).gameObject);
        }

        private void Update()
        {
            if(Input.GetKey(KeyCode.Alpha1))
            {
                PulseCatchedEnemy();
            }
        }
    }

}