using UnityEngine;

namespace DevTools
{
    public class DontDestroy : MonoBehaviour
    {
        private DontDestroy _instance;
        
        private void Awake()
        {
            if (!_instance)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}