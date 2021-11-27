using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

namespace cngamejam{

    public class InGameUIManager : MonoBehaviour
    {
        [SerializeField]
        Transform transformHearts;

        [SerializeField]
        Transform transformCaves;

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

        }

        // Update is called once per frame
        void Update()
        {

        }

        void RemoveHP(int hp)
        {
            Destroy(transformHearts.GetChild(transformHearts.childCount - 1).gameObject);
        }

        void RemoveCaveSkill(int caveSkill)
        {
            Destroy(transformCaves.GetChild(transformCaves.childCount - 1).gameObject);
        }
    }

}