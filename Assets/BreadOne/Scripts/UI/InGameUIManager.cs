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

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}