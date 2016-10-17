using UnityEngine;
using System.Collections;

public class SodiumCollider : MonoBehaviour
{
	private int max_chlorine = 1;
	private int num_chlorine;

	public bool connected;

	// Use this for initialization
	void Start ()
	{
		num_chlorine = 0;
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

	void OnCollisionEnter(Collision collision)
	{
		GameObject atom_entering = collision.gameObject;
		GameObject this_atom = this.gameObject;

		bool chlorine_connected = false;

		// Test if it is chlorine that is coming in and check whether or not it already has a connection
		if(atom_entering.GetComponent<ChlorineCollider>() != null)
			chlorine_connected = atom_entering.GetComponent<ChlorineCollider>().getConnectedStatus();

		if(atom_entering.name == "ChlorinePrefab(Clone)" && num_chlorine < max_chlorine && !chlorine_connected)
		{
			// First, make the chlorine a child of this sodium atom.
			// Next, create a Fixed Joint component on the chlorine and stick it to the sodium
			atom_entering.transform.parent = this_atom.transform;
			atom_entering.AddComponent<FixedJoint>();
			atom_entering.GetComponent<FixedJoint>().connectedBody = this_atom.GetComponent<Rigidbody>();

			atom_entering.GetComponent<ChlorineCollider>().setConnectedStatus(true);

			// Play the electrical "buzz" sound
			GetComponent<AudioSource>().Play();
			BlueEMS.active().buzz(0.2f);

			// Increment the number of hydrogens connected to the oxygen
			num_chlorine += 1;

		}
	}

}
