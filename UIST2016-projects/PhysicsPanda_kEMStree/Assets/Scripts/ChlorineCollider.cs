using UnityEngine;
using System.Collections;

public class ChlorineCollider : MonoBehaviour
{
	public bool connected;

	// Use this for initialization
	void Start ()
	{
		connected = false;
	}

	public bool getConnectedStatus()
	{
		return connected;
	}

	public void setConnectedStatus(bool new_status)
	{
		connected = new_status;
	}

}
