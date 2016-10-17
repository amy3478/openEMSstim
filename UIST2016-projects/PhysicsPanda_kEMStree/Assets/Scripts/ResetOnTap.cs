using UnityEngine;
using System.Collections;

public class ResetOnTap : MonoBehaviour {

	void Update () {
		if (GvrViewer.Instance.Triggered) {
			ResetCenter ();
		}
	}

	void ResetCenter () {
		GvrViewer.Instance.Recenter ();
	}
}