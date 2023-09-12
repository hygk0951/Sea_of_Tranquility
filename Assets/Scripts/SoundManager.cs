using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public List<AudioClip> bgmClips = new List<AudioClip>();
    AudioSource currentBgm;

    [SerializeField] bool debugPlay = false;

    void Awake()
    {
        currentBgm = gameObject.AddComponent<AudioSource>();
        // AddSFXList();
        if (debugPlay && bgmClips.Count != 0)
        {
            currentBgm.clip = bgmClips[0];
            currentBgm.Play();
        }
    }

    private void AddSFXList()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
