using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSound : MonoBehaviour
{
	public AudioClip[] sounds;
	AudioSource source;
	int index = 0;
    // Start is called before the first frame update
    void Start()
    {
		source = GetComponent<AudioSource>();
		source.clip = sounds[index];
    }

    public void Play()
	{
		source.clip = sounds[index];
		source.Play();

		index++;
		if (index >= sounds.Length)
			index = 0;
	}
}
