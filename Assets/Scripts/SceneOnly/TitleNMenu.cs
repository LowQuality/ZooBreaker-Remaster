using System.Collections;
using Managements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SceneOnly
{
    public class TitleNMenu : MonoBehaviour
    {
        /* Unity API */
        private void Start()
        {
            FadeManager.Instance.FadeIn(0.1f);
            StartCoroutine(ResetData());
            StartCoroutine(SoundSettings());
            BGMManager.Instance.FadeInNPlay(0);
            Application.targetFrameRate = 300;
        }
        
        /* Variables */
        [SerializeField] private GameObject title;
        [SerializeField] private GameObject menu;
        [SerializeField] private GameObject records;
        [SerializeField] private GameObject settings;
        [SerializeField] private GameObject help;
        
        [SerializeField] private GameObject resetData;
        
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider seSlider;
        
        /* Functions */
        private void SceneSetActivates(bool bTitle, bool bMenu, bool bRecords, bool bSettings, bool bHelp)
        {
            title.SetActive(bTitle);
            menu.SetActive(bMenu);
            records.SetActive(bRecords);
            settings.SetActive(bSettings);
            help.SetActive(bHelp);
        }
        
        /* Buttons */
        public void TitleClick()
        {
            StartCoroutine(Title2Menu());
        }
        public void StoryModeClick()
        {
            StartCoroutine(Menu2StoryMode());
        }
        public void EndlessModeClick()
        {
            StartCoroutine(Menu2EndlessMode());
        }
        public void RecordsClick()
        {
            StartCoroutine(Menu2Records());
        }
        public void SettingsClick()
        {
            StartCoroutine(Menu2Settings());
        }
        public void HelpClick()
        {
            StartCoroutine(Menu2Help());
        }
        public void ExitClick()
        {
            StartCoroutine(Exit());
        }
        public void DataResetClick()
        {
            SeManager.Instance.Play2Shot(7);
            resetData.SetActive(true);
        }
        public void DataResetConfirmClick()
        {
            SeManager.Instance.Play2Shot(7);
            resetData.SetActive(false);
            ValueManager.Instance.ResetData();
        }
        public void DataResetCancelClick()
        {
            SeManager.Instance.Play2Shot(7);
            resetData.SetActive(false);
        }
        
        /* Coroutines */
        private IEnumerator Title2Menu()
        {
            SeManager.Instance.Play2Shot(7);
            FadeManager.Instance.BlackFXFadeOut(0.1f);
            yield return new WaitForSeconds(0.5f);
            SceneSetActivates(false, true, false, false, false);
            FadeManager.Instance.FadeIn(0.1f);
        }
        private IEnumerator Menu2StoryMode()
        {
            SeManager.Instance.Play2Shot(7);
            FadeManager.Instance.BlackFXFadeOut(0.1f);
            yield return new WaitForSeconds(0.5f);
            BGMManager.Instance.FadeOut(0.5f);
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("StoryMenu");
        }
        private IEnumerator Menu2EndlessMode()
        {
            SeManager.Instance.Play2Shot(7);
            FadeManager.Instance.BlackFXFadeOut(0.1f);
            yield return new WaitForSeconds(0.5f);
            BGMManager.Instance.FadeOut(0.5f);
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("EndlessMode");
        }
        private IEnumerator Menu2Records()
        {
            SeManager.Instance.Play2Shot(7);
            FadeManager.Instance.BlackFXFadeOut(0.1f);
            yield return new WaitForSeconds(0.5f);
            SceneSetActivates(false, false, true, false, false);
            FadeManager.Instance.FadeIn(0.1f);
        }
        private IEnumerator Menu2Settings()
        {
            SeManager.Instance.Play2Shot(7);
            FadeManager.Instance.BlackFXFadeOut(0.1f);
            yield return new WaitForSeconds(0.5f);
            SceneSetActivates(false, false, false, true, false);
            FadeManager.Instance.FadeIn(0.1f);
        }
        private IEnumerator Menu2Help()
        {
            SeManager.Instance.Play2Shot(7);
            FadeManager.Instance.BlackFXFadeOut(0.1f);
            yield return new WaitForSeconds(0.5f);
            SceneSetActivates(false, false, false, false, true);
            FadeManager.Instance.FadeIn(0.1f);
        }
        private static IEnumerator Exit()
        {
            SeManager.Instance.Play2Shot(7);
            FadeManager.Instance.BlackFXFadeOut(0.25f);
            yield return new WaitForSeconds(0.5f);
            Application.Quit();
        }
        private static IEnumerator ResetData()
        {
            var resetData = 0;
            while (true)
            {
                yield return null;
                if (!Input.GetKeyDown(KeyCode.F2)) continue;
                resetData++;
                if (resetData != 10) continue;
                ValueManager.Instance.ResetData();
                resetData = 0;
            }
            // ReSharper disable once IteratorNeverReturns
        }
        private IEnumerator SoundSettings()
        {
            bgmSlider.value = Settings.Instance.BgmVolume;
            seSlider.value = Settings.Instance.SeVolume;
            while (true)
            {
                yield return null;
                Settings.Instance.BgmVolume = (int)bgmSlider.value;
                Settings.Instance.SeVolume = (int)seSlider.value;
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}