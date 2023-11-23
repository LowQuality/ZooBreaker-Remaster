using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managements
{
    public class BGMManager: MonoBehaviour
    {
        [SerializeField] private List<AudioClip> bgmSources;
        [SerializeField] private AudioSource bgmPlayer;

        private bool _isFading;
        
        public static BGMManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        private void Start()
        {
            StartCoroutine(VolumeManager());
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
        public void FadeInNPlay(int index, float duration = 0)
        {
            bgmPlayer.clip = bgmSources[index];
            bgmPlayer.loop = true;
            bgmPlayer.Play();
            StartCoroutine(FadeInCoroutine(Settings.Instance.BgmVolume, duration));
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
                bgmPlayer.volume = (float)maxVolume / 100;
            }
            else
            {
                _isFading = true;
                while (bgmPlayer.volume < (float)maxVolume / 100)
                {
                    bgmPlayer.volume += Time.deltaTime / duration;
                    yield return null;
                }
                _isFading = false;
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
                _isFading = true;
                while (bgmPlayer.volume > 0.0f)
                {
                    bgmPlayer.volume -= Time.deltaTime / duration;
                    yield return null;
                }
                bgmPlayer.Stop();
                _isFading = false;
            }
        }
        private IEnumerator VolumeManager()
        {
            while (true)
            {
                if (!_isFading)
                {
                    bgmPlayer.volume = Settings.Instance.BgmVolume / 100f;
                    yield return null;
                }
                else
                {
                    yield return null;
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}