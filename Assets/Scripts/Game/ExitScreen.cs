using System.Collections;
using Managements;
using UnityEngine;

namespace Game
{
    public class ExitScreen : MonoBehaviour
    {
        /* Unity API */
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape) || exit.activeSelf) return;
            SeManager.Instance.Play2Shot(7);
            ValueManager.Instance.IsGamePaused = true;
            BGMManager.Instance.Pause();
            exit.SetActive(true);
        }

        /* Variables */
        [SerializeField] private GameObject exit;
    
        /* Buttons */
        public void ConfirmButton()
        {
            StartCoroutine(Confirm());
        }
        public void CancelButton()
        {
            BGMManager.Instance.UnPause();
            exit.SetActive(false);
        }
    
        /* Coroutines */
        private IEnumerator Confirm()
        {
            FadeManager.Instance.BlackFXFadeOut(0.25f);
            yield return new WaitForSeconds(0.5f);
            Application.Quit();
        }
    }
}