using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Managements
{
    public class ValueManager : MonoBehaviour
    {
        [Description ("ValueManager 클래스 내에서 정적함수가 아닌 함수를 호출합니다.")] 
        public static ValueManager Instance;

        /* Private Variables */
        [SerializeField] private List<string> isNotInGameScene = new();
        [SerializeField] private GameObject[] blocks;

        // _GLO_ : 글로벌 변수 (게임이 종료되어도 초기화되지 않음 + 세이브 파일에 저장)
        private bool _GLO_isStoryWatched;
        private int _GLO_latestClearedStageID;
        private int _GLO_lastLevelLocated;
        private List<int> _GLO_endlessModeHighScore = new();

        // _LOC_ : 로컬 변수 (게임이 종료되면 초기화)
        private bool _LOC_isPlaying;
        private bool _LOC_isGameEnded;
        private bool _LOC_isGamePaused;
        private bool _LOC_isGeneratingBlock;
        private int _LOC_gameMode;
        private int _LOC_gameSeed;
        private List<string> _LOC_queuedBlocks = new();
        

        /* Public Variables */
        [Description("게임 스토리를 봤는지 여부를 나타냅니다.")]
        public bool IsStoryWatched
        {
            get => _GLO_isStoryWatched;
            set
            {
                _GLO_isStoryWatched = value;
                Save();
            }
        }
        
        [Description("게임에서 클리어한 스테이지의 수를 나타냅니다.")]
        public int LatestClearedStageID
        {
            get => _GLO_latestClearedStageID;
            set
            {
                _GLO_latestClearedStageID = value;
                Save();
            }
        }

        [Description("마지막으로 있던 레벨을 나타냅니다. (1 : 동물원, 2 : 도시, 3 : 숲, 4 : 바다)")]
        public int LastLevelLocated
        {
            get => _GLO_lastLevelLocated;
            set
            {
                _GLO_lastLevelLocated = value;
                Save();
            }
        }
        
        [Description("엔드리스 모드의 점수를 추가하거나 최고 6개의 점수를 가져옵니다.")]
        public List<int> EndlessModeHighScore(int score = -1)
        {
            if (score != -1)
            {
                _GLO_endlessModeHighScore ??= new List<int>();
                _GLO_endlessModeHighScore.Add(score);
                // 최고 점수가 6개 이상이면 6개만 저장후 정렬
                if (_GLO_endlessModeHighScore.Count >= 6)
                {
                    _GLO_endlessModeHighScore.Sort();
                    _GLO_endlessModeHighScore.Reverse();
                    _GLO_endlessModeHighScore.RemoveRange(6, _GLO_endlessModeHighScore.Count - 6);
                }

                Save();
            }
            else
            {
                return _GLO_endlessModeHighScore;
            }
            return null;
        }
        
        [Description("게임이 진행중인지 여부를 나타냅니다.")]
        public bool IsPlaying
        {
            get => _LOC_isPlaying;
            set => _LOC_isPlaying = value;
        }
        
        [Description("게임이 끝났는지 여부를 나타냅니다.")]
        public bool IsGameEnded
        {
            get => _LOC_isGamePaused;
            set => _LOC_isGamePaused = value;
        }
        
        [Description("게임이 일시정지되었는지 여부를 나타냅니다.")] 
        public bool IsGamePaused
        {
            get => _LOC_isGamePaused;
            set => _LOC_isGamePaused = value;
        }
        
        [Description("블록이 생성중인지 여부를 나타냅니다.")]
        public bool IsGeneratingBlock
        {
            get => _LOC_isGeneratingBlock;
            set => _LOC_isGeneratingBlock = value;
        }
        
        [Description("게임 모드를 나타냅니다. (1 : 일반, 2 : 엔드리스)")]
        public int GameMode
        {
            get => _LOC_gameMode;
            set => _LOC_gameMode = value;
        }
        
        [Description("게임 시드를 나타냅니다.")]
        public int GameSeed
        {
            get => _LOC_gameSeed;
            set => _LOC_gameSeed = value;
        }
        
        [Description("블록 대기열을 추가 하거나 가져옵니다.")]
        public List<string> QueuedBlocks(int id = -1, int size = -1, int rotation = -1)
        {
            _LOC_queuedBlocks ??= new List<string>();
            if (id != -1 && size != -1 && rotation != -1)
            {
                _LOC_queuedBlocks.Add($"{id}/{size}/{rotation}");
                Save();
            }
            else
            {
                return _LOC_queuedBlocks;
            }
            return null;
        }

        /* !! Dont Touch !! */
        /* Unity API */
        private void Awake()
        {
            Load();
            SceneManager.sceneLoaded += OnSceneLoaded;
            if (!Instance)
            {
                Instance = this;
            }
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (isNotInGameScene.Any(ignoredSceneName => scene.name == ignoredSceneName))
            {
                ResetLocalData();
            }
        }

        /* Functions */
        // TODO : 좀더 효율적인 방법 찾기 (All)
        private void Save()
        {
            // 클래스의 필드들 모두 가져오기
            var fields = typeof(ValueManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            // 변수명 앞에 _GLO_가 붙은 변수만 저장
            fields = fields.Where(field => field.Name.StartsWith("_GLO_")).ToArray();

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
                            saveData += $"{field.Name}:{field.FieldType}:{{}}\n";
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
                File.WriteAllText(Application.persistentDataPath + "/PersonalizationFile/save.dat", saveData);
            
                // Debug.Log($"세이브 파일 저장 완료 | 저장된 데이터 보기 \n---\n{saveData}\n---");
                /* 후처리 */
            } catch (Exception e)
            {
                Debug.LogError($"세이브 파일을 저장할 수 없습니다. {e}");
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
                        if (listSplit[0] == "")
                        {
                            fieldInfo?.SetValue(this, null);
                        }
                        else
                        {
                            var list = listSplit.Select(int.Parse).ToList();
                            fieldInfo?.SetValue(this, list);
                        }
                        
                    }
                    else
                    {
                        fieldInfo?.SetValue(this, Convert.ChangeType(fieldValue, Type.GetType(fieldType)!));
                    }
                }
            
                Debug.Log($"세이브 파일 불러오기 완료! | 불러온 데이터 보기 \n---\n{saveData}\n---");
            } catch (Exception e)
            {
                Debug.LogError($"세이브 파일을 볼러올 수 없습니다. {e}");
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
                    // _GLO_나 _LOC_가 붙지 않은경우 다음으로 넘어감
                    if (!field.Name.StartsWith("_GLO_") && !field.Name.StartsWith("_LOC_")) continue;
                    field.SetValue(this, default);
                }
                // Debug.Log("메모리 초기화 완료!");
            } catch (Exception e)
            {
                Debug.LogError($"세이브 파일을 삭제할 수 없습니다. {e}");
            }
        }
        public void ResetLocalData()
        {
            // 클래스의 필드들 모두 가져오기
            var fields = typeof(ValueManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            // 변수명 앞에 _LOC_가 붙은 변수만 가져오기
            fields = fields.Where(field => field.Name.StartsWith("_LOC_")).ToArray();

            try
            {
                // 메모리에 저장된 데이터를 초기값으로 설정
                foreach (var field in fields)
                {
                    field.SetValue(this, default);
                }
                
                Debug.Log("로컬 데이터 초기화 완료!");
            } catch (Exception e)
            {
                Debug.LogError($"로컬 데이터를 초기화할 수 없습니다. {e}");
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
                // _GLO_나 _LOC_가 붙지 않은경우 다음으로 넘어감
                if (!field.Name.StartsWith("_GLO_") && !field.Name.StartsWith("_LOC_")) continue;
                
                // List(int, string) 타입은 따로 처리
                if (field.FieldType == typeof(List<int>))
                {
                    // List<int> 가 비어있으면 다른값 출력
                    if ((List<int>) field.GetValue(this) == null || ((List<int>) field.GetValue(this)).Count == 0)
                    {
                        saveData += $"{field.Name.Replace("_LOC_", "[L] ").Replace("_GLO_", "[G] ")}: {{}}\n";
                    }
                    else
                    {
                        var list = (List<int>) field.GetValue(this);
                        var listString = list.Aggregate("", (current, i) => current + $"{i},");
                        listString = listString.Remove(listString.Length - 1);
                        saveData += $"{field.Name.Replace("_LOC_", "[L] ").Replace("_GLO_", "[G] ")}: {{{listString}}}\n";
                    }
                }
                else if (field.FieldType == typeof(List<string>))
                {
                    // List<string> 가 비어있으면 다른값 출력
                    if ((List<string>) field.GetValue(this) == null || ((List<string>) field.GetValue(this)).Count == 0)
                    {
                        saveData += $"{field.Name.Replace("_LOC_", "[L] ").Replace("_GLO_", "[G] ")}: {{}}\n";
                    }
                    else
                    {
                        var list = (List<string>) field.GetValue(this);
                        var listString = list.Aggregate("", (current, i) => current + $"{i},");
                        listString = listString.Remove(listString.Length - 1);
                        saveData += $"{field.Name.Replace("_LOC_", "[L] ").Replace("_GLO_", "[G] ")}: {{{listString}}}\n";
                    }
                }
                else
                {
                    saveData += $"{field.Name.Replace("_LOC_", "[L] ").Replace("_GLO_", "[G] ")}: {field.GetValue(this)}\n";
                }
                
            }
            saveData = saveData.Remove(saveData.Length - 1);
            
            return saveData;
        }
#endif
        /* !! Dont Touch !! */
    }
}