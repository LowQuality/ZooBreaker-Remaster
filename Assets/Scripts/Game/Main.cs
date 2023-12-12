using System;
using System.Collections;
using Managements;
using TMPro;
using UnityEngine;

namespace Game
{
    public class Main : MonoBehaviour
    {
        public static Main Instance { get; private set; }
        
        [SerializeField] private new Camera camera;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject hiddenBlockObject;
        [SerializeField] private TextMeshProUGUI hiddenBlockCount;
        [SerializeField] private GameObject[] blockQueueLocations;
        [SerializeField] private GameObject[] blockQueuePrefabs;
        [SerializeField] private GameObject[] blockPrefabs;

        /* Unity API */
        private void Start()
        {
            StartCoroutine(RefreshBlockQueue());
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
            if (ValueManager.Instance.IsGameEnded) return;
            ValueManager.Instance.IsGamePaused = true;
            pausePanel.SetActive(true);
        }
        public void MenuButton()
        {
            if (ValueManager.Instance.IsGameEnded) return;
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

        /* Coroutines */
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
            yield return new WaitForSeconds(0.3f);
            FadeManager.Instance.FadeIn(0.1f);
        
            yield return new WaitForSeconds(2.5f);
            SeManager.Instance.Play2Shot(2);
        }
        public IEnumerator RefreshBlockQueue()
        {
            // ValueManager.Instance.IsGeneratingBlock 이 false가 될 때까지 대기
            while (ValueManager.Instance.IsGeneratingBlock)
            {
                yield return null;
            }
            
            var queuedBlocks = ValueManager.Instance.QueuedBlocks();
            
            if (queuedBlocks.Count > 6)
            {
                // 숨겨진 블록 개수 반영
                if (ValueManager.Instance.GameMode == 1)
                {
                    hiddenBlockObject.SetActive(true);
                    hiddenBlockCount.text = $"+{queuedBlocks.Count - 6}";
                }
                else
                {
                    hiddenBlockObject.SetActive(true);
                    hiddenBlockCount.text = "";
                }
                
                // TODO 에니메이션 구현하기
                // 큐에 있는 블록들 모두 제거
                foreach (var block in GameObject.FindGameObjectsWithTag("BlockQueue"))
                {
                    Destroy(block);
                }
            
                var blockInfos = ValueManager.Instance.QueuedBlocks();

                // TODO NowDropBlock에서 오브젝트 위치 조정하기
                // 큐에 있는 블록들 모두 생성
                for (var i = 0; i < 5; i++)
                {
                    var blockInfo = blockInfos[i].Split("/");
                    var id = int.Parse(blockInfo[0]);
                    var size = int.Parse(blockInfo[1]);
                    var rotation = int.Parse(blockInfo[2]);
                    
                    var block = Instantiate(blockQueuePrefabs[id], blockQueueLocations[i].transform);
                    
                    // GameObject에서 TextMeshProUGUI를 가져옴
                    var blockTMP = block.GetComponentInChildren<TextMeshProUGUI>();
                    var blockSize = blockTMP.text.Split("x");
                    
                    // 큐에 있는 블록들의 크기 반영
                    var x = int.Parse(blockSize[0]);
                    var y = int.Parse(blockSize[1]);
                    blockTMP.text = $"{x*size}x{y*size}";
                    
                    // 큐에 있는 블록들의 회전 반영
                    block.transform.rotation = Quaternion.Euler(0, 0, rotation * 90);
                }
            }
            else
            {
                hiddenBlockObject.SetActive(false);
                
                // TODO 에니메이션 구현하기
                // 큐에 있는 블록들 모두 제거
                foreach (var block in GameObject.FindGameObjectsWithTag("BlockQueue"))
                {
                    Destroy(block);
                }
            
                var blockInfos = ValueManager.Instance.QueuedBlocks();

                // TODO NowDropBlock에서 오브젝트 위치 조정하기
                // 큐에 있는 블록들 모두 생성
                for (var i = 0; i < 6; i++)
                {
                    var blockInfo = blockInfos[i].Split("/");
                    var id = int.Parse(blockInfo[0]);
                    var size = int.Parse(blockInfo[1]);
                    var rotation = int.Parse(blockInfo[2]);
                    
                    var block = Instantiate(blockQueuePrefabs[id], blockQueueLocations[i].transform);
                    
                    // GameObject에서 TextMeshProUGUI를 가져옴
                    var blockTMP = block.GetComponentInChildren<TextMeshProUGUI>();
                    var blockText = blockTMP.text.Split();
                    
                    // 큐에 있는 블록들의 크기 반영
                    var x = int.Parse(blockText[0]);
                    var y = int.Parse(blockText[2]);
                    blockTMP.text = $"{x*size}x{y*size}";
                    
                    // 큐에 있는 블록들의 회전 반영
                    block.transform.rotation = Quaternion.Euler(0, 0, rotation * 90);
                }
            }
            

            yield return null;
        }
    }
}
