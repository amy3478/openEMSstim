using UnityEngine;
using System.Collections;

public class OxygenCollider : MonoBehaviour
{
	private int max_hydrogens = 2;
	private int num_hydrogens;

	public bool connected;


	// Use this for initialization
	void Start ()
	{
		connected = false;
		num_hydrogens = 0;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	void OnCollisionEnter(Collision collision)
	{
		GameObject atom_entering = collision.gameObject;
		GameObject this_atom = this.gameObject;

		bool hydrogen_connected = false;

		// Test if it is hydrogen that is coming in and check whether or not it already has a connection
		if(atom_entering.GetComponent<HydrogenCollider>() != null)
			hydrogen_connected = atom_entering.GetComponent<HydrogenCollider>().getConnectedStatus();

		if(atom_entering.name == "HydrogenPrefab(Clone)" && num_hydrogens < max_hydrogens && !hydrogen_connected)
		{
			// First, make the hydrogen a child of this oxygen atom.
			// Next, create a Fixed Joint component on the hydrogen and stick it to the oxygen
			atom_entering.transform.parent = this_atom.transform;
			atom_entering.AddComponent<FixedJoint>();
            atom_entering.GetComponent<FixedJoint>().connectedBody = this_atom.GetComponent<Rigidbody>();

			atom_entering.GetComponent<HydrogenCollider>().setConnectedStatus(true);

			// Play the electrical "buzz" sound
			GetComponent<AudioSource>().Play();
			BlueEMS.active().buzz(0.2f);


			// Increment the number of hydrogens connected to the oxygen
			num_hydrogens += 1;

		}
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
