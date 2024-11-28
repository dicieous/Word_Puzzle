using UnityEngine.Audio;
using System;
using UnityEngine;

public class SoundHapticManager : MonoBehaviour
{
	public static SoundHapticManager Instance;

	public AudioMixerGroup mixerGroup;

	public Sound[] sounds;
	int hitNo = 0;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
	}

	private void Start()
	{
		if(SavedData.SoundToggle)
			Play("BG_Music");
		Vibration.Init();
	}

	public void HitMethod()
	{
		Play("hit" + hitNo);
		if (hitNo <= 4)
		{
			hitNo++;
		}
		else
		{
			hitNo = 0;
		}
	}

	public void Vibrate(long MilliSecs)
	{
		Vibration.VibrateAndroid(MilliSecs);
	}

	public void Play(string sound)
	{
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

	public void Pause(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Pause();
	}
}