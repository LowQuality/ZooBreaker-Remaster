using System.ComponentModel;
using UnityEngine;

namespace Management
{
    public class ValueManager : MonoBehaviour
    {
        [Description ("ValueManager 클래스 내에서 정적화되지 않은 함수를 호출합니다.")] public static ValueManager Instance;

        [Description("게임이 일시정지되었는지 여부를 나타냅니다.")] public bool IsGamePaused { get; set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
        }
    }
}