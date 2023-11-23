using Managements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sliders
{
    public class Sounds : MonoBehaviour, IPointerUpHandler
    {
        public void OnPointerUp(PointerEventData eventData)
        {
            Settings.Instance.Save();
        }
    }
}