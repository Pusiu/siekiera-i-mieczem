using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioZone : MonoBehaviour
{
	public AudioClip audioClip;
	public bool isPlayerInZone = false;
	float maxVol = 0.1f;
	AudioSource source;

	private void Start()
	{
		source = GetComponent<AudioSource>();
		source.clip = audioClip;
		maxVol = source.volume;
	}

	float fadeTime = 4;
	IEnumerator FadeCoroutine()
	{
		float s = (isPlayerInZone) ? maxVol : 0;
		float e = (isPlayerInZone) ? 0 : maxVol;
		float startTime = Time.time;


		while (Time.time < startTime+fadeTime)
		{
			source.volume = Mathf.Lerp(s, e, Mathf.Clamp01((Time.time - startTime) / fadeTime));
			yield return new WaitForEndOfFrame();
		}
		if (source.volume == 0)
			source.Stop();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			if (!source.isPlaying)
			{
				//StopAllCoroutines();
				StartCoroutine(FadeCoroutine());
				source.Play();
			}
			isPlayerInZone = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			//source.Stop();
			StopAllCoroutines();
			StartCoroutine(FadeCoroutine());
			isPlayerInZone = false;
		}
	}
}
