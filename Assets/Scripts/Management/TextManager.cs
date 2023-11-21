using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Management
{
    public class TextManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Transform finishMark;
        [SerializeField] private GameObject ignoreClickObject;
        [Tooltip ("한 글자씩 입력되는 속도를 의미합니다.")] [SerializeField] private float defaultTextSpeed = 0.1f;

        private bool _isTextActive;
        private bool _isRichTextStarted;
        private readonly List<string> _preloadTexts = new ();

        public static TextManager Instance { get; private set; }

        public void Awake()
        {
            if (Instance == null || Instance != this)
            {
                Instance = this;
            }
        }

        private void ResetQueue()
        {
            _isTextActive = false;
            _preloadTexts.Clear();
            finishMark.gameObject.SetActive(false);
            text.text = "";
            ignoreClickObject.SetActive(false);

            StopAllCoroutines();
        }
        
        [Description ("대화 내용을 대기열에 추가합니다.")]
        public void AddQueueText(IEnumerable<string> newTexts)
        {
            if (!_isTextActive)
            {
                ignoreClickObject.SetActive(true);
            }

            _preloadTexts.AddRange(newTexts);
            
            if (!_isTextActive)
            {
                StartCoroutine(SpellText());
            }
        }

        private IEnumerator SpellText()
        {
            var tmp = "";
            _isTextActive = true;
            text.text = "";
            
            foreach (var c in _preloadTexts[0].SelectMany(t => t.ToString().Split("")))
            {
                if (!_isRichTextStarted)
                {
                    if (c == "<")
                    {
                        tmp += c;
                        _isRichTextStarted = true;
                    }
                    else
                    {
                        text.text += c;
                        yield return new WaitForSeconds(defaultTextSpeed);
                    }
                }
                else
                {
                    tmp += c;
                    if (c != ">") continue;
                    
                    text.text += tmp;
                    tmp = "";
                    _isRichTextStarted = false;
                }
            }
            _preloadTexts.RemoveAt(0);

            finishMark.gameObject.SetActive(true);
            StartCoroutine(WaitForClick());
        }

        private IEnumerator WaitForClick()
        {
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
            
            if (_preloadTexts.Count != 0)
            {
                finishMark.gameObject.SetActive(false);
                StartCoroutine(SpellText());
            }
            else
            {
                ResetQueue();
            }
        }
    }
}
