using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Management
{
    public class Settings : MonoBehaviour
    {
        public static Settings Instance;

        /* Unity API */
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        
        /* Private Variables */
        private int _bgmVolume = 100;
        private int _seVolume = 100;
        
        /* Public Variables */
        [Description("BGM의 볼륨을 나타냅니다.")]
        public int BgmVolume
        {
            get => _bgmVolume;
            set
            {
                _bgmVolume = value;
                Save();
            }
        }
        
        [Description("효과음의 볼륨을 나타냅니다.")]
        public int SeVolume
        {
            get => _seVolume;
            set
            {
                _seVolume = value;
                Save();
            }
        }
        
        /* !! Dont Touch !! */
        /* Functions */
        private void Save()
        {
            // TODO
        }
        private void Load()
        {
            // TODO
        }
        public string DebugInfo()
        {
            if (!Debug.isDebugBuild) return null;
            // 클래스의 필드들 모두 가져오기
            var fields = typeof(Settings).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            
            // 메모리에 저장된 데이터들을 문자열로 변환
            return fields.Aggregate("", (current, field) => current + $"{field.Name}: {field.GetValue(this)}\n");
        }
        /* !! Dont Touch !! */
    }
}