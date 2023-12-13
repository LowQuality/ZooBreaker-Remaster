using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managements
{
    public class SeManager : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> seSources;
        [SerializeField] private AudioSource sePlayer;
        
        public static SeManager Instance { get; private set; }
        
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
        /// 효과음을 재생합니다.
        /// <br />
        /// <para>인덱스 목록 :</para>
        /// 0 = 블록 접촉<br />
        /// 1 = 클리어 타이머<br />
        /// 2 = 콤보(?) / 팝업 표시 / 게임 준비<br />
        /// 3 = 게임 클리어<br />
        /// 4 = 게임 오버<br />
        /// 5 = 잠긴 버튼 클릭<br />
        /// 6 = 잠금<br />
        /// 7 = 메뉴 선택<br />
        /// 8 = 뉴 레코드<br />
        /// 9 = 블록 떨어뜨림<br />
        /// 10 = 시간 경고<br />
        /// 11 = 게임 시작<br />
        /// </summary>
        /// <param name="index">재생할 효과음의 인덱스입니다.</param>
        public void Play2Shot(int index)
        {
            sePlayer.PlayOneShot(seSources[index], Settings.Instance.SeVolume / 100f);
        }
        
        /* Coroutines */
        private IEnumerator VolumeManager()
        {
            while (true)
            {
                sePlayer.volume = Settings.Instance.SeVolume / 100f;
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}