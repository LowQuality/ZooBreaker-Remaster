using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Managements
{
    public class Settings : MonoBehaviour
    {
        public static Settings Instance;

        /* Unity API */
        private void Awake()
        {
            // 세이브할 폴더가 없으면 새로운 폴더 생성
            if (!Directory.Exists(Application.persistentDataPath + "/PersonalizationFile"))
                Directory.CreateDirectory(Application.persistentDataPath + "/PersonalizationFile");
            Load();
            
            if (Instance == null)
            {
                Instance = this;
            }
        }

        /* Private Variables */
        private int _bgmVolume;
        private int _seVolume;
        
        /* Public Variables */
        [Description("BGM의 볼륨을 나타냅니다.")]
        public int BgmVolume
        {
            get => _bgmVolume;
            set => _bgmVolume = value;
        }
        
        [Description("효과음의 볼륨을 나타냅니다.")]
        public int SeVolume
        {
            get => _seVolume;
            set => _seVolume = value;
        }
        
        /* !! Dont Touch !! */
        /* Functions */
        // TODO : 좀더 효율적인 방법 찾기 (All)
        public void Save()
        {
            // 클래스의 필드들 모두 가져오기
            var fields = typeof(Settings).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            /* 전처리 */
            try
            {
                var saveData = "";
                foreach (var field in fields)
                {
                    // List 타입은 따로 처리
                    if (field.FieldType == typeof(List<Type>))
                    {
                        // List<Type> 가 비어있으면 다른값 출력
                        if ((List<Type>) field.GetValue(this) == null || ((List<Type>) field.GetValue(this)).Count == 0)
                        {
                            saveData += $"{field.Name}:{field.FieldType}:{{}}\n";
                        }
                        else
                        {
                            var list = (List<Type>) field.GetValue(this);
                            var listString = list.Aggregate("", (current, i) => current + $"{i},");
                            listString = listString.Remove(listString.Length - 1);
                            saveData += $"{field.Name}:{field.FieldType}:{{{listString}}}\n";
                        }
                    }
                    else
                    {
                        saveData += $"{field.Name}:{field.FieldType}:{field.GetValue(this)}\n";
                    }
                }
                /* 전처리 */
                
                /* 후처리 */
                saveData = saveData.Remove(saveData.Length - 1);
            
                // 파일에 저장
                File.WriteAllText(Application.persistentDataPath + "/PersonalizationFile/config.dat", saveData);
            
                // Debug.Log($"설정 파일 저장 완료 | 저장된 데이터 보기 \n---\n{saveData}\n---");
                /* 후처리 */
            } catch (Exception e)
            {
                Debug.LogError($"설정 파일을 저장할 수 없습니다. {e}");
            }
        }
        private void Load()
        {
            try
            {
                // 파일에서 불러오기
                var saveData = File.ReadAllText(Application.persistentDataPath + "/PersonalizationFile/config.dat");
            
                // 필드들 모두 가져오기
                var saveDateSplit = saveData.Split('\n');
            
                // 필드들에 저장된 데이터들을 메모리에 로드
                foreach (var field in saveDateSplit)
                {
                    var fieldSplit = field.Split(':');
                    var fieldName = fieldSplit[0];
                    var fieldType = fieldSplit[1];
                    var fieldValue = fieldSplit[2];
                
                    var fieldInfo = typeof(Settings).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    // List<Type> 타입은 따로 처리
                    if (fieldType.Contains("System.Collections.Generic.List"))
                    {
                        var listSplit = fieldValue.Replace("{", "").Replace("}", "").Split(',');
                        var list = listSplit.Select(Type.GetType).ToList();
                        fieldInfo?.SetValue(this, list);
                    }
                    else
                    {
                        fieldInfo?.SetValue(this, Convert.ChangeType(fieldValue, Type.GetType(fieldType)!));
                    }
                    
                }
            
                Debug.Log($"설정 파일 불러오기 완료! | 불러온 데이터 보기 \n---\n{saveData}\n---");
            } catch (Exception e)
            {
                if (!File.Exists(Application.persistentDataPath + "/PersonalizationFile/config.dat"))
                {
                    Debug.LogWarning("설정 파일이 없습니다. 기본값으로 초기화합니다.");
                    _bgmVolume = 100;
                    _seVolume = 100;
                    Save();
                }
                else
                {
                    Debug.LogError($"설정 파일을 볼러올 수 없습니다. {e}");
                }
            }
        }
#if (DEV)
        public string DebugInfo()
        {
            // if (!Debug.isDebugBuild) return null;
            // 클래스의 필드들 모두 가져오기
            var fields = typeof(Settings).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            
            // 메모리에 저장된 데이터들을 문자열로 변환
            return fields.Aggregate("", (current, field) => current + $"[G] {field.Name}: {field.GetValue(this)}\n");
        }
#endif
        /* !! Dont Touch !! */
    }
}