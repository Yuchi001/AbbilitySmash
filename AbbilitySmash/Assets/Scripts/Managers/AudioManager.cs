using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region
    public static AudioManager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion
    [SerializeField] private AudioClass[] clips;
    [SerializeField] private GameObject audioSourcePrefab;
    public void PlaySoundEffect(ESoundEffectName passedName)
    {
        foreach(var clip in clips)
        {
            if (clip.clipName == passedName)
            {
                AudioSource ads = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
                ads.loop = false;
                ads.volume = SavingManager.Instance.soundEffectsVolume;
                ads.PlayOneShot(clip.clip);
                Destroy(ads.gameObject, 0.3f);
            }
        }
    }
}
[System.Serializable]
public class AudioClass
{
    public AudioClip clip;
    public ESoundEffectName clipName;
}
