using UnityEngine;

namespace cngamejam
{

    public class BackgroundController : MonoSingleton<BackgroundController>
    {
        [SerializeField]
        SwitchScrollMap[] scrollMaps;

        private void Awake()
        {
            ResumeScroll();
        }

        public void PauseScroll()
        {
            foreach (var scrollMap in scrollMaps)
            {
                scrollMap.Scroll = false;
            }
        }

        public void ResumeScroll()
        {
            foreach (var scrollMap in scrollMaps)
            {
                scrollMap.Scroll = true;
            }
        }
    }
}