using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClipResfsSO audioClipRefsSO;

    private bool isGameplaySoundEnabled = true;


    private AudioSource musicAudioSource;   
    private AudioSource uiAudioSource;  

    private readonly List<AudioSource> gameplayAudioSources = new();


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

      
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.loop = true;
        musicAudioSource.spatialBlend = 0f;
        musicAudioSource.volume = 0.4f;

        uiAudioSource = gameObject.AddComponent<AudioSource>();
        uiAudioSource.loop = false;
        uiAudioSource.spatialBlend = 0f;
        uiAudioSource.volume = 1f;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        RegisterAllEvents();

        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnregisterAllEvents();
    }

  

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isGameplaySoundEnabled = true;   

        StopAllGameplaySounds();
        RegisterAllEvents();

        musicAudioSource.Stop();
        musicAudioSource.loop = true;
        musicAudioSource.volume = 0.4f;

        if (scene.name == "GameMenuScenes")
            PlayMusic(audioClipRefsSO.menuMusic);
        else
            PlayMusic(audioClipRefsSO.gameMusic);
    }


    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }



    private void RegisterAllEvents()
    {
        UnregisterAllEvents();

        if (KitchenGameManager.Instance != null)
            KitchenGameManager.Instance.OnGameOver += OnGameOver;

        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.OnRecipeSuccess += OnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFailed += OnRecipeFailed;
        }

        if (Player.Instance != null)
            Player.Instance.OnPickedSomething += OnPickedSomething;

        CuttingCounter.OnAnyCut += OnAnyCut;
        BaseCounter.OnAnyObjectPlaceHere += OnObjectPlaced;
        TrashCounter.OnAnyobjectTrashed += OnObjectTrashed;
    }

    private void UnregisterAllEvents()
    {
        if (KitchenGameManager.Instance != null)
            KitchenGameManager.Instance.OnGameOver -= OnGameOver;

        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.OnRecipeSuccess -= OnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFailed -= OnRecipeFailed;
        }

        if (Player.Instance != null)
            Player.Instance.OnPickedSomething -= OnPickedSomething;

        CuttingCounter.OnAnyCut -= OnAnyCut;
        BaseCounter.OnAnyObjectPlaceHere -= OnObjectPlaced;
        TrashCounter.OnAnyobjectTrashed -= OnObjectTrashed;
    }


    private void OnGameOver(object sender, EventArgs e)
    {
        isGameplaySoundEnabled = false;

        StopAllGameplaySounds();

        if (audioClipRefsSO.gameOverMusic == null) return;

        musicAudioSource.Stop();
        musicAudioSource.loop = true;
        musicAudioSource.volume = 0.5f;
        musicAudioSource.clip = audioClipRefsSO.gameOverMusic;
        musicAudioSource.Play();
    }




    private void OnRecipeSuccess(object sender, EventArgs e)
    {
        if (DeliveryCounter.Instance == null) return;
        PlayWorldSound(audioClipRefsSO.deliverySuccess,
            DeliveryCounter.Instance.transform.position);
    }

    private void OnRecipeFailed(object sender, RecipeFailedEventArgs e)
    {
        if (DeliveryCounter.Instance == null) return;
        PlayWorldSound(audioClipRefsSO.deliveryFail,
            DeliveryCounter.Instance.transform.position);
    }

    private void OnPickedSomething(object sender, EventArgs e)
    {
        if (Player.Instance == null) return;
        PlayWorldSound(audioClipRefsSO.objectPickup,
            Player.Instance.transform.position);
    }

    private void OnAnyCut(object sender, EventArgs e)
    {
        CuttingCounter c = sender as CuttingCounter;
        if (c == null) return;
        PlayWorldSound(audioClipRefsSO.chop, c.transform.position);
    }

    private void OnObjectPlaced(object sender, EventArgs e)
    {
        BaseCounter c = sender as BaseCounter;
        if (c == null) return;
        PlayWorldSound(audioClipRefsSO.objectDrop, c.transform.position);
    }

    private void OnObjectTrashed(object sender, EventArgs e)
    {
        TrashCounter c = sender as TrashCounter;
        if (c == null) return;
        PlayWorldSound(audioClipRefsSO.trash, c.transform.position);
    }

    public void PlayFootstepSound(Vector3 pos, float volume)
    {
        PlayWorldSound(audioClipRefsSO.footstep, pos, volume);
    }

    public void PlayUIHoverSound()
    {
        if (audioClipRefsSO.uiHover.Length > 0)
            uiAudioSource.PlayOneShot(audioClipRefsSO.uiHover[0], 0.7f);
    }

    public void PlayUIClickSound()
    {
        if (audioClipRefsSO.uiClick.Length > 0)
            uiAudioSource.PlayOneShot(audioClipRefsSO.uiClick[0], 1f);
    }


    private void PlayWorldSound(AudioClip[] clips, Vector3 position, float volume = 1f)
    {
        if (!isGameplaySoundEnabled) return; // 🔥 CHỐT HẠ

        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];

        GameObject temp = new GameObject("GameplayAudio");
        temp.transform.position = position;

        AudioSource src = temp.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volume;
        src.spatialBlend = 1f;
        src.Play();

        gameplayAudioSources.Add(src);
        Destroy(temp, clip.length);
    }


    private void StopAllGameplaySounds()
    {
        foreach (var src in gameplayAudioSources)
        {
            if (src == null) continue;
            src.Stop();
            Destroy(src.gameObject);
        }
        gameplayAudioSources.Clear();
    }
}
