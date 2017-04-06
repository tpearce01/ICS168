using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

	public static SoundManager i;
    public float volume; 

	//EndSound vars
	AudioSource target;
	float duration;

	List<AudioSource> clipsPlaying = new List<AudioSource>();
	public AudioClip[] clips;

	void Awake () {
		//Destroy this object if one already exists
	    if (GameObject.FindGameObjectsWithTag("GameManager").Length > 1)
	    {
	        Destroy(gameObject);
	    }
	    else
	    {
			Initialize ();
        }
	}

	void Initialize(){
		i = this;
		target = null;
		duration = 0;
		volume = 0.5f;
		DontDestroyOnLoad(gameObject);
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
	public void PlaySound(Sound clipNumber, float v){
		AudioSource source = gameObject.AddComponent<AudioSource> ();
		source.clip = clips [(int)clipNumber];
		source.volume = volume * v;
		source.Play ();
		clipsPlaying.Add (source);
	}

	//Plays a sound on loop
	public void PlaySoundLoop(Sound clipNumber, float v){
		AudioSource source = gameObject.AddComponent<AudioSource> ();
		source.clip = clips [(int)clipNumber];
		source.volume = volume * v;
		source.Play ();
		source.loop = true;
		clipsPlaying.Add (source);
	}

	//Ends a sound immediately
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
    public void EndAllSound()
    {
        AudioSource[] sources = gameObject.GetComponents<AudioSource>();
        for (int i = 0; i < sources.Length; i++)
        {
                EndSoundAbrupt(sources[i].clip.name);
        }
    }

	//Ends all sounds of the specified name
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

[System.Serializable]
public enum Sound{
	Test = 0,
};
