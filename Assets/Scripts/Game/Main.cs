using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managements;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [SerializeField] private List<GameObjectListWrapper> blockPrefabs;
        [SerializeField] private GameObject blockStore;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private GraphicRaycaster graphicRayCaster;

        private int _queueUpdateCount;
        private bool _catchingBlock;

        private PointerEventData _pointerEventData;
        private GameObject _imagineBlock;

        // 카메라 이동 관련 변수
        private bool _isCameraMoving;
        private float _elapsedCameraLerpTime;
        private float _timeToMoveCamera;
        private Vector3 _startCameraPos;
        private Vector3 _endCameraPos;

        /* Unity API */
        private void Start()
        {
            StartCoroutine(InitQueue());
            StartCoroutine(DetectMouse());
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
            ResetImagineBlock();
            ValueManager.Instance.IsGamePaused = true;
            pausePanel.SetActive(true);
        }
        public void MenuButton()
        {
            if (ValueManager.Instance.IsGameEnded || !ValueManager.Instance.IsPlaying) return;
            ResetImagineBlock();
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

        public void BlockDrop()
        {
            // 게임이 끝나거나 일시정지 상태일 때 블록이 떨어지지 않도록 함
            if (ValueManager.Instance.IsGameEnded || !ValueManager.Instance.IsPlaying || ValueManager.Instance.IsGamePaused || !ValueManager.Instance.CanDropBlock) return;
            
            // 현제 떨어뜨릴 블록의 정보를 가져옴
            var blockInfo = ValueManager.Instance.QueuedBlocks()[0].Split("/");
            var id = int.Parse(blockInfo[0]);
            var size = int.Parse(blockInfo[1]);
            var rotation = int.Parse(blockInfo[2]);
            var style = int.Parse(blockInfo[3]);
            
            // 현제 마우스 위치를 가져옴
            var mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
            
            // 블록을 생성 / 사운드 재생
            SeManager.Instance.Play2Shot(9);
            ValueManager.Instance.CanDropBlock = false;
            var block = Instantiate(blockPrefabs[style].gameObjects[id], mousePosition, Quaternion.Euler(0, 0, rotation * 90), blockStore.transform);
            
            var blockSize = block.transform.localScale;
            blockSize.x *= size * 2;
            blockSize.y *= size * 2;
            block.transform.localScale = blockSize;
            
            block.GetComponent<Rigidbody2D>().mass = size;
            
            StartCoroutine(RemoveQueueAndUpdate());
        }
        public void ResetImagineBlock()
        {
            if (_imagineBlock == null) return;
            Destroy(_imagineBlock);
            _imagineBlock = null;
            blockQueueLocations[0].SetActive(true);
        }
        public int GetHighestBlockHeight()
        {
            var highestBlockHeight = 0;
            for (var i = 0; i < blockStore.transform.childCount; i++)
            {
                var block = blockStore.transform.GetChild(i);

                if (block.CompareTag("Last") && !block.GetComponent<Blocks>().playedCrashSound) continue;
                var blockSprite = block.GetComponent<SpriteRenderer>();
                var blockHeight = Mathf.FloorToInt(block.transform.position.y + blockSprite.bounds.size.y / 2f);
                if (highestBlockHeight < blockHeight) highestBlockHeight = blockHeight;
            }
            return highestBlockHeight;
        }

        /* Coroutines */
        private IEnumerator DetectMouse() 
        {
            while (true)
            {
                yield return null;
                if (ValueManager.Instance.IsGameEnded || !ValueManager.Instance.IsPlaying || ValueManager.Instance.IsGamePaused || !ValueManager.Instance.CanDropBlock) continue;
                
                // 마우스 위치에 특정 오브젝트가 있는지 확인
                var mousePosition = Input.mousePosition;
                _pointerEventData = new PointerEventData(eventSystem)
                {
                    position = mousePosition
                };
                var results = new List<RaycastResult>();
                graphicRayCaster.Raycast(_pointerEventData, results);

                var hit = results.Count(result => result.gameObject.CompareTag("Droppable"));
                var position = new Vector3(camera.ScreenToWorldPoint(Input.mousePosition).x,
                    camera.ScreenToWorldPoint(Input.mousePosition).y, -10);

                if (Input.GetMouseButtonDown(0))
                {
                    if (hit == 0) continue;
                    _catchingBlock = true;
                    
                    blockQueueLocations[0].SetActive(false);
                    
                    // 현제 떨어뜨릴 블록의 정보를 가져옴
                    var blockInfo = ValueManager.Instance.QueuedBlocks()[0].Split("/");
                    var id = int.Parse(blockInfo[0]);
                    var size = int.Parse(blockInfo[1]);
                    var rotation = int.Parse(blockInfo[2]);
                    var style = int.Parse(blockInfo[3]);

                    // imagineBlock이 null이면 imagineBlock을 생성
                    if (_imagineBlock == null)
                    {
                        _imagineBlock = Instantiate(blockPrefabs[style].gameObjects[id], position,
                            Quaternion.Euler(0, 0, rotation * 90));
                        
                        _imagineBlock.transform.SetAsFirstSibling();
                        
                        var imagineBlockSize = _imagineBlock.transform.localScale;
                        imagineBlockSize.x *= size * 2;
                        imagineBlockSize.y *= size * 2;
                        _imagineBlock.transform.localScale = imagineBlockSize;
                        
                        _imagineBlock.GetComponent<Blocks>().enabled = false;
                        _imagineBlock.GetComponent<Collider2D>().enabled = false;
                        
                        var imagineBlockColor = _imagineBlock.GetComponent<SpriteRenderer>().color;
                        imagineBlockColor.a = 0.5f;
                        _imagineBlock.GetComponent<SpriteRenderer>().color = imagineBlockColor;
                    }
                }

                if (Input.GetMouseButton(0))
                {
                    if (_imagineBlock == null) continue;
                    _imagineBlock.transform.position = position;
                }

                if (Input.GetMouseButtonUp(0) && _catchingBlock && _imagineBlock != null)
                {
                    Destroy(_imagineBlock);
                    _imagineBlock = null;
                    blockQueueLocations[0].SetActive(true);
                    
                    if (hit == 1) continue;
                    BlockDrop();
                    _catchingBlock = false;
                }
            }
        }
        public IEnumerator CameraMove(float y, float cameraSpeed = 1f, float duration = 1f)
        {
            if (_endCameraPos.y >= y) yield break;
            // 이미 카메라가 움직이고 있을 때 작동
            if (_isCameraMoving)
            {
                // 카메라 시작 위치를 현제 카메라 위치로 변경
                _startCameraPos = camera.transform.position;
                
                // 카메라 이동 끝 위치를 y로 변경
                _endCameraPos = new Vector3(0, y, 0);
                
                // 시간 초기화
                _elapsedCameraLerpTime = 0f;
                _timeToMoveCamera = 0f;
                
                yield break;
            }
            _isCameraMoving = true;
            
            _startCameraPos = camera.transform.position;
            _endCameraPos = new Vector3(0, y, 0);

            while (_timeToMoveCamera < 1)
            {
                _elapsedCameraLerpTime += Time.deltaTime * cameraSpeed;
                // Math Time! _elapsedCameraLerpTime means x, t means y (Calc : https://www.desmos.com/calculator/g1mjeudvoa)
                _timeToMoveCamera = (_elapsedCameraLerpTime - 1) * (_elapsedCameraLerpTime - 1) * (_elapsedCameraLerpTime - 1) + 1 / duration;
                transform.position = Vector3.Lerp(_startCameraPos, _endCameraPos, _timeToMoveCamera);
                yield return null;
            }
            transform.position = _endCameraPos;
            // 변수 초기화
            _elapsedCameraLerpTime = 0f;
            _timeToMoveCamera = 0f;
            _startCameraPos = Vector3.zero;
            _endCameraPos = Vector3.zero;
            
            _isCameraMoving = false;
        }
        public IEnumerator GameOverDetect(float targetY)
        {
            ValueManager.Instance.IsGameEnded = true;
            
            Destroy(_imagineBlock);
            blockQueueLocations[0].SetActive(true);

            // targetY가 화면에 보이는지 확인
            var targetPos = camera.WorldToViewportPoint(new Vector3(0, targetY, 0));
            if (targetPos.y < 0) StartCoroutine(CameraMove(targetY, 8f));
            yield return new WaitForSeconds(0.5f);
            SeManager.Instance.Play2Shot(4);
            FadeManager.Instance.WhiteFXFadeOut(0.1f);
            yield return new WaitForSeconds(0.1f);
            gameOverText.SetActive(true);
            FadeManager.Instance.FadeIn(0.1f);
            
            yield return new WaitForSeconds(2.0f);
            gameOverText.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            
            SeManager.Instance.Play2Shot(2);
            retryPanel.SetActive(true);
        }

        // UIs
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
                
                // 부모 오브젝트의 width와 height의 값으로 크기 조정
                var queueBlockRectTransform = block.GetComponent<RectTransform>();
                var blockQueueLocationRectTransform = blockQueueLocations[i].GetComponent<RectTransform>();
                
                queueBlockRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, blockQueueLocationRectTransform.rect.width);
                queueBlockRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, blockQueueLocationRectTransform.rect.height);
                    
                // GameObject에서 TextMeshProUGUI를 가져옴
                var blockTMP = block.GetComponentInChildren<TextMeshProUGUI>();
                var blockSize = blockTMP.text.Split("x");
                
                if (i == 0)
                {
                    block.transform.SetAsFirstSibling();
                    
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
        private IEnumerator RemoveQueueAndUpdate()
        {
            // queueUpdateCount가 0일 때까지 대기
            while (_queueUpdateCount != 0)
            {
                yield return null;
            }
            
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
            
            /* 새로운 블록 생성 시작 */
            var blockInfo = queuedBlocks[createQueueBlockCount - 1].Split("/");
            var id = int.Parse(blockInfo[0]);
            var size = int.Parse(blockInfo[1]);
            var rotation = int.Parse(blockInfo[2]);
            
            var blockC = Instantiate(blockQueuePrefabs[id], blockQueueLocations[createQueueBlockCount - 1].transform);
            
            // GameObject에서 TextMeshProUGUI를 가져옴
            var blockTMP = blockC.GetComponentInChildren<TextMeshProUGUI>();
            var blockSize = blockTMP.text.Split("x");
                    
            // 큐에 있는 블록들의 크기 반영
            var x = int.Parse(blockSize[0]);
            var y = int.Parse(blockSize[1]);
            blockTMP.text = $"{x*size}x{y*size}";
                    
            // 큐에 있는 블록들의 회전 반영
            blockC.transform.rotation = Quaternion.Euler(0, 0, rotation * 90);
            /* 새로운 블록 생성 끝 */

            // blockQueueLocations[n]에 있던 오브젝트를 blockQueueLocations[n - 1]으로 이동 1 ~ 5
            for (var i = 1; i < createQueueBlockCount; i++)
            {
                _queueUpdateCount++;
                var block = blockQueueLocations[i].transform.GetChild(0).gameObject;
                
                // block의 부모를 blockQueueLocations[i - 1]으로 변경
                block.transform.SetParent(blockQueueLocations[i - 1].transform);
                
                // 부모 오브젝트의 width와 height의 값으로 크기 조정
                var queueBlockRectTransform = block.GetComponent<RectTransform>();
                var blockQueueLocationRectTransform = blockQueueLocations[i - 1].GetComponent<RectTransform>();
                
                queueBlockRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, blockQueueLocationRectTransform.rect.width);
                queueBlockRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, blockQueueLocationRectTransform.rect.height);

                if (i == 1)
                {
                    block.transform.SetAsFirstSibling();
                    block.transform.GetChild(0).localPosition = Vector3.zero;
                    block.transform.GetChild(1).localPosition = Vector3.zero;
                }
                
                // 큐에 있는 블록들의 위치 반영
                StartCoroutine(MoveBlock(block));
            }
            yield return null;
            
        }
        private IEnumerator MoveBlock(GameObject block, float moveSpeed = 1f)
        {
            var elapsedLerpTime = 0f;

            while (Math.Abs(block.transform.localPosition.x - Vector3.zero.x) > 0.01)
            {
                yield return null;
                try
                {
                    elapsedLerpTime += Time.deltaTime * moveSpeed;
                    if (elapsedLerpTime > 1f) elapsedLerpTime = 1f;
                    block.transform.localPosition = Vector3.Lerp(block.transform.localPosition, Vector3.zero, elapsedLerpTime);
                }
                catch (MissingReferenceException)
                {
                    _queueUpdateCount--;
                    StopCoroutine(MoveBlock(block, moveSpeed));
                }
            }
            block.transform.localPosition = Vector3.zero;
            _queueUpdateCount--;
        }
        // // ButtonCoroutines
        private static IEnumerator RetryConfirm()
        {
            SeManager.Instance.Play2Shot(7);
            FadeManager.Instance.BlackFXFadeOut(0.1f);
            yield return new WaitForSeconds(1f);
            ValueManager.Instance.ResetLocalData();
            SceneManager.LoadScene("EndlessMode");
        }
        // // // MenuOptions
        private static IEnumerator MainMenu()
        {
            SeManager.Instance.Play2Shot(7);
            FadeManager.Instance.BlackFXFadeOut(0.1f);
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("MainMenu");
        }
    }
    
    [Serializable]
    public class GameObjectListWrapper
    {
        public string name;
        public List<GameObject> gameObjects;
    }
    
    [Serializable]
    public class StringListWrapper
    {
        public string name;
        public List<string> strings;
    }
}
