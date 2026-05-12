// AudioPool.cs
using UnityEngine;

public class AudioPool : MonoBehaviour
{
    public static AudioPool Instance { get; private set; }

    [SerializeField] int initialSize = 10;
    [SerializeField] Vector2 pitchRange = new(0.9f, 1.1f);

    AudioSource[] _sources;

    void Awake()
    {
        Instance = this;
        _sources = new AudioSource[initialSize];

        for (int i = 0; i < initialSize; i++)
        {
            var go = new GameObject("AudioSource");
            go.transform.SetParent(transform);
            _sources[i] = go.AddComponent<AudioSource>();
            _sources[i].playOnAwake = false;
        }
    }

    public void Play(AudioClip clip, Vector3 position, bool randomPitch = true)
    {
        if (clip == null) return;

        var source = GetFree();
        source.transform.position = position;
        source.clip = clip;
        source.pitch = randomPitch ? Random.Range(pitchRange.x, pitchRange.y) : 1f;
        source.Play();
    }

    AudioSource GetFree()
    {
        foreach (var s in _sources)
            if (!s.isPlaying) return s;

        // все заняты — берём первый (перебиваем)
        return _sources[0];
    }
}