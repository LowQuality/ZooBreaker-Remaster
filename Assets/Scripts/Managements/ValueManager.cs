using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Utils;

namespace Managements
{
    public class ValueManager : MonoBehaviour
    {
        [Description ("ValueManager 클래스 내에서 정적함수가 아닌 함수를 호출합니다.")] 
        public static ValueManager Instance;

        /* Private Variables */
        private bool _isGamePausedN;
        private bool _isPlayingN;
        
        private bool _isStoryWatched;
        private int _clearedStage;
        private int _lastLevelLocated;
        private List<int> _endlessModeHighScore = new();

        /* Public Variables */
        [Description("게임이 일시정지되었는지 여부를 나타냅니다.")] 
        public bool IsGamePaused
        {
            get => _isGamePausedN;
            set => _isGamePausedN = value;
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
        
        [Description("게임이 진행중인지 여부를 나타냅니다.")]
        public bool IsPlaying
        {
            get => _isPlayingN;
            set => _isPlayingN = value;
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
        
        [Description("엔드리스 모드의 점수를 추가하거나 최고 6개의 점수를 가져옵니다.")]
        public List<int> EndlessModeHighScore(int score = -1)
        {
            if (score != -1)
            {
                if (_endlessModeHighScore == null) _endlessModeHighScore = new List<int>();
                _endlessModeHighScore.Add(score);
                // 최고 점수가 6개 이상이면 6개만 저장후 정렬
                if (_endlessModeHighScore.Count >= 6)
                {
                    _endlessModeHighScore.Sort();
                    _endlessModeHighScore.Reverse();
                    _endlessModeHighScore.RemoveRange(6, _endlessModeHighScore.Count - 6);
                }

                Save();
            }
            else
            {
                return _endlessModeHighScore;
            }
            return null;
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
        // TODO : 좀더 효율적인 방법 찾기 (All)
        private void Save()
        {
            // 클래스의 필드들 모두 가져오기
            var fields = typeof(ValueManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            // 변수명 끝에 N이 붙은 변수는 저장하지 않음
            fields = fields.Where(field => !field.Name.EndsWith("N")).ToArray();

            /* 전처리 */
            try
            {
                var saveData = "";
                foreach (var field in fields)
                {
                    // List 타입은 따로 처리
                    if (field.FieldType == typeof(List<int>))
                    {
                        // List<int> 가 비어있으면 다른값 출력
                        if ((List<int>) field.GetValue(this) == null || ((List<int>) field.GetValue(this)).Count == 0)
                        {
                            saveData += $"{field.Name}:{field.FieldType}:{{0, 0, 0, 0, 0, 0}}\n";
                        }
                        else
                        {
                            var list = (List<int>) field.GetValue(this);
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
                    
                // 암호화
                saveData = Crypto.EncryptionAes("Ddh912!#jCh9H3)5dK8@h^fb&d6hN2M&", saveData);
                saveData = Convert.ToBase64String(Encoding.UTF8.GetBytes(saveData));
            
                // 파일에 저장
                File.WriteAllText(Application.persistentDataPath + "/SaveFiles/save.dat", saveData);
            
                // Debug.Log($"세이브 파일 저장 완료 | 저장된 데이터 보기 \n---\n{saveData}\n---");
                /* 후처리 */
            } catch (Exception e)
            {
                Debug.Log($"세이브 파일을 저장할 수 없습니다. {e}");
            }
        }
        private void Load()
        {
            try
            {
                // 파일에서 불러오기
                var saveData = File.ReadAllText(Application.persistentDataPath + "/PersonalizationFile/save.dat");
            
                // 복호화
                saveData = Encoding.UTF8.GetString(Convert.FromBase64String(saveData));
                saveData = Crypto.DecryptionAes("Ddh912!#jCh9H3)5dK8@h^fb&d6hN2M&", saveData);
            
                // 필드들 모두 가져오기
                var saveDateSplit = saveData.Split('\n');
            
                // 필드들에 저장된 데이터들을 메모리에 로드
                foreach (var field in saveDateSplit)
                {
                    var fieldSplit = field.Split(':');
                    var fieldName = fieldSplit[0];
                    var fieldType = fieldSplit[1];
                    var fieldValue = fieldSplit[2];
                
                    var fieldInfo = typeof(ValueManager).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    // List 타입은 따로 처리
                    if (fieldType.Contains("System.Collections.Generic.List"))
                    {
                        var listSplit = fieldValue.Replace("{", "").Replace("}", "").Split(',');
                        var list = listSplit.Select(int.Parse).ToList();
                        fieldInfo?.SetValue(this, list);
                    }
                    else
                    {
                        fieldInfo?.SetValue(this, Convert.ChangeType(fieldValue, Type.GetType(fieldType)!));
                    }
                }
            
                Debug.Log($"세이브 파일 불러오기 완료! | 불러온 데이터 보기 \n---\n{saveData}\n---");
            } catch (Exception e)
            {
                Debug.Log($"세이브 파일을 볼러올 수 없습니다. {e}");
            }
        }
        public void ResetData()
        {
            try
            {
                File.Delete(Application.persistentDataPath + "/PersonalizationFile/save.dat");
                Debug.Log("세이브 파일 삭제 완료!");
                
                // 클래스의 필드들 모두 가져오기
                var fields = typeof(ValueManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                
                // 메모리에 저장된 데이터를 초기값으로 설정
                foreach (var field in fields)
                {
                    field.SetValue(this, default);
                }
                // Debug.Log("메모리 초기화 완료!");
            } catch (Exception e)
            {
                Debug.Log($"세이브 파일을 삭제할 수 없습니다. {e}");
            }
        }
#if (DEV)
        public string DebugInfo()
        {
            if (!Debug.isDebugBuild) return null;
            // 클래스의 필드들 모두 가져오기
            var fields = typeof(ValueManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            
            // 메모리에 저장된 데이터들을 문자열로 변환
            var saveData = "";
            foreach (var field in fields)
            {
                // List 타입은 따로 처리
                if (field.FieldType == typeof(List<int>))
                {
                    // List<int> 가 비어있으면 다른값 출력
                    if ((List<int>) field.GetValue(this) == null || ((List<int>) field.GetValue(this)).Count == 0)
                    {
                        saveData += $"{field.Name}: {{0, 0, 0, 0, 0, 0}}\n";
                    }
                    else
                    {
                        var list = (List<int>) field.GetValue(this);
                        var listString = list.Aggregate("", (current, i) => current + $"{i},");
                        listString = listString.Remove(listString.Length - 1);
                        saveData += $"{field.Name}: {{{listString}}}\n";
                    }
                }
                else
                {
                    saveData += $"{field.Name}: {field.GetValue(this)}\n";
                }
            }
            saveData = saveData.Remove(saveData.Length - 1);
            
            return saveData;
        }
#endif
        /* !! Dont Touch !! */
    }
}