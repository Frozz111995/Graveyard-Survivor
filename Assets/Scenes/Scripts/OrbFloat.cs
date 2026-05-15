using UnityEngine;

public class OrbFloat : MonoBehaviour
{
    [SerializeField] float groundY = 0f;
    [SerializeField] float floatAmplitude = 0.15f;
    [SerializeField] float floatSpeed = 2f;
    [SerializeField] float arcHeight = 1.5f;
    [SerializeField] float arcDuration = 0.4f;
    [SerializeField] float scatterRadius = 1.2f;

    float _baseY;
    float _timeOffset;
    Vector3 _startPos;
    Vector3 _targetPos;
    float _arcTime;
    bool _landing;

    void OnEnable()
    {
        _startPos = transform.position;
        
        // случайная точка приземления
        Vector2 scatter = Random.insideUnitCircle * scatterRadius;
        _targetPos = new Vector3(
            _startPos.x + scatter.x,
            groundY,
            _startPos.z + scatter.y
        );

        _arcTime = 0f;
        _landing = true;
        _timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (_landing)
        {
            _arcTime += Time.deltaTime / arcDuration;
            float t = Mathf.Clamp01(_arcTime);

            // дуга: lerp позиции + синус по высоте
            Vector3 pos = Vector3.Lerp(_startPos, _targetPos, t);
            pos.y = Mathf.Lerp(_startPos.y, groundY, t) + Mathf.Sin(t * Mathf.PI) * arcHeight;
            transform.position = pos;

            if (t >= 1f)
            {
                _landing = false;
                _baseY = groundY;
            }
        }
        else
        {
            // плавание после приземления
            Vector3 pos = transform.position;
            pos.y = _baseY + Mathf.Sin(Time.time * floatSpeed + _timeOffset) * floatAmplitude;
            transform.position = pos;
        }
    }
}