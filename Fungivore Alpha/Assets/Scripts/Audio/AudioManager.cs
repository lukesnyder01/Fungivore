﻿using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// From Brackeys https://www.youtube.com/watch?v=6OT43pvUyfY

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance { get; private set; }

	public AudioMixerGroup mixerGroup;

	public Sound[] sounds;

	private bool isFadingOut = false;
	private IEnumerator fadeCoroutine;


	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
			InitializeAudioSources();
		}
	}

	
	private void InitializeAudioSources()
	{
		foreach (Sound s in sounds)
		{
			if (s.source == null)
			{
				s.source = gameObject.AddComponent<AudioSource>();
				s.source.clip = s.clip;
				s.source.loop = s.loop;
				s.source.outputAudioMixerGroup = mixerGroup;
			}
		}
	}



	public void Play(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		StartCoroutine(SmoothVolumeCoroutine(s));
	}




	public void StartLoop(string sound)
	{
		if (isFadingOut)
		{
			StopCoroutine(fadeCoroutine);
			isFadingOut = false;
		}

		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
		s.source.Play();
	}


	public void StartLoop(string sound, float pitch)
	{
		if (isFadingOut)
		{
			StopCoroutine(fadeCoroutine);
			isFadingOut = false;
		}

		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = pitch;
		s.source.Play();
	}


	public void Stop(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.Stop();
	}


	public void FadeOut(string sound, float fadeLength)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		//store a reference to the most recent fade out so we can stop the coroutine later
		fadeCoroutine = FadeOutCoroutine(s, fadeLength);
		StartCoroutine(fadeCoroutine);
	}


	public void PlayAtVolume(string sound, float playbackVolume)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);

		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		if (s.source == null)
		{
			Debug.LogWarning("AudioSource for sound " + sound + " is null.");
			return;
		}

		s.volume = playbackVolume;

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}



	public IEnumerator FadeOutCoroutine(Sound s, float fadeLength)
	{
		isFadingOut = true;

		float startVolume = s.source.volume;

		while (s.source.volume > 0)
		{
			s.source.volume -= startVolume * Time.deltaTime / fadeLength;
			yield return null;
		}

		if (s.source.volume <= 0)
		{
			s.source.Stop();
			isFadingOut = false;
		}
	}



	public IEnumerator SmoothVolumeCoroutine(Sound s)
	{
		float startVolume = s.source.volume;

		//this quickly fades the volume out of the audio source before playing the clip, to avoid popping by ensuring new clips always start from 0
		while (s.source.volume > 0)
		{
			s.source.volume -= startVolume * Time.deltaTime / 0.05f;
			yield return null;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
		s.source.Play();
	}




}