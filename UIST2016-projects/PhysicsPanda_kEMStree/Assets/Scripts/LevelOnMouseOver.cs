using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class LevelOnMouseOver : MonoBehaviour {

	public CanvasGroup infoPanel;
	// Use this for initialization
	void Start () {
		infoPanel.alpha = 0f;
	}
	
	// Update is called once per frame
	public void onMouseOver () {
		infoPanel.alpha = 1f;
	}

	public void onMouseOut () {
		infoPanel.alpha = 0f;
	}
}
