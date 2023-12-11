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

            // 숨겨진 블록 개수 반영
            if (queuedBlocks.Count > 6)
            {
                switch (ValueManager.Instance.GameMode)
                {
                    case 1:
                        hiddenBlockObject.SetActive(true);
                        hiddenBlockCount.text = $"+{queuedBlocks.Count - 6}";
                        break;
                    case 2:
                        hiddenBlockObject.SetActive(true);
                        hiddenBlockCount.text = "";
                        break;
                }
            }
            else
            {
                hiddenBlockObject.SetActive(false);
            }
            
            // TODO:블록 큐 위치 반영
            

            yield return null;
        }
    }
}
