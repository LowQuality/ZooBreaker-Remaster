using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Utils;

namespace Management
{
    public class ValueManager : MonoBehaviour
    {
        [Description ("ValueManager 클래스 내에서 정적함수가 아닌 함수를 호출합니다.")] 
        public static ValueManager Instance;

        /* Private Variables */
        private bool _isGamePaused;
        private bool _isStoryWatched;
        private int _clearedStage;
        private int _lastLevelLocated;
        
        /* Public Variables */
        [Description("게임이 일시정지되었는지 여부를 나타냅니다.")] 
        public bool IsGamePaused
        {
            get => _isGamePaused;
            set
            {
                _isGamePaused = value;
                Save();
            }
        }

        [Description("게임 스토리를 봤는지 여부를 나타냅니다.")]
        public bool IsStoryWatched
        {
            get => _isStoryWatched;
            set
            {
                _isStoryWatched = value;
                Save();
            }
        }

        [Description("게임에서 클리어한 스테이지의 수를 나타냅니다.")]
        public int ClearedStage
        {
            get => _clearedStage;
            set
            {
                _clearedStage = value;
                Save();
            }
        }

        [Description("마지막으로 있던 레벨을 나타냅니다. (1 : 동물원, 2 : 도시, 3 : 숲, 4 : 바다)")]
        public int LastLevelLocated
        {
            get => _lastLevelLocated;
            set
            {
                _lastLevelLocated = value;
                Save();
            }
        }


        /* !! Dont Touch !! */
        /* Unity API */
        private void Awake()
        {
            Load();
            if (!Instance)
            {
                Instance = this;
            }
        }

        /* Functions */
        private void Save()
        {
            // 클래스의 필드들 모두 가져오기
            var fields = typeof(ValueManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            try
            {
                // 메모리에 저장된 데이터들을 문자열로 변환
                var saveData = fields.Aggregate("", (current, field) => current + $"{field.Name}:{field.FieldType}:{field.GetValue(this)},");
                saveData = saveData.Remove(saveData.Length - 1);
            
                // 세이브할 폴더가 없으면 새로운 폴더 생성
                if (!Directory.Exists(Application.persistentDataPath + "/SaveFiles"))
                    Directory.CreateDirectory(Application.persistentDataPath + "/SaveFiles");
            
                // 암호화
                saveData = Crypto.EncryptionAes("Ddh912!#jCh9H3)5dK8@h^fb&d6hN2M&", saveData);
                saveData = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(saveData));
            
                // 파일에 저장
                File.WriteAllText(Application.persistentDataPath + "/SaveFiles/Save.dat", saveData);
            
                Debug.Log($"세이브 파일 저장 완료 | 세이브 데이터 : {saveData}");
            } catch (Exception e)
            {
                Debug.Log($"세이브 파일을 저장할 수 없습니다. {e}");
            }
        }
        private void Load()
        {
            // 세이브할 폴더가 없으면 새로운 폴더 생성
            if (!Directory.Exists(Application.persistentDataPath + "/SaveFiles"))
                Directory.CreateDirectory(Application.persistentDataPath + "/SaveFiles");

            try
            {
                // 파일에서 불러오기
                var saveData = File.ReadAllText(Application.persistentDataPath + "/SaveFiles/Save.dat");
            
                // 복호화
                saveData = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(saveData));
                saveData = Crypto.DecryptionAes("Ddh912!#jCh9H3)5dK8@h^fb&d6hN2M&", saveData);
            
                // 필드들 모두 가져오기
                var saveDateSplit = saveData.Split(',');
            
                // 필드들에 저장된 데이터들을 메모리에 로드
                foreach (var field in saveDateSplit)
                {
                    var fieldSplit = field.Split(':');
                    var fieldName = fieldSplit[0];
                    var fieldType = fieldSplit[1];
                    var fieldValue = fieldSplit[2];
                
                    var fieldInfo = typeof(ValueManager).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                    switch (fieldType)
                    {
                        case "System.Boolean":
                            if (fieldInfo != null) fieldInfo.SetValue(this, bool.Parse(fieldValue));
                            break;
                        case "System.Int32":
                            if (fieldInfo != null) fieldInfo.SetValue(this, int.Parse(fieldValue));
                            break;
                    }
                }
            
                Debug.Log($"세이브 파일 불러오기 완료! | 세이브 데이터 : {saveData}");
            } catch (Exception e)
            {
                Debug.Log($"세이브 파일을 볼러올 수 없습니다. {e}");
            } 
        }
        public void ResetData()
        {
            try
            {
                File.Delete(Application.persistentDataPath + "/SaveFiles/Save.dat");
                Debug.Log("세이브 파일 삭제 완료!");
                
                // 클래스의 필드들 모두 가져오기
                var fields = typeof(ValueManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                
                // 메모리에 저장된 데이터를 초기값으로 설정
                foreach (var field in fields)
                {
                    field.SetValue(this, default);
                }
                Debug.Log("메모리 초기화 완료!");
            } catch (Exception e)
            {
                Debug.Log($"세이브 파일을 삭제할 수 없습니다. {e}");
            }
        }
        public string DebugInfo()
        {
            if (!Debug.isDebugBuild) return null;
            // 클래스의 필드들 모두 가져오기
            var fields = typeof(ValueManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            
            // 메모리에 저장된 데이터들을 문자열로 변환
            var saveData = fields.Aggregate("", (current, field) => current + $"{field.Name}: {field.GetValue(this)}\n");
            saveData = saveData.Remove(saveData.Length - 1);
            
            return saveData;
        }
    }
}