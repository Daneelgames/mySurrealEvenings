using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicController : MonoBehaviour
{
    public AudioSource _audio;
    public List<AudioClip> tracks;

    private bool fade = false;
    public void SetMusicTrack(string time)
    {
        fade = false;
        _audio.volume = 1;

        int musicTrack = 0;

        switch (time)
        {
            case "day":
                musicTrack = 1; //randomize tracks
                break;

            case "night":
                musicTrack = 0; //randomize tracks
                break;
        }
        _audio.clip = tracks[musicTrack];
		_audio.Play();
    }

    public void SetMusicFade()
    {
        fade = true;
    }

    void Update()
    {
        if (fade && _audio.volume > 0.01f)
        {
            _audio.volume = Mathf.Lerp(_audio.volume, 0, 0.1f);
        }
        else if (_audio.volume <= 0.01f)
            _audio.volume = 0;
    }
}