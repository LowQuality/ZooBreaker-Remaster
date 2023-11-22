using System;
using System.Collections;
using Management;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    [SerializeField] private Camera _camera;


    /* Unity API */
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    /* Coroutines */
    public IEnumerator CameraMove(float y, float cameraSpeed = 1f)
    {
        // var finalY = transform.position.y + addY;
        while (Math.Abs(transform.position.y - y) > 0.001)
        {
            var position = transform.position;
            var movePosition = new Vector3(0, y, 0);
            
            transform.position = Vector3.Lerp(position, movePosition, Time.deltaTime * cameraSpeed);
            yield return null;
        }
        
        transform.position = new Vector3(0, y, 0);
    }
    
    public IEnumerator GameOverDetect(float targetY)
    {
        ValueManager.Instance.IsPlaying = false;
        StartCoroutine(CameraMove(targetY, 5));
        yield return new WaitForSeconds(0.5f);
        SeManager.Instance.Play2Shot(4);
        FadeManager.Instance.WhiteFXFadeOut(0.1f);
        yield return new WaitForSeconds(0.35f);
        FadeManager.Instance.FadeIn(0.1f);
        
        yield return new WaitForSeconds(5f);
        SeManager.Instance.Play2Shot(2);
        
    }
}
