using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace cngamejam{

    public class Train : MonoBehaviour
    {

        [SerializeField]
        float strength = 0.1f;

        [SerializeField]
        int vibrato = 5;

        // Start is called before the first frame update
        void Start()
        {
            transform.DOShakePosition(1f, strength: this.strength, vibrato: this.vibrato).SetLoops(-1);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
