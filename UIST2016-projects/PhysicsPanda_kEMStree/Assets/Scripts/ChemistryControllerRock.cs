using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ChemistryControllerRock : MonoBehaviour
{
	private bool calcite_built;
	private bool quartz_built;

	public GameObject infoPanel;
	public GameObject resultPanel;

	void Start ()
	{
		calcite_built = false;
		quartz_built = false;
	}

	void Update ()
	{
		if(!calcite_built && checkCalcite()) // if calcite is built
		{
//			Debug.Log("calcite built");

			//show green checkmark
			GameObject elem1 = GameObject.Find("Element1");
			if (elem1 != null)
			{
				elem1.GetComponent<Transform> ().Find ("MarkDone").gameObject.SetActive (true);
			}

			// water is now built
			calcite_built = true;
		}

		if(!quartz_built && checkQuartz()) //if quartz is built
		{
//			Debug.Log ("magnesite built");

			//show green checkmark
			GameObject elem2 = GameObject.Find("Element2");
			if (elem2 != null) {
				elem2.GetComponent<Transform> ().Find ("MarkDone").gameObject.SetActive (true);
			}

			// quartz is now built
			quartz_built = true;

		}

		if(calcite_built && quartz_built)
		{
			Debug.Log("both is built!");

			infoPanel.SetActive (false);
			resultPanel.SetActive (true);
			Animator am = resultPanel.GetComponent<Animator> ();
			am.SetTrigger ("FadeIn");

			calcite_built = false;
			quartz_built = false;
		}

	}

	public bool checkCalcite()
	{
		GameObject [] goArray;
		goArray = GameObject.FindGameObjectsWithTag ("calciumball");
		int oxyCount = 0;
		int carbCount = 0;

		if(goArray.Length != 0)
		{
			foreach (GameObject go in goArray) // for each gameobject that we find that matches this tag
			{
				if (go.transform.childCount == 4) // check if it has four children
				{
					foreach (Transform child in go.transform) // for each child
					{
						if (child.gameObject.name == "OxygenPrefab(Clone)") // check if it is a oxygen
						{
							oxyCount++; // if so, increment count
						}
						if (child.gameObject.name == "CarbonPrefab(Clone)") // check if it is carbon
						{
							carbCount++; // if so, increment count
						}
					}
				}

				if(oxyCount == 3 && carbCount == 1) // if you have 3 oxygens and 1 carbon
					return true; // then you've built calcite, otherwise, you haven't.
				else
				{
					oxyCount = 0;
					carbCount = 0;
				}
			}
		}
		return false;

	}

	public bool checkQuartz()
	{
		GameObject [] goArray;
		goArray = GameObject.FindGameObjectsWithTag ("siliconball");
		int oxyCount = 0;

		if(goArray.Length != 0)
		{
			foreach (GameObject go in goArray) // for each gameobject that we find that matches this tag
			{
				if (go.transform.childCount == 2) // check if it has two children
				{
					foreach (Transform child in go.transform) // for each child
					{
						if (child.gameObject.name != "OxygenPrefab(Clone)") // check if it is a oxygen
							break; //if child is not just oxygen, fail
					}
					return true;
				}
			}
		}
		return false;
	}
}
