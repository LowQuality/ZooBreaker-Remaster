using System;
using System.Collections;
using Managements;
using UnityEngine;

namespace SceneOnly
{
    public class EndlessMode : MonoBehaviour
    {
        [SerializeField] private int maxBlockSize;
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
            ValueManager.Instance.IsPlaying = true;
            yield return new WaitForSeconds(0.1f);
            FadeManager.Instance.FadeIn(0.1f);

            StartCoroutine(GenerateBlocks());
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
                    var id = random.Next(1, 3);
                    var size = random.Next(1, maxBlockSize);
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
