using System.Collections;
using Management;
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
            yield return new WaitForSeconds(0.1f);
            FadeManager.Instance.FadeIn(0.1f);
        }
    }
}
