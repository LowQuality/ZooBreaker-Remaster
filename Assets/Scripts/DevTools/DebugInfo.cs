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
                // 안드로이드 디버그 대응
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    _showDebugInfo = !_showDebugInfo;
                    Debug.Log($"Show Debug Info: {_showDebugInfo}");
                }
                
                if (Input.GetKey(KeyCode.F5))
                {
                    _showVariables = !_showVariables;
                    Debug.Log($"Show Variables: {_showVariables}");
                }
                
                if (Input.GetKey(KeyCode.F6))
                {
                    // 1/4, 1/2, 1, 2, 4 배율
                    _guiScale = _guiScale switch
                    {
                        0.25f => 0.5f,
                        0.5f => 1f,
                        1f => 2f,
                        2f => 4f,
                        4f => 0.25f,
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
        }
    
        private void OnGUI()
        {
            if (!_showDebugInfo) return;
            var values = _showVariables ? ValueManager.Instance.DebugInfo() : null;
            var text = $"Version: {Application.version}\n" +
                       $"FPS: {Mathf.Round(_count)} ({_deltaTime * 1000.0f:0.0} ms) | {FPSUpdateRate}s\n" +
                       $"\n-- Variables(Show: {_showVariables}) --\n" +
                       $"{values}";
            var count = text.Split('\n').Length - 1;

            var location = new Rect(5, 5, 250 * _guiScale, count * 25 * _guiScale);
            Texture black = Texture2D.linearGrayTexture;
            GUI.DrawTexture(location, black, ScaleMode.StretchToFill);
            GUI.color = Color.black;
            GUI.skin.label.fontSize = (int)(18 * _guiScale);
            GUI.Label(location, text);
        }
    }
}