using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace cngamejam{

    public class SkillEffect : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SoundManager.Instance.Play("08_skill_use");
        }
    }

}