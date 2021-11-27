using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace cngamejam{

    public class TitleManager : MonoBehaviour
    {

        [SerializeField]
        Image blackImage;

        [SerializeField]
        Image player;

        [SerializeField]
        Image light;

        [SerializeField]
        Image Enemy;

        [SerializeField]
        Image title;

        [SerializeField]
        Text pleaseAnyKey;

        bool interactable = false;

        private void Start()
        {
            ShowTitle2();
        }

        async void ShowTitle()
        {
            await blackImage.DOFade(0f, 2f).AsyncWaitForCompletion();

            interactable = true;
        }


        async void ShowTitle2()
        {
            pleaseAnyKey.DOFade(0f, 0f);
            player.transform.DOMoveY(-1100f, 0f);
            Enemy.transform.DOMoveY(-500f, 0f);
            title.transform.DOMoveX(-1600f, 0f);
            light.DOFade(0f, 0f);

            await blackImage.DOFade(0f, 2f).AsyncWaitForCompletion();

            SoundManager.Instance.PlayBGM("00_title");

            player.transform.DOMoveY(540f, 1f).SetEase(Ease.InQuad);

            await Enemy.transform.DOMoveY(540f - 248f, 1f).SetDelay(0.5f).SetEase(Ease.InQuad).AsyncWaitForCompletion();

            await title.transform.DOMoveX(960f - 310f, 1f).AsyncWaitForCompletion();

            light.DOFade(1f, 0.1f).SetDelay(0.5f);


            pleaseAnyKey.DOFade(1f, 0.5f).SetLoops(-1, LoopType.Yoyo);


            interactable = true;
        }


        // Update is called once per frame
        void Update()
        {
            if (interactable && Input.anyKey)
            {
                LoadInGameScene();
                interactable = false;
            }
        }

        async void LoadInGameScene()
        {
            SoundManager.Instance.Play("04_atk_pc");
            await UniTask.Delay(500);
            SceneManager.LoadScene(1);
        }
    }
}
