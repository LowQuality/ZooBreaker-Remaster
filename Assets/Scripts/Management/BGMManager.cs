using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management
{
    public class BGMManager: MonoBehaviour
    {
        [SerializeField] private List<AudioClip> bgmSources;
        [SerializeField] private AudioSource bgmPlayer;
        
        public static BGMManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        /// <summary>
        /// 배경음악을 재생합니다.
        /// <br />
        /// <para>인덱스 목록 :</para>
        /// 0 = 타이틀<br />
        /// 1 = 인트로<br />
        /// 2 = 동물원<br />
        /// 3 = 도시<br />
        /// 4 = 숲<br />
        /// 5 = 바다<br />
        /// 6 = 엔딩<br />
        /// </summary>
        /// <param name="index">재생할 효과음의 인덱스입니다.</param>
        /// <param name="duration">지속시간을 설정합니다.</param>
        /// <param name="maxVolume">페이드 인 후 최대 볼륨을 설정합니다. 비워놓을 경우 100으로 자동 설정됩니다.</param>
        public void FadeInNPlay(int index, float duration = 0, int maxVolume = -1)
        {
            if (maxVolume == -1)
            {
                maxVolume = 100;
            }
            
            bgmPlayer.clip = bgmSources[index];
            bgmPlayer.volume = 0;
            bgmPlayer.loop = true;
            bgmPlayer.Play();
            StartCoroutine(FadeInCoroutine(maxVolume, duration));
        }
        public void FadeOut(float duration = 0)
        {
            StartCoroutine(FadeOutCoroutine(duration));
        }
        public void Pause()
        {
            bgmPlayer.Pause();
        }
        public void UnPause()
        {
            bgmPlayer.UnPause();
        }
        
        /* Coroutines */
        private IEnumerator FadeInCoroutine(int maxVolume, float duration)
        {
            bgmPlayer.volume = 0;
            if (Mathf.Approximately(duration, 0f))
            {
                bgmPlayer.volume = maxVolume;
            }
            else
            {
                while (bgmPlayer.volume < maxVolume)
                {
                    bgmPlayer.volume += + Time.deltaTime / duration;
                    yield return null;
                }
                bgmPlayer.volume = maxVolume;
            }
        }
        private IEnumerator FadeOutCoroutine(float duration)
        {
            if (Mathf.Approximately(duration, 0f))
            {
                bgmPlayer.volume = 0;
                bgmPlayer.Stop();
            }
            else
            {
                while (bgmPlayer.volume > 0)
                {
                    bgmPlayer.volume -= - Time.deltaTime / duration;
                    yield return null;
                }
                bgmPlayer.Stop();
            }
        }
    }
}