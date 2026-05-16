using UnityEngine;

public class OrbFloat : MonoBehaviour
{
    [SerializeField] float groundY = 0f;
    [SerializeField] float floatAmplitude = 0.15f;
    [SerializeField] float floatSpeed = 2f;
    [SerializeField] float arcHeight = 1.5f;
    [SerializeField] float arcDuration = 0.4f;
    [SerializeField] float scatterRadius = 1.2f;
    [SerializeField] float rotateSpeed = 90f;
    [SerializeField] float pulseSpeed = 2f;
    [SerializeField] float pulseMin = 1f;
    [SerializeField] float pulseMax = 3f;

    float _baseY;
    float _timeOffset;
    Vector3 _startPos;
    Vector3 _targetPos;
    float _arcTime;
    bool _landing;
    Renderer _renderer;
    MaterialPropertyBlock _propBlock;
    Color _emissionColor;

    void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    void OnEnable()
    {
        _startPos = transform.position;

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

    public void SetEmissionColor(Color color)
    {
        _emissionColor = color;
    }

    void Update()
    {
        if (_landing)
        {
            _arcTime += Time.deltaTime / arcDuration;
            float t = Mathf.Clamp01(_arcTime);

            Vector3 pos = Vector3.Lerp(_startPos, _targetPos, t);
            pos.y = Mathf.Lerp(_startPos.y, groundY, t) + Mathf.Sin(t * Mathf.PI) * arcHeight;
            transform.position = pos;
            transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);

            if (t >= 1f)
            {
                _landing = false;
                _baseY = groundY;
            }
        }
        else
        {
            Vector3 pos = transform.position;
            pos.y = _baseY + Mathf.Sin(Time.time * floatSpeed + _timeOffset) * floatAmplitude;
            transform.position = pos;
            transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
        }

        float intensity = Mathf.Lerp(pulseMin, pulseMax, (Mathf.Sin(Time.time * pulseSpeed + _timeOffset) + 1f) * 0.5f);
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_EmissionColor", _emissionColor * intensity);
        _renderer.SetPropertyBlock(_propBlock);
    }
}