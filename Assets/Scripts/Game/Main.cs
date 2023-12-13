using System;
using System.Collections;
using Managements;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class Main : MonoBehaviour
    {
        public static Main Instance { get; private set; }
        
        [SerializeField] private new Camera camera;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject retryPanel;
        [SerializeField] private GameObject hiddenBlockObject;
        [SerializeField] private GameObject gameOverText;
        [SerializeField] private TextMeshProUGUI hiddenBlockCount;
        [SerializeField] private GameObject[] blockQueueLocations;
        [SerializeField] private GameObject[] blockQueuePrefabs;
        [SerializeField] private GameObject[] blockPrefabs;

        /* Unity API */
        private void Start()
        {
            StartCoroutine(InitQueue());
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    
        /* Functions */
        public void PauseButton()
        {
            if (ValueManager.Instance.IsGameEnded || !ValueManager.Instance.IsPlaying) return;
            ValueManager.Instance.IsGamePaused = true;
            pausePanel.SetActive(true);
        }
        public void MenuButton()
        {
            if (ValueManager.Instance.IsGameEnded || !ValueManager.Instance.IsPlaying) return;
            ValueManager.Instance.IsGamePaused = true;
            menuPanel.SetActive(true);
        }
        public void ResumeButton()
        {
            SeManager.Instance.Play2Shot(7);
            pausePanel.SetActive(false);
            menuPanel.SetActive(false);
            ValueManager.Instance.IsGamePaused = false;
        }

        public void RetryConfirmButton()
        {
            StartCoroutine(RetryConfirm());
        }
        // MenuOptions
        public void MainMenuButton()
        {
            StartCoroutine(MainMenu());
        }

        /* Coroutines */
        private IEnumerator InitQueue()
        {
            // ValueManager.Instance.IsGeneratingBlock 이 false가 될 때까지 대기
            while (ValueManager.Instance.IsGeneratingBlock)
            {
                yield return null;
            }
            
            var queuedBlocks = ValueManager.Instance.QueuedBlocks();

            int createQueueBlockCount;
            if (queuedBlocks.Count > 6)
            {
                if (ValueManager.Instance.GameMode == 1)
                {
                    // 숨겨진 블록 개수 반영
                    hiddenBlockObject.SetActive(true);
                    hiddenBlockCount.text = $"+{queuedBlocks.Count - 6}";
                }
                else
                {
                    // 엔드리스 모드
                    hiddenBlockObject.SetActive(true);
                    hiddenBlockCount.text = "";
                }
                createQueueBlockCount = 5;
            }
            else
            {
                hiddenBlockObject.SetActive(false);
                createQueueBlockCount = 6;
            }
            
            // 큐에 있는 블록들 모두 제거
            foreach (var block in GameObject.FindGameObjectsWithTag("BlockQueue"))
            {
                Destroy(block);
            }
            
            var blockInfos = ValueManager.Instance.QueuedBlocks();

            // 큐에 있는 블록들 모두 생성
            for (var i = 0; i < createQueueBlockCount; i++)
            {
                var blockInfo = blockInfos[i].Split("/");
                var id = int.Parse(blockInfo[0]);
                var size = int.Parse(blockInfo[1]);
                var rotation = int.Parse(blockInfo[2]);
                    
                var block = Instantiate(blockQueuePrefabs[id], blockQueueLocations[i].transform);
                    
                // GameObject에서 TextMeshProUGUI를 가져옴
                var blockTMP = block.GetComponentInChildren<TextMeshProUGUI>();
                var blockSize = blockTMP.text.Split("x");
                
                if (i == 0)
                {
                    // NowDropBlock 오브젝트 위치 조정
                    block.transform.GetChild(0).localPosition = Vector3.zero;
                    block.transform.GetChild(1).localPosition = Vector3.zero;
                    
                    // NowDropBlock 투명도 조정
                    var blockSprite = block.transform.GetChild(0).GetComponent<Image>();
                    var blockSpriteColor = blockSprite.color;
                    blockSpriteColor.a = 0.49f;
                    blockSprite.color = blockSpriteColor;
                }
                    
                // 큐에 있는 블록들의 크기 반영
                var x = int.Parse(blockSize[0]);
                var y = int.Parse(blockSize[1]);
                blockTMP.text = $"{x*size}x{y*size}";
                    
                // 큐에 있는 블록들의 회전 반영
                block.transform.rotation = Quaternion.Euler(0, 0, rotation * 90);
            }

            yield return null;
        }
        public IEnumerator CameraMove(float y, float cameraSpeed = 1f)
        {
            // var finalY = transform.position.y + addY;
            while (Math.Abs(transform.position.y - y) > 0.001)
            {
                var position = transform.position;
                var movePosition = new Vector3(0, y, 0);
            
                transform.position = Vector3.Lerp(position, movePosition, Time.deltaTime * cameraSpeed);
                yield return null;
            }
        
            transform.position = new Vector3(0, y, 0);
        }
        public IEnumerator GameOverDetect(float targetY)
        {
            ValueManager.Instance.IsGameEnded = true;

            // targetY가 화면에 보이는지 확인
            var targetPos = camera.WorldToViewportPoint(new Vector3(0, targetY, 0));
            if (targetPos.y < 0) StartCoroutine(CameraMove(targetY, 5));
            yield return new WaitForSeconds(0.5f);
            SeManager.Instance.Play2Shot(4);
            FadeManager.Instance.WhiteFXFadeOut(0.1f);
            yield return new WaitForSeconds(0.2f);
            gameOverText.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            FadeManager.Instance.FadeIn(0.1f);
            
            yield return new WaitForSeconds(2.0f);
            gameOverText.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            
            SeManager.Instance.Play2Shot(2);
            retryPanel.SetActive(true);
        }
        public IEnumerator RemoveQueueAndUpdate()
        {
            // ValueManager.Instance.IsGeneratingBlock 이 false가 될 때까지 대기
            while (ValueManager.Instance.IsGeneratingBlock)
            {
                yield return null;
            }
            
            ValueManager.Instance.QueuedBlocks().RemoveAt(0);
            var queuedBlocks = ValueManager.Instance.QueuedBlocks();
            
            int createQueueBlockCount;
            if (queuedBlocks.Count > 6)
            {
                if (ValueManager.Instance.GameMode == 1)
                {
                    // 숨겨진 블록 개수 반영
                    hiddenBlockObject.SetActive(true);
                    hiddenBlockCount.text = $"+{queuedBlocks.Count - 6}";
                }
                else
                {
                    // 엔드리스 모드
                    hiddenBlockObject.SetActive(true);
                    hiddenBlockCount.text = "";
                }
                createQueueBlockCount = 5;
            }
            else
            {
                hiddenBlockObject.SetActive(false);
                createQueueBlockCount = 6;
            }
            
            // NowDropBlock에 있던 오브젝트 제거
            Destroy(blockQueueLocations[0].transform.GetChild(0).gameObject);
            
            // TODO 6 -> 5, 5-> 4, 4 -> 3, 3 -> 2, 2 -> 1, 1 -> NowDropBlock 위치로 이동 (에니메이션)

            yield return null;
        }
        // ButtonCoroutines
        private static IEnumerator RetryConfirm()
        {
            SeManager.Instance.Play2Shot(7);
            FadeManager.Instance.BlackFXFadeOut(0.1f);
            yield return new WaitForSeconds(1f);
            ValueManager.Instance.ResetLocalData();
            SceneManager.LoadScene("EndlessMode");
        }
        // MenuOptions
        private static IEnumerator MainMenu()
        {
            SeManager.Instance.Play2Shot(7);
            FadeManager.Instance.BlackFXFadeOut(0.1f);
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("MainMenu");
        }
    }
}
