using UnityEngine;
using System.Collections;

public class ToggleParticleSystem : MonoBehaviour {

	public ParticleSystem particles;
	
	public void PlayParticles() {
		particles.Play ();
	}

	public void StopParticles() {
		particles.Stop ();
	}

}
