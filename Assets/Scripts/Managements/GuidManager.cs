using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Managements
{
    public class GuidManager : MonoBehaviour
    {
#if (UNITY_EDITOR)
        [ReadOnly]
#endif
        [SerializeField]
        internal string mGuid = System.Guid.NewGuid().ToString();
        
        [HideInInspector]
        [SerializeField]
        internal string mLastGuid;
        
        [SerializeField]
        internal bool dynamicGuid;

        internal bool GameStarted;

        public string Guid => mGuid;
        public string LastGuid => mLastGuid;

        private void Awake()
        {
            GameStarted = true;
            if (dynamicGuid)
            {
                mGuid = System.Guid.NewGuid().ToString();
            }
        }

        [ContextMenu("Generate new GUID")]
        internal void GenerateNewGuid()
        {
            mGuid = System.Guid.NewGuid().ToString();
        }

        public static GameObject GetObjectByGuid(string guid)
        {
            var allObjects = FindObjectsOfType<GameObject>();
            return (from obj in allObjects
                let guidManager = obj.GetComponent<GuidManager>()
                where guidManager != null && guidManager.Guid == guid
                select obj).FirstOrDefault();
        }
        
        public static string GetGuidByObject(GameObject obj)
        {
            var guidManager = obj.GetComponent<GuidManager>();
            return guidManager != null ? guidManager.Guid : null;
        }
    }
    
#if (UNITY_EDITOR)
    [CustomEditor(typeof(GuidManager))]
    public class GuidButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Generate new GUID"))
            {
                (target as GuidManager)?.GenerateNewGuid();
            }

            if (GUILayout.Button("Copy GUID"))
            {
                EditorGUIUtility.systemCopyBuffer = (target as GuidManager)?.Guid;
            }

            switch ((target as GuidManager)?.dynamicGuid)
            {
                case true when (target as GuidManager)?.GameStarted == false:
                {
                    if ((target as GuidManager)?.mLastGuid is "" or null) ((GuidManager)target).mLastGuid = (target as GuidManager)?.Guid;
                    ((GuidManager)target).mGuid = "--- Dynamic GUID Mode Enabled ---";
                    break;
                }
                case false when (target as GuidManager)?.GameStarted == false && (target as GuidManager)?.mGuid == "--- Dynamic GUID Mode Enabled ---":
                    ((GuidManager)target).mGuid = (target as GuidManager)?.LastGuid;
                    ((GuidManager)target).mLastGuid = null;
                    break;
            }
        }
    } 
    
    public class ReadOnlyAttribute : PropertyAttribute
    { }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
#endif
}