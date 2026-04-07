using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Tooltip("Assign an AudioClip for successful bond formation")]
    public AudioClip sfx_bondSuccess,
        sfx_break,
        sfx_snap, sfx_atomCreated;
    private AudioSource audioSource;
    Transform cameraTransform;

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

    void Start()
    {
        cameraTransform = Camera.main.transform;
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

    bool canPlaySnapSoundAgain = true;

    public void PlaySnap(Vector3 position)
    {
        if (sfx_snap != null && canPlaySnapSoundAgain)
        {
            canPlaySnapSoundAgain = false;
            AudioSource.PlayClipAtPoint(sfx_snap, position);
            StartCoroutine(SubRoutine());
        }

        IEnumerator SubRoutine()
        {
            yield return new WaitForSeconds(0.5f);
            canPlaySnapSoundAgain = true;
        }
    }

    public void PlayAtomCreated(Vector3 position)
    {
        if (sfx_atomCreated != null)
            AudioSource.PlayClipAtPoint(sfx_atomCreated, position);
    }
}
