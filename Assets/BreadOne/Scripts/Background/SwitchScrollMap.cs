using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cngamejam
{
    public class SwitchScrollMap : MonoBehaviour
    {
        public SpriteRenderer Map_1, Map_2;
        public float Speed;

        public bool Scroll = true;

        SpriteRenderer active_map, hide_map;

        private void Start()
        {
            InitMap();
        }

        // Update is called once per frame
        void Update()
        {
            if (Scroll)
            {
                active_map.transform.Translate(Vector3.left * Speed * Time.deltaTime);
                hide_map.transform.Translate(Vector3.left * Speed * Time.deltaTime);

                if (hide_map.transform.localPosition.x <= 0f)
                {
                    SwitchMap();
                }
            }
        }

        void InitMap()
        {
            active_map = Map_1;
            hide_map = Map_2;

            active_map.transform.localPosition = Vector3.zero;
            hide_map.transform.localPosition = new Vector3(active_map.sprite.bounds.size.x, 0f, 0f);
        }

        void SwitchMap()
        {
            //Switch
            SpriteRenderer temp = active_map;
            active_map = hide_map;
            hide_map = temp;

            hide_map.transform.localPosition = new Vector3(active_map.sprite.bounds.size.x, 0f, 0f);
        }
    }

}