using UnityEngine;

[CreateAssetMenu()]
public class AudioClipResfsSO : ScriptableObject
{
    public AudioClip[] chop;
    public AudioClip[] deliveryFail;
    public AudioClip[] deliverySuccess;
    public AudioClip[] footstep;
    public AudioClip[] objectDrop;
    public AudioClip[] objectPickup;
    public AudioClip stoveSizzle;
    public AudioClip[] trash;
    public AudioClip[] warning;

    // UI
    public AudioClip[] uiHover;
    public AudioClip[] uiClick;

    // Music
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip gameOverMusic;


}
