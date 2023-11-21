using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace Management
{
    public class FadeManager : MonoBehaviour
    {
        [SerializeField] private Image whiteFX;
        [SerializeField] private Image blackFX;

        private readonly Color _whiteColor = Color.white;
        private readonly Color _blackColor = Color.black;

        public static FadeManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        [Description ("화면을 정해진 시간동안 흰색으로 전환됩나다.")]
        public void WhiteFXFadeOut(float time)
        {
            StopAllCoroutines();
            StartCoroutine(WhiteFXFadeOutCoroutine(time));
        }

        [Description ("화면을 정해진 시간동안 검은색으로 전환됩나다.")]
        public void BlackFXFadeOut(float time)
        {
            StopAllCoroutines();
            StartCoroutine(BlackFXFadeOutCoroutine(time));
        }

        [Description ("페이드 아웃된 화면을 정해진 시간동안 페이드 인됩니다.")]
        public void FadeIn(float time)
        {
            StopAllCoroutines();
            StartCoroutine(FadeInCoroutine(time));
        }

        /* Coroutines */
        private IEnumerator WhiteFXFadeOutCoroutine(float time)
        {
            whiteFX.gameObject.SetActive(true);
            if (Mathf.Approximately(time, 0f))
            {
                whiteFX.color = _whiteColor;
            }
            else
            {
                while (whiteFX.color.a < 1.0f)
                {
                    whiteFX.color = new Color(_whiteColor.r, _whiteColor.g, _whiteColor.b, whiteFX.color.a + Time.deltaTime / time);
                    yield return null;
                }
            }
        }

        private IEnumerator BlackFXFadeOutCoroutine(float time)
        {
            blackFX.gameObject.SetActive(true);
            if (Mathf.Approximately(time, 0f))
            {
                blackFX.color = _blackColor;
            }
            else
            {
                while (blackFX.color.a < 1.0f)
                {
                    blackFX.color = new Color(_blackColor.r, _blackColor.g, _blackColor.b, blackFX.color.a + Time.deltaTime / time);
                    yield return null;
                }
            }
        }

        private IEnumerator FadeInCoroutine(float time)
        {
            if (Mathf.Approximately(time, 0f))
            {
                blackFX.color = new Color(_blackColor.r, _blackColor.g, _blackColor.b, 0f);
                whiteFX.color = new Color(_whiteColor.r, _whiteColor.g, _whiteColor.b, 0f);
            }
            else
            {
                while (blackFX.color.a > 0.0f)
                {
                    blackFX.color = new Color(_blackColor.r, _blackColor.g, _blackColor.b, blackFX.color.a - Time.deltaTime / time);
                    whiteFX.color = new Color(_whiteColor.r, _whiteColor.g, _whiteColor.b, whiteFX.color.a - Time.deltaTime / time);
                    yield return null;
                }
                blackFX.gameObject.SetActive(false);
                whiteFX.gameObject.SetActive(false);
            }
        }
    }
}
