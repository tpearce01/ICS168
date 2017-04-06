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

	// Use this for initialization
	void Awake () {
	    if (GameObject.FindGameObjectsWithTag("GameManager").Length > 1)
	    {
	        Destroy(gameObject);
	    }
	    else
	    {
	        i = this;
	        target = null;
	        duration = 0;
            volume = 0.5f;
            //DontDestroyOnLoad(gameObject);
        }
	}

	// Update is called once per frame
	void Update () {
		

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

		if (target != null) {
			target.volume -= (1/duration) * Time.deltaTime;
			if (target.volume <= 0) {
				clipsPlaying.Remove (target);
				Destroy (target);
				target = null;
			}
		}
	}

	public void PlaySound(Sound clipNumber, float volume){
		AudioSource source = gameObject.AddComponent<AudioSource> ();
		source.clip = clips [(int)clipNumber];
		source.volume = volume;
		source.Play ();
		clipsPlaying.Add (source);
	}

	public void PlaySoundLoop(Sound clipNumber, float volume){
		AudioSource source = gameObject.AddComponent<AudioSource> ();
		source.clip = clips [(int)clipNumber];
		source.volume = volume;
		source.Play ();
		source.loop = true;
		clipsPlaying.Add (source);
	}

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

    public void EndAllSound()
    {
        AudioSource[] sources = gameObject.GetComponents<AudioSource>();
        for (int i = 0; i < sources.Length; i++)
        {
                EndSoundAbrupt(sources[i].clip.name);
        }
    }
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
