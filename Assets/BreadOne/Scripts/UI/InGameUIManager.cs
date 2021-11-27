using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using DG.Tweening;
using UnityEngine.SceneManagement;

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
        Text textCatchedEnemy;

        [SerializeField]
        GameObject heartPrefab;

        [SerializeField]
        GameObject cavePrefab;

        [SerializeField]
        GameObject result;

        [SerializeField]
        Text resultText;

        [SerializeField]
        Button retryButton;

        [SerializeField]
        Button titleButton;

        // Start is called before the first frame update
        void Start()
        {
            SoundManager.Instance.PlayBGM("01_ingame");

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

            Player.Instance.OnDie.AddListener(ShowResult);
            retryButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(1);
            });

            titleButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(0);
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

        void ShowResult()
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.Play("02_result");
            result.SetActive(true);

            resultText.text = $"당신은 총 {Player.Instance.CatchedEnemys.Value}마리의 악귀를\n지옥행 급행열차에 태웠습니다.";

            retryButton.transform.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            titleButton.transform.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }

}