using System;
using System.Collections;
using Managements;
using UnityEngine;

namespace SceneOnly
{
    public class EndlessMode : MonoBehaviour
    {
        [SerializeField] private int maxBlockSize;
        [SerializeField] private GameObject startText;
        
        /* Unity API */
        private void Awake()
        {
            StartCoroutine(GameStart());
        }
        
        /* Coroutines */
        private IEnumerator GameStart()
        {
            SeManager.Instance.Play2Shot(11);
            ValueManager.Instance.GameMode = 2;
            ValueManager.Instance.IsGeneratingBlock = true;
            yield return new WaitForSeconds(0.1f);
            FadeManager.Instance.FadeIn(0.1f);
            StartCoroutine(GenerateBlocks());
            yield return new WaitForSeconds(0.05f);
            startText.SetActive(true);
            
            yield return new WaitForSeconds(0.3f);
            startText.SetActive(false);
            ValueManager.Instance.IsPlaying = true;
        }
        private IEnumerator GenerateBlocks()
        {
            // has Y2K38 Problem
            var seed = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            ValueManager.Instance.GameSeed = seed;
            var random = new System.Random(seed);
            
            while (true)
            {
                yield return null;
                
                if (ValueManager.Instance.QueuedBlocks().Count < 8)
                {
                    ValueManager.Instance.IsGeneratingBlock = true;
                    var id = random.Next(0, 3);
                    var size = random.Next(1, maxBlockSize + 1);
                    const int rotation = 0;

                    ValueManager.Instance.QueuedBlocks(id, size, rotation);
                }
                else
                {
                    ValueManager.Instance.IsGeneratingBlock = false;
                }
            }
        }
    }
}
