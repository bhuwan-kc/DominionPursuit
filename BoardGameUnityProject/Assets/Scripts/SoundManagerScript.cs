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

    private static float sfxLevel = 0.95f;
    private static float musicLevel = 0.25f;

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

        audioSrcSfx.volume = sfxLevel;
        audioSrcMusic.volume = musicLevel;

        if (SceneManager.GetActiveScene().buildIndex == 0)
            PlaySound(Sound.menu);
        else
            PlaySound(Sound.gamePlay);
    }

    public void Update()
    {
    }

    public static void StopMusic()
    {
        audioSrcMusic.Stop();
    }

    public static void StopSfx()
    {
        audioSrcSfx.Stop();
    }

    public static void PlaySound(Sound clip)
    {
        foreach(AudioClip a in sounds)
        {
            if(clip.ToString().Equals(a.name))
            {
                if (a.name == "gamePlay" || a.name == "menu")
                    audioSrcMusic.PlayOneShot(a);
                else
                    audioSrcSfx.PlayOneShot(a);
                return;
            }
        }
    }
}
