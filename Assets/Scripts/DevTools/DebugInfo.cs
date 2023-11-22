using UnityEngine;
using System.Collections;
using Management;

namespace DevTools
{
    public class DebugInfo : MonoBehaviour
    {
        private float _count;
        private float _deltaTime;
        private float _guiScale = 1f;
        private const float FPSUpdateRate = 1f;
        private bool _showDebugInfo;
        private bool _showVariables;

        private IEnumerator Start()
        {
            StartCoroutine(DataUpdate());
            GUI.depth = 2;
            while (true)
            {
                // TODO: 예약 (F7 : 이전 페이지 / F8 : 다음 페이지)
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    _showDebugInfo = !_showDebugInfo;
                    Debug.Log($"Show Debug Info: {_showDebugInfo}");
                }
                
                if (Input.GetKeyDown(KeyCode.F5))
                {
                    _showVariables = !_showVariables;
                    Debug.Log($"Show Variables: {_showVariables}");
                }
                
                if (Input.GetKeyDown(KeyCode.F6))
                {
                    // 1/4, 1/2, 1, 2, 4 배율
                    _guiScale = _guiScale switch
                    {
                        0.25f => 0.5f,
                        0.5f => 1f,
                        1f => 2f,
                        2f => 4f,
                        4f => 6f,
                        6f => 8f,
                        8f => 0.25f,
                        _ => 1f
                    };
                    Debug.Log($"GUI Scale: {_guiScale}");
                }
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private IEnumerator DataUpdate()
        {
            while (true)
            {
                _count = 1f / Time.unscaledDeltaTime;
                _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
                yield return new WaitForSeconds(FPSUpdateRate);
            }
            // ReSharper disable once IteratorNeverReturns
        }
    
        private void OnGUI()
        {
            if (!_showDebugInfo) return;
            var values = _showVariables ? $"{Settings.Instance.DebugInfo()}\n{ValueManager.Instance.DebugInfo()}" : "";
            var text = $"Version: {Application.version}\n" +
                       $"FPS: {Mathf.Round(_count)} ({_deltaTime * 1000.0f:0.0} ms) | {FPSUpdateRate}s\n" +
                       $"\n-- Variables(Show: {_showVariables}) --\n" +
                       $"{values}";
            
            var content = new GUIContent(text);
            GUI.skin.label.fontSize = (int)(15 * _guiScale);
            // 화면 크기에 맞춰서 디버그 정보 크기 조절
            var size = GUI.skin.label.CalcSize(content);
            size.x = Mathf.Min(size.x, Screen.width - 75);
            size.y = GUI.skin.label.CalcHeight(content, size.x);
            
            var location = new Rect(5, 5, size.x, size.y);
            Texture black = Texture2D.linearGrayTexture;
            GUI.DrawTexture(location, black, ScaleMode.StretchToFill);
            GUI.color = Color.black;
            
            GUI.Label(location, text);
        }
    }
}