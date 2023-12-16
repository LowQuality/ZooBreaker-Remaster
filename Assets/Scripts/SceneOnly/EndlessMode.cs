using System;
using System.Collections;
using Game;
using Managements;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace SceneOnly
{
    public class EndlessMode : MonoBehaviour
    {
        [SerializeField] private int maxBlockSize;
        [SerializeField] private GameObject startText;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private TextMeshProUGUI nowScoreText;
        
        private Camera _camera;
        private int _nowHighestHeight;
        
        /* Unity API */
        private void Awake()
        {
            _camera = Camera.main;
            StartCoroutine(GameStart());
            StartCoroutine(SendAndUpdateScore());
            StartCoroutine(DynamicCamera());
            
            // Rank Test Sample
            // ValueManager.Instance.EndlessModeHighScore(49849894);
            // ValueManager.Instance.EndlessModeHighScore(419871651);
            // ValueManager.Instance.EndlessModeHighScore(1561651632);
            // ValueManager.Instance.EndlessModeHighScore(21165);
            // ValueManager.Instance.EndlessModeHighScore(4918);
            // ValueManager.Instance.EndlessModeHighScore(89489);
            // ValueManager.Instance.EndlessModeHighScore(149811);
            // ValueManager.Instance.EndlessModeHighScore(149811486);
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
            ValueManager.Instance.CanDropBlock = true;
        }
        private IEnumerator GenerateBlocks()
        {
            // has Y2K38 Problem
            var seed = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            ValueManager.Instance.GameSeed = seed;
            var random = new Random(seed);
            
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
        private IEnumerator SendAndUpdateScore()
        {
            var bestScores = ValueManager.Instance.EndlessModeHighScore();
            while (!ValueManager.Instance.IsGameEnded)
            {
                yield return null;
                bestScoreText.text = ValueManager.Instance.BlockBestHeight > bestScores[0] ? $"{ValueManager.Instance.BlockBestHeight}<color=#FF8047>m</color>" : $"{bestScores[0]}<color=#FF8047>m</color>";
                nowScoreText.text = $"{_nowHighestHeight}<color=#FF8047>m</color>";
            }

            ValueManager.Instance.EndlessModeHighScore(ValueManager.Instance.BlockBestHeight);
        }
        private IEnumerator DynamicCamera()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.25f);
                _nowHighestHeight = Mathf.FloorToInt(Main.Instance.GetHighestBlockHeight());
                ValueManager.Instance.BlockBestHeight = _nowHighestHeight;
            
                // 만약 엔드리스 모드 이고 카메라 높이가 블록의 높이보다 낮다고 최대 높이보다 높으면 카메라 높이를 블록의 높이로 변경
                if (_camera.transform.position.y < ValueManager.Instance.BlockBestHeight && ValueManager.Instance.GameMode == 2 && !ValueManager.Instance.IsGameEnded)
                {
                    // 카메라 높이 변경
                    Main.Instance.StartCoroutine(Main.Instance.CameraMove(ValueManager.Instance.BlockBestHeight + 0.5f));
                }
            }
        }
    }
}
