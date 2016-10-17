using UnityEngine;
using System.Collections;


[RequireComponent(typeof(GvrAudioSource))]
public class buttonClickSound : MonoBehaviour {

	public AudioClip sound;
	AudioSource audio;

	void Start() {
		audio = GetComponent<AudioSource> ();
	}
	public void OnBtnClick() {
		audio.PlayOneShot (sound);
	}
		
}
