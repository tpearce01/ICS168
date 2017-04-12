using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : Singleton<SoundManager> {

    public float masterVolume; 

	//EndSound vars
	AudioSource target;
	float duration;

	List<AudioSource> clipsPlaying = new List<AudioSource>();
	public AudioClip[] clips;


	void Start(){
		Initialize ();
	}

	void Initialize(){
		//i = this;
		target = null;
		duration = 0;
	}

	void Update () {
		//Check for clips which have finished playing
		for (int i = 0; i < clipsPlaying.Count; i++) {
			AudioSource source = clipsPlaying [i];
			if (!source.isPlaying) {
				clipsPlaying.Remove (source);
				Destroy (source);
				i--;
			} else {
				clipsPlaying [i].pitch = Mathf.Clamp(Time.timeScale, 0.5f, 1f);
			}
		}

		//Fade the target clip if one exists
		if (target != null) {
			target.volume -= (1/duration) * Time.deltaTime;
			if (target.volume <= 0) {
				clipsPlaying.Remove (target);
				Destroy (target);
				target = null;
			}
		}
	}

	//Plays a sound
	/// <summary>
	/// Plays a sound once at default volume
	/// </summary>
	/// <param name="clipNumber">Clip number.</param>
	public void PlaySound(Sound clipNumber){
		PlaySound (clipNumber, 1);
	}
	/// <summary>
	/// Plays a sound once at the specified volume
	/// </summary>
	/// <param name="clipNumber">Clip number.</param>
	/// <param name="v">V.</param>
	public void PlaySound(Sound clipNumber, float v){
		if ((int)clipNumber < clips.Length && (int)clipNumber >= 0) {
			AudioSource source = gameObject.AddComponent<AudioSource> ();
			source.clip = clips [(int)clipNumber];
			source.volume = masterVolume * v;
			source.Play ();
			clipsPlaying.Add (source);
		}
	}

	//Plays a sound on loop
	/// <summary>
	/// Plays a sound with looping enabled at default volume
	/// </summary>
	/// <param name="clipNumber">Clip number.</param>
	public void PlaySoundLoop(Sound clipNumber){
		PlaySoundLoop (clipNumber, 1);
	}
	/// <summary>
	/// Plays a sound with looping enabled at specified volume
	/// </summary>
	/// <param name="clipNumber">Clip number.</param>
	/// <param name="v">V.</param>
	public void PlaySoundLoop(Sound clipNumber, float v){
		if ((int)clipNumber < clips.Length && (int)clipNumber >= 0) {
			AudioSource source = gameObject.AddComponent<AudioSource> ();
			source.clip = clips [(int)clipNumber];
			source.volume = masterVolume * v;
			source.Play ();
			source.loop = true;
			clipsPlaying.Add (source);
		}
	}

	//Ends a sound immediately
	/// <summary>
	/// Ends a sound immediately
	/// </summary>
	/// <param name="soundName">Sound name.</param>
	public void EndSoundAbrupt(string soundName){
		AudioSource[] sources = gameObject.GetComponents<AudioSource>();
		for (int i = 0; i < sources.Length; i++) {
			if (sources[i].clip.name == soundName) {
				clipsPlaying.Remove (sources [i]);
				Destroy (sources[i]);
				break;
			}
		}
	}

	//Fades a sound out over a duration
	/// <summary>
	/// Ends a sound over a duration. Currently only handles 1 sound at a time.
	/// </summary>
	/// <param name="soundName">Sound name.</param>
	/// <param name="d">D.</param>
	public void EndSoundFade(string soundName, float d){
		AudioSource[] sources = gameObject.GetComponents<AudioSource>();
		for (int i = 0; i < sources.Length; i++) {
			if (sources[i].clip.name == soundName) {
				target = sources[i];
				break;
			}
		}
		duration = d;
	}

	//Ends all sounds immediately
	/// <summary>
	/// Ends all sounds immediately
	/// </summary>
    public void EndAllSound()
    {
        AudioSource[] sources = gameObject.GetComponents<AudioSource>();
        for (int i = 0; i < sources.Length; i++)
        {
                EndSoundAbrupt(sources[i].clip.name);
        }
    }

	//Ends all sounds of the specified name
	/// <summary>
	/// Ends all sounds of the specified name
	/// </summary>
	/// <param name="soundName">Sound name.</param>
    public void EndAllSound(string soundName)
    {
        AudioSource[] sources = gameObject.GetComponents<AudioSource>();
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i].clip.name == soundName)
            {
                EndSoundAbrupt(soundName);
            }
        }
    }
}

/// <summary>
/// Sound library. Converts sound name to integer
/// </summary>
[System.Serializable]
public enum Sound{
	Test = -1,
};