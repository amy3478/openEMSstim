using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ChemistryController : MonoBehaviour
{
	private bool water_built;
	private bool salt_built;

	public GameObject infoPanel;
	public GameObject resultPanel;

	void Start ()
	{
		salt_built = false;
		water_built = false;
	}


	void Update ()
	{
		if(!water_built && checkWater()) // if water is built
		{
//			Debug.Log("water built");
			//show green checkmark
			GameObject elem1 = GameObject.Find("Element1");
			if (elem1 != null)
			{
				elem1.GetComponent<Transform> ().Find ("MarkDone").gameObject.SetActive (true);
			}

			// water is now built
			water_built = true;
		}

		if(!salt_built && checkSalt()) //if salt is built
		{
//			Debug.Log ("salt built");
			//show green checkmark
			GameObject elem2 = GameObject.Find("Element2");
			if (elem2 != null) {
				elem2.GetComponent<Transform> ().Find ("MarkDone").gameObject.SetActive (true);
			}

			// salt is now built
			salt_built = true;

		}

		if(water_built && salt_built)
		{
			Debug.Log("both is built!");

			infoPanel.SetActive (false);
			resultPanel.SetActive (true);
			Animator am = resultPanel.GetComponent<Animator> ();
			am.SetTrigger ("FadeIn");

			water_built = false;
			salt_built = false;
			//show success message and show back button
		}

	}

	public bool checkWater()
	{

		// Make sure that oxygen exists and that it has two hydrogen children
		GameObject [] goArray;
		goArray = GameObject.FindGameObjectsWithTag ("oxygenball");

		if(goArray.Length != 0)
		{
			foreach (GameObject go in goArray) { // for each gameobject that we find that matches this tag
				if (go.transform.childCount == 2) { // check if it has two children
					foreach (Transform child in go.transform) { // for each child
						if (child.gameObject.name != "HydrogenPrefab(Clone)") // check if it is a hydrogen
							break; //if child is not just hydrogen, fail
					}
					return true; //otherwise, success
				}
			}
		}
		return false;
	}

	public bool checkSalt()
	{
		// Make sure that sodium exists and that it has one chlorine children
		GameObject [] goArray;
		goArray = GameObject.FindGameObjectsWithTag ("sodiumball");

		if(goArray.Length != 0)
		{
			foreach (GameObject go in goArray) { // for each gameobject that we find that matches this tag
				if (go.transform.childCount == 1 && go.transform.GetChild(0).gameObject.name == "ChlorinePrefab(Clone)") { // check if it has one children
					return true; //otherwise, success
				}
			}
		}
		return false;
	}
}
