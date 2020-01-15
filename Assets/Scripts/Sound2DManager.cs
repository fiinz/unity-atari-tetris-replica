using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound2DManager : MonoBehaviour
{
    public static Sound2DManager instance;
    private AudioSource musicAudioSource;
    public AudioMixerGroup musicAudioMixerGroup;
    public AudioMixerGroup sfxAudioMixerGroup;
    public int numSfxAudioSources;
    private AudioSource[] audioSfxSources2D;

    private void Awake()
    {
        if (instance == null)instance = this;
        else Destroy(this);        
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        musicAudioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        if (audioSfxSources2D == null)
        {
            audioSfxSources2D = new AudioSource[numSfxAudioSources];
            for (int i = 0; i < numSfxAudioSources; i++)
            { audioSfxSources2D[i] = gameObject.AddComponent(typeof(AudioSource)) as AudioSource; }

        }
      
    }


    public void PlaySfx(AudioClip sfxClip)
    {
        for (int i = 0; i < numSfxAudioSources; i++)
        {
            if(!audioSfxSources2D[i].isPlaying && audioSfxSources2D[i].isActiveAndEnabled)
            {
                audioSfxSources2D[i].volume = 1;
                audioSfxSources2D[i].loop = false;
                audioSfxSources2D[i].clip = sfxClip;
                audioSfxSources2D[i].Play();
                return;

            }
        }


    }

    public void PlayMusic(AudioClip musicClip)
    {
        Debug.Log("music" + musicClip);
        musicAudioSource.clip = musicClip;
        musicAudioSource.loop = true;
        musicAudioSource.volume = 0.5f;
       // musicAudioSource.outputAudioMixerGroup = musicAudioMixerGroup;
        musicAudioSource.Play();

    }

   
}
