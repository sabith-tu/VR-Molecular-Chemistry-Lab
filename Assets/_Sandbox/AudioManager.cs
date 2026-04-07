using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Tooltip("Assign an AudioClip for successful bond formation")]
    public AudioClip sfx_bondSuccess,
        sfx_break,
        sfx_snap;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound for VR
    }

    public void PlayBondSound(Vector3 position)
    {
        if (sfx_bondSuccess != null)
            AudioSource.PlayClipAtPoint(sfx_bondSuccess, position);
    }

    public void PlayBreakSound(Vector3 position)
    {
        if (sfx_break != null)
            AudioSource.PlayClipAtPoint(sfx_break, position);
    }

    public void PlaySnap(Vector3 position)
    {
        if (sfx_snap != null)
            AudioSource.PlayClipAtPoint(sfx_snap, position);
    }
}
