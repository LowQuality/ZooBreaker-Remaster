
using UnityEngine;

namespace DevTools
{
    public class DevelopmentBuildOnly : MonoBehaviour
    {
#if (!DEV)
        public void Awake()
        {
            Destroy(gameObject);
        }
#endif
    }
}