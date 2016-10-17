using UnityEngine;
using System.Collections;

public class BlueEMS : MonoBehaviour {

	private AndroidJavaObject mod_fatctory = null;
	private AndroidJavaObject activityContext = null;
	private AndroidJavaObject ems_mod = null;

	public static BlueEMS active() {
		return ACTIVE;
	}
	private static BlueEMS ACTIVE = null;
	// Use this for initialization
	void Start () {
		ACTIVE = this;
		Debug.Log ("Querying unity 3d context");
		if (null != mod_fatctory)
			return;
		using(AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
		}
		using(AndroidJavaClass pluginClass = new AndroidJavaClass("com.openems.stim.EMSFactory")) {
			if (pluginClass != null) {
				mod_fatctory = pluginClass.CallStatic<AndroidJavaObject> ("inst");
				mod_fatctory.Call ("setContext", activityContext);
				activityContext.Call ("runOnUiThread", new AndroidJavaRunnable (() => {
					Debug.Log ("Showing text in another thread");
					mod_fatctory.Call ("showMessage", "Creating EMS module...");
					Debug.Log("Angthing like ahhhhh...");
					ems_mod = mod_fatctory.Call<AndroidJavaObject> ("createModule");
//					mod_fatctory.Call("showMessage", "ems_mod: " + (null == ems_mod ? "NULL" : "Not NULL"));
				}));
			} else {
				Debug.Log("blue_ems could not find android EMSFactory class");
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		//		Debug.Log ("mod_factory: " + (null == mod_fatctory ? "NULL" : "Not NULL"));
//		Debug.Log("EMS: " + (null == ems_mod ? "NULL" : "Not NULL"));
	}

	IEnumerator stopChannel(int ch, float seconds) {
		yield return new WaitForSeconds(seconds);
		ems_mod.Call("stopCommand", ch);
	}

	public void buzz(float seconds) {
		if (null == ems_mod)
			return;
		ems_mod.Call("startCommand", 0);
		ems_mod.Call("startCommand", 1);
		StartCoroutine(stopChannel(0, seconds));
		StartCoroutine(stopChannel(1, seconds));
	}
}
