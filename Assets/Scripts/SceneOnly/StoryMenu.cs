using System.Collections;
using Management;
using UnityEngine;

namespace SceneOnly
{
    public class StoryMenu : MonoBehaviour
    {
        /* Unity API */
        private void Start()
        {
            StartCoroutine(FadeInNewLoad());
            
        }
        
        /* Coroutines */
        private static IEnumerator FadeInNewLoad()
        {
            if (!ValueManager.Instance.IsStoryWatched && ValueManager.Instance.LastLevelLocated == 0)
            {
                BGMManager.Instance.FadeInNPlay(1, 0.5f);
                yield return new WaitForSeconds(0.1f);
                FadeManager.Instance.FadeIn(0.1f);
                
                ValueManager.Instance.LastLevelLocated = 1;
                ValueManager.Instance.IsStoryWatched = true;
            }
            else
            {
                
            }
        }
    }
}