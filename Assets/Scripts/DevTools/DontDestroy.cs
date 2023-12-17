using System.Collections.Generic;
using Managements;
using UnityEngine;

namespace DevTools
{
    public class DontDestroy : MonoBehaviour
    {
        private static readonly List<string> GuidList = new ();
        private void Awake()
        {
            var guid = GuidManager.GetGuidByObject(gameObject);
            if (guid == null)
            {
                Debug.LogError($"해당 게임 오브젝트 '{gameObject.name}'에는 GuidManager가 없기 때문에 DontDestroyOnLoad에 등록 할 수 없습니다!");
                return;
            }
            
            if (GuidList.Contains(guid))
            {
                Destroy(gameObject);
            }
            else
            {
                GuidList.Add(guid);
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}