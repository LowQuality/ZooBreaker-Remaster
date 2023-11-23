using System.Collections;
using Managements;
using UnityEngine;

namespace SceneOnly
{
    public class EndlessMode : MonoBehaviour
    {
        /* Unity API */
        private void Start()
        {
            StartCoroutine(GameStart());
        }
        
        /* Coroutines */
        private IEnumerator GameStart()
        {
            SeManager.Instance.Play2Shot(11);
            ValueManager.Instance.IsPlaying = true;
            yield return new WaitForSeconds(0.1f);
            FadeManager.Instance.FadeIn(0.1f);

            ValueManager.Instance.EndlessModeHighScore(49849894);
            ValueManager.Instance.EndlessModeHighScore(419871651);
            ValueManager.Instance.EndlessModeHighScore(1561651632);
            ValueManager.Instance.EndlessModeHighScore(21165);
            ValueManager.Instance.EndlessModeHighScore(4918);
            ValueManager.Instance.EndlessModeHighScore(89489);
            ValueManager.Instance.EndlessModeHighScore(149811);
            ValueManager.Instance.EndlessModeHighScore(149811486);
        }
    }
}
