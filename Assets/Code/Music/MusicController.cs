using UnityEngine;

public class MusicController: MonoBehaviour
{
    [Header("Audio Sources for crossfading)")]
    public AudioSource source1;
    public AudioSource source2;

    [Header("Playlist")]
    public AudioClip[] playlist;

    [Header("Settings")]
    public float crossfadeDuration = 3.0f; // seconds
    public float switchBeforeEnd = 5.0f;    // start fading before the end
    public float maxVolume = 0.3f;

    private int currentClipIndex = 0;
    private AudioSource currentSource;
    private AudioSource nextSource;
    private bool isCrossfading = false;

    public static MusicController instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentSource = source1;
        nextSource = source2;

        playlist.Shuffle();

        if (playlist.Length > 0)
        {
            PlayClip(currentSource, playlist[currentClipIndex]);
            currentSource.volume = maxVolume;
        }
    }

    private void Update()
    {
        if (!isCrossfading && currentSource.isPlaying)
        {
            if (currentSource.clip.length - currentSource.time <= switchBeforeEnd)
            {
                CrossfadeToNextSong();
            }
        }
    }

    private void CrossfadeToNextSong()
    {
        isCrossfading = true;

        // Prepare next clip
        currentClipIndex = (currentClipIndex + 1) % playlist.Length;
        AudioClip nextClip = playlist[currentClipIndex];

        PlayClip(nextSource, nextClip);
        nextSource.volume = 0f;

        // Fade volumes using LeanTween
        LeanTween.value(gameObject, currentSource.volume, 0f, crossfadeDuration)
            .setOnUpdate((float val) => { currentSource.volume = val; });

        LeanTween.value(gameObject, nextSource.volume, maxVolume, crossfadeDuration)
            .setOnUpdate((float val) => { nextSource.volume = val; })
            .setOnComplete(() => {
                currentSource.Stop();
                SwapSources();
                isCrossfading = false;
            });
    }

    private void PlayClip(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    private void SwapSources()
    {
        AudioSource temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
