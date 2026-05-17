using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Воспроизводит треки из списка в случайном порядке без повторов подряд.
/// Привяжи компонент к любому GameObject, добавь AudioClip-ы в инспекторе.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class RandomMusicPlayer : MonoBehaviour
{
    [Header("Треки")]
    [Tooltip("Список аудиоклипов для воспроизведения")]
    public List<AudioClip> tracks = new List<AudioClip>();

    [Header("Настройки")]
    [Tooltip("Громкость (0–1)")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Tooltip("Задержка между треками (сек)")]
    public float delayBetweenTracks = 0f;

    // ── внутренние поля ──────────────────────────────────────────────────────
    private AudioSource _audioSource;
    private List<int>   _shuffledIndices = new List<int>();
    private int         _currentIndex    = -1;   // позиция в перемешанном списке
    private int         _lastPlayedClip  = -1;   // индекс клипа (защита от повтора)
    private bool        _isTransitioning = false; // защита от множественного запуска корутины

    // ────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        _audioSource        = GetComponent<AudioSource>();
        _audioSource.loop   = false;
        _audioSource.volume = volume;
    }

    private void Start()
    {
        if (tracks == null || tracks.Count == 0)
        {
            Debug.LogWarning("[RandomMusicPlayer] Список треков пуст!");
            return;
        }

        if (tracks.Count == 1)
        {
            PlayClip(0);
            return;
        }

        BuildShuffledList();
        PlayNext();
    }

    private void Update()
    {
        // Запускаем следующий трек только если:
        // — ничего не играет
        // — нет активного перехода (защита от чехарды при потере фокуса)
        if (!_audioSource.isPlaying && tracks.Count > 0 && !_isTransitioning)
        {
            _isTransitioning = true;
            StartCoroutine(PlayNextWithDelay());
        }
    }

    // Пауза при потере фокуса, продолжение с той же точки при возврате
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            _audioSource.Pause();
        else
            _audioSource.UnPause();
    }

    // ── публичные методы ─────────────────────────────────────────────────────

    /// <summary>Пропустить текущий трек и включить следующий.</summary>
    public void Skip()
    {
        if (_isTransitioning) return;
        _audioSource.Stop();
        _isTransitioning = true;
        StartCoroutine(PlayNextWithDelay());
    }

    /// <summary>Остановить воспроизведение.</summary>
    public void Stop() => _audioSource.Stop();

    /// <summary>Возобновить воспроизведение.</summary>
    public void Resume()
    {
        if (!_audioSource.isPlaying)
            _audioSource.Play();
    }

    /// <summary>Изменить громкость во время игры.</summary>
    public void SetVolume(float v)
    {
        volume = Mathf.Clamp01(v);
        _audioSource.volume = volume;
    }

    // ── внутренняя логика ────────────────────────────────────────────────────

    private void BuildShuffledList()
    {
        _shuffledIndices.Clear();

        for (int i = 0; i < tracks.Count; i++)
            _shuffledIndices.Add(i);

        // Fisher-Yates shuffle
        for (int i = _shuffledIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (_shuffledIndices[i], _shuffledIndices[j]) = (_shuffledIndices[j], _shuffledIndices[i]);
        }

        // Первый элемент нового списка не должен совпадать с последним сыгранным
        if (_lastPlayedClip != -1 && _shuffledIndices[0] == _lastPlayedClip && _shuffledIndices.Count > 1)
        {
            int swapWith = Random.Range(1, _shuffledIndices.Count);
            (_shuffledIndices[0], _shuffledIndices[swapWith]) = (_shuffledIndices[swapWith], _shuffledIndices[0]);
        }

        _currentIndex = 0;
    }

    private void PlayNext()
    {
        if (_currentIndex >= _shuffledIndices.Count)
            BuildShuffledList();

        int clipIndex = _shuffledIndices[_currentIndex];
        _currentIndex++;

        PlayClip(clipIndex);
    }

    private void PlayClip(int clipIndex)
    {
        AudioClip clip = tracks[clipIndex];
        if (clip == null)
        {
            Debug.LogWarning($"[RandomMusicPlayer] Трек #{clipIndex} равен null, пропускаю.");
            _isTransitioning = false;
            PlayNext();
            return;
        }

        _lastPlayedClip     = clipIndex;
        _audioSource.clip   = clip;
        _audioSource.volume = volume;
        _audioSource.Play();

        Debug.Log($"[RandomMusicPlayer] Играет: {clip.name} (индекс {clipIndex})");
    }

    private IEnumerator PlayNextWithDelay()
    {
        _audioSource.clip = null;

        if (delayBetweenTracks > 0f)
            yield return new WaitForSeconds(delayBetweenTracks);

        PlayNext();
        _isTransitioning = false; // снимаем блокировку после запуска трека
    }
}