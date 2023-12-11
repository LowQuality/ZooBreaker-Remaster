using System.Collections;
using Managements;
using UnityEngine;

namespace Game
{
    public class Blocks : MonoBehaviour
    {
        /* Private Variables */
        [SerializeField] private new Rigidbody2D rigidbody2D;
    
        /* Unity API */
        private void Start()
        {
            StartCoroutine(CheckIsPlaying());
        }
    
        private void OnBecameInvisible()
        {
            if (transform.gameObject.CompareTag("Last") && !ValueManager.Instance.IsGameEnded)
            {
                Main.Instance.StartCoroutine(Main.Instance.GameOverDetect(transform.position.y));
            }
        }
    
        /* Coroutines */
        private IEnumerator CheckIsPlaying()
        {
            while (true)
            {
                yield return null;
                if (ValueManager.Instance.IsPlaying && !ValueManager.Instance.IsGameEnded && !ValueManager.Instance.IsGamePaused)
                {
                    rigidbody2D.simulated = true;
                }
                else
                {
                    rigidbody2D.simulated = false;
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}