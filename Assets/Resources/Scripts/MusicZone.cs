using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicZone : MonoBehaviour
{
	AudioSource src;
	public float timeBetweenPlays = 10;
	float nextMusicTime = 0;

    // Start is called before the first frame update
    void Start()
    {
		src = GetComponent<AudioSource>();
		nextMusicTime = src.clip.length+timeBetweenPlays;
		PlayMusic();
    }

	public void PlayMusic()
	{
		src.Stop();
		src.Play();
		Invoke("PlayMusic", nextMusicTime);
	}
}
