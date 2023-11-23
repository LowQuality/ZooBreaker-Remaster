using System.Collections;
using Managements;
using UnityEngine;

public class Block : MonoBehaviour
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
        if (transform.gameObject.CompareTag("Last") && ValueManager.Instance.IsPlaying)
        {
            Game.Instance.StartCoroutine(Game.Instance.GameOverDetect(transform.position.y));
        }
    }
    
    /* Coroutines */
    private IEnumerator CheckIsPlaying()
    {
        while (true)
        {
            yield return null;
            if (!ValueManager.Instance.IsPlaying || ValueManager.Instance.IsGamePaused)
            {
                rigidbody2D.simulated = false;
            }
            else
            {
                rigidbody2D.simulated = true;
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }
}