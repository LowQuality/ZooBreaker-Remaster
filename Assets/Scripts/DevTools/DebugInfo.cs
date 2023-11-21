using UnityEngine;
using System.Collections;

namespace DevTools
{
    public class DebugInfo : MonoBehaviour
    {
        private float _count;
        private float _deltaTime;

        private IEnumerator Start()
        {
            GUI.depth = 2;
            while (true)
            {
                _count = 1f / Time.unscaledDeltaTime;
                _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
            // ReSharper disable once IteratorNeverReturns
        }
    
        private void OnGUI()
        {
            var location = new Rect(5, 5, 165, 50);
            var text = $"Version: {Application.version}\n" +
                            $"FPS: {Mathf.Round(_count)} ({_deltaTime * 1000.0f:0.0} ms)";
            Texture black = Texture2D.linearGrayTexture;
            GUI.DrawTexture(location, black, ScaleMode.StretchToFill);
            GUI.color = Color.black;
            GUI.skin.label.fontSize = 18;
            GUI.Label(location, text);
        }
    }
}