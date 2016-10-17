using UnityEngine;
using System.Collections;

public class ShowComponents : MonoBehaviour {

	public GameObject gameObject;

	// Update is called once per frame
	public void show () {
		gameObject.SetActive(true);
	}
}
