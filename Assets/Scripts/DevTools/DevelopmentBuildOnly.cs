using UnityEngine;

namespace DevTools
{
    public class DevelopmentBuildOnly : MonoBehaviour
    {
        public void Awake()
        {
            if (!Debug.isDebugBuild)
            {
                Destroy(gameObject);
            }
        }
    }
}