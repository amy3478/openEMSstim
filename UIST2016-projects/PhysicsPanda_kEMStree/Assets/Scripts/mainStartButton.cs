using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class mainStartButton : MonoBehaviour {

	public string sceneToLoad;

	public void loadScene () {
		Debug.Log ("Button clicked");
		SceneManager.LoadScene (sceneToLoad);
	}
}
