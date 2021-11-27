using UnityEngine;
using System.Linq;

namespace cngamejam{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [SerializeField]
        AudioClip[] audioClips;

        AudioSource bgmSource;

        private void Awake()
        {
            if(instance != null && instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        public void Play(string audioClipName)
        {
            Vector3 playPosition = Camera.main.transform.position;

            AudioClip clip = audioClips.FirstOrDefault(c => c.name == audioClipName);

            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, playPosition);
        }

        public void PlayBGM(string bgmName)
        {
            AudioClip bgmClip = audioClips.FirstOrDefault(clip => clip.name == bgmName);

            if(bgmClip != null)
            {
                bgmSource = GetComponent<AudioSource>();

                bgmSource.clip = bgmClip;
                bgmSource.loop = true;
                bgmSource.Play();
            }
        }

        public void StopBGM()
        {
            if(bgmSource != null)
            {
                bgmSource.Stop();
            }

        }
    }
}