using System.Collections;
using Managements;
using UnityEngine;

namespace Game
{
    public class Blocks : MonoBehaviour
    {
        /* Private Variables */
        [SerializeField] private Rigidbody2D rigidBody2D;
        
        public bool playedCrashSound;
        
        private Camera _camera;
    
        /* Unity API */
        private void Start()
        {
            if (transform.parent.name.Contains("BlockStore"))
            {
                // Last 태그를 가진 블록 모두 태그 제거
                var lastBlocks = GameObject.FindGameObjectsWithTag("Last");
                foreach (var lastBlock in lastBlocks)
                {
                    lastBlock.tag = "Untagged";
                }
                // 이 블록을 Last 태그로 변경
                gameObject.tag = "Last";
            }
            
            _camera = Camera.main;
            StartCoroutine(CheckIsPlaying());
            StartCoroutine(CheckBlocksOutOfBound());
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (playedCrashSound) return;
            SeManager.Instance.Play2Shot(0);
            ValueManager.Instance.CanDropBlock = true;
            
            playedCrashSound = true;
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
            
            // 카메라가 보여주는 y좌표의 최솟값
            var minY = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane)).y;
            
            while (true)
            {
                yield return null;
                // 블록이 y:-5 이하로 떨어지면 게임오버
                if (transform.position.y < -5f && !ValueManager.Instance.IsGameEnded)
                {
                    Main.Instance.StartCoroutine(Main.Instance.GameOverDetect(transform.position.y));
                }
                // 블록의 x좌표가 카메라 밖으로 나가면 게임오버
                if ((transform.position.x < minX - 1f || transform.position.x > maxX + 1f) && !ValueManager.Instance.IsGameEnded)
                {
                    Main.Instance.StartCoroutine(Main.Instance.GameOverDetect(transform.position.y));
                }
                // 내가 Last 태그를 가지고 있으면서 블록이 카메라 밖으로 나가면 게임오버
                if (transform.position.y < minY - 1f && gameObject.CompareTag("Last") && !ValueManager.Instance.IsGameEnded)
                {
                    Main.Instance.StartCoroutine(Main.Instance.GameOverDetect(transform.position.y));
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}