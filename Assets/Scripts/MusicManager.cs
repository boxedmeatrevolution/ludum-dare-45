using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip ghostighostTheme;
    public AudioClip pabsTheme;
    public AudioClip mainTheme;
    private AudioSource audioSource;

    public enum Music {
        Ghostighost,
        Pabs,
        Main
    };

    private Music currentlyPlaying;
    private Music nextPlaying;
    private float transitionFactor;

    // Start is called before the first frame update
    void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
        this.currentlyPlaying = Music.Ghostighost;
        this.nextPlaying = Music.Ghostighost;
        this.transitionFactor = 1f;
        this.audioSource.clip = this.ghostighostTheme;
        this.audioSource.loop = true;
        this.audioSource.Play();
    }

    // Update is called once per frame
    void Update() {
        float deltaTime = 1f / 60f;
        this.audioSource.volume = 0.5f * this.transitionFactor;
        if (this.nextPlaying != this.currentlyPlaying) {
            this.transitionFactor -= 0.5f * deltaTime;
            if (this.transitionFactor < 0f) {
                this.audioSource.Stop();
                if (this.nextPlaying == Music.Ghostighost) {
                    this.audioSource.clip = this.ghostighostTheme;
                }
                else if (this.nextPlaying == Music.Main) {
                    this.audioSource.clip = this.mainTheme;
                }
                else if (this.nextPlaying == Music.Pabs) {
                    this.audioSource.clip = this.pabsTheme;
                }
                this.currentlyPlaying = this.nextPlaying;
                this.audioSource.Play();
                this.audioSource.volume = 0f;
                this.transitionFactor = 0f;
            }
        }
        else {
            if (this.transitionFactor < 1f) {
                this.transitionFactor += 0.5f * deltaTime;
            }
            if (this.transitionFactor > 1f) {
                this.transitionFactor = 1f;
            }
        }
    }

    public void PlayGhostighostTheme() {
        this.nextPlaying = Music.Ghostighost;
    }

    public void PlayPabsTheme() {
        this.nextPlaying = Music.Pabs;
    }

    public void PlayMainTheme() {
        this.nextPlaying = Music.Main;
    }
}
