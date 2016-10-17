using UnityEngine;
using System.Collections;

public class MirrorRotation : MonoBehaviour {

	public Camera vrCamera;

	// Update is called once per frame
	void Update () {
		transform.rotation = vrCamera.transform.rotation;
//		transform.Rotate (new Vector3 (0f, 180f, 0f)); qq
	}
}
