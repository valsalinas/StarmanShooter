using System.Collections;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    private bool _isCameraShaking = false; 
    private float _timeShaking = 0.3f; 
    private float _shakeStrength = 0.2f;

    public void StartCameraShake()
    {
        StartCoroutine(CameraShakeRoutine());
    }
    IEnumerator CameraShakeRoutine()
    {
        Vector3 defaultPosition = transform.position; 
        float _timeSpan = 0.0f;
        _isCameraShaking = true;

        while (_timeSpan < _timeShaking)
        {
            float xPosition = Random.Range(-1.0f, 1.0f) * _shakeStrength;
            float yPosition = Random.Range(-1.0f, 1.0f) * _shakeStrength;
            transform.position = new Vector3(xPosition, yPosition, -10.0f);
            _timeSpan += Time.deltaTime;
            yield return null;
        }

        _isCameraShaking = false;
        transform.position = defaultPosition;
    }
}
