using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManagerScript : MonoBehaviour {

    public enum Sound
    {
        buttonClick, damage, gamePlay, gameWinner, menu, portal, powerUp, rollDice, death,
    };
    
    public static List<AudioClip> sounds = new List<AudioClip>();

    private static AudioSource audioSrcSfx;
    private static AudioSource audioSrcMusic;

    public Slider sfxVolume;
    public Slider musicVolume;

    private static int limiter = 0;

    private void Start()
    {
        sounds.Add(Resources.Load<AudioClip>("buttonClick"));
        sounds.Add(Resources.Load<AudioClip>("damage"));
        sounds.Add(Resources.Load<AudioClip>("gamePlay"));
        sounds.Add(Resources.Load<AudioClip>("gameWinner"));
        sounds.Add(Resources.Load<AudioClip>("menu"));
        sounds.Add(Resources.Load<AudioClip>("portal"));
        sounds.Add(Resources.Load<AudioClip>("powerUp"));
        sounds.Add(Resources.Load<AudioClip>("rollDice"));
        sounds.Add(Resources.Load<AudioClip>("death"));

        AudioSource[] sources = GetComponents<AudioSource>();
        audioSrcSfx = sources[0];
        audioSrcMusic = sources[1];

        audioSrcSfx.volume = PlayerPrefs.GetFloat("sfxVolume",0.95f);
        sfxVolume.value = audioSrcSfx.volume;
        audioSrcMusic.volume = PlayerPrefs.GetFloat("musicVolume",0.10f);
        musicVolume.value = audioSrcMusic.volume;

        if (SceneManager.GetActiveScene().buildIndex == 0)
            PlaySound(Sound.menu);
        else
            PlaySound(Sound.gamePlay);
    }

    public void Update()
    {
        if(!audioSrcMusic.isPlaying)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
                PlaySound(Sound.menu);
            else
                PlaySound(Sound.gamePlay);
        }
           
    }

    public void SetMusicLevel()
    {
        audioSrcMusic.volume = musicVolume.value;
        PlayerPrefs.SetFloat("musicVolume", musicVolume.value);
    }

    public void SetSfxLevel()
    {
        audioSrcSfx.volume = sfxVolume.value;
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume.value);
    }

    public static void PlaySound(Sound clip)
    {
        limiter++;
        foreach(AudioClip a in sounds)
        {
            if(clip.ToString().Equals(a.name))
            {
                if (a.name == "gamePlay" || a.name == "menu")
                    audioSrcMusic.PlayOneShot(a);
                else
                    if(limiter<2)
                        audioSrcSfx.PlayOneShot(a);
                limiter--;
                return;
            }
        }
        limiter--;
    }
}
