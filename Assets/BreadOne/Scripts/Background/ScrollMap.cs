using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cngamejam{

    public class ScrollMap : MonoBehaviour
    {
        public float Speed;

        public bool Scroll = true;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(Scroll)
            {
                transform.Translate(Vector3.left * Speed * Time.deltaTime);
            }
        }
    }
}
