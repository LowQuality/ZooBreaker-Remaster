using System.Collections;
using Managements;
using UnityEngine;

namespace Game
{
    public class Blocks : MonoBehaviour
    {
        /* Private Variables */
        [SerializeField] private Rigidbody2D rigidBody2D;
        
        private Camera _camera;
    
        /* Unity API */
        private void Start()
        {
            _camera = Camera.main;
            StartCoroutine(CheckIsPlaying());
            StartCoroutine(CheckBlocksOutOfBound());
        }
    
        /* Coroutines */
        private IEnumerator CheckIsPlaying()
        {
            while (true)
            {
                yield return null;
                if (ValueManager.Instance.IsPlaying && !ValueManager.Instance.IsGameEnded && !ValueManager.Instance.IsGamePaused)
                {
                    rigidBody2D.simulated = true;
                }
                else
                {
                    rigidBody2D.simulated = false;
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }
        private IEnumerator CheckBlocksOutOfBound()
        {
            // 카메라가 보여주는 x좌표의 최대최솟값
            var minX = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane)).x;
            var maxX = _camera.ViewportToWorldPoint(new Vector3(1, 0, _camera.nearClipPlane)).x;
            
            while (true)
            {
                yield return null;
                // 블럭이 y:-15 이하로 떨어지면 게임오버
                if (transform.position.y < -15f && !ValueManager.Instance.IsGameEnded)
                {
                    Main.Instance.StartCoroutine(Main.Instance.GameOverDetect(transform.position.y));
                }
                // 블럭의 x좌표가 카메라 밖으로 나가면 게임오버
                if ((transform.position.x < minX - 1f || transform.position.x > maxX + 1f) && !ValueManager.Instance.IsGameEnded)
                {
                    Main.Instance.StartCoroutine(Main.Instance.GameOverDetect(transform.position.y));
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}