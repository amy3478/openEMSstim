using UnityEngine;
using System.Collections;

public class HideComponents : MonoBehaviour {


	public GameObject gameObject;

	// Update is called once per frame
	public void hide () {
		gameObject.SetActive(false);
	}
}
