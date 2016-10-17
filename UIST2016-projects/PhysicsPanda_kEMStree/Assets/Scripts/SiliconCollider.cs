using UnityEngine;
using System.Collections;

public class SiliconCollider : MonoBehaviour
{
	private int max_oxygens = 2;
	private int num_oxygens;

	// Use this for initialization
	void Start ()
	{
		num_oxygens = 0;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	void OnCollisionEnter(Collision collision)
	{
		GameObject atom_entering = collision.gameObject;
		GameObject this_atom = this.gameObject;

		bool oxygen_connected = false;

		// Test if it is oxygen that is coming in and check whether or not it already has a connection
		if(atom_entering.GetComponent<OxygenCollider>() != null)
			oxygen_connected = atom_entering.GetComponent<OxygenCollider>().getConnectedStatus();

		if(atom_entering.name == "OxygenPrefab(Clone)" && num_oxygens < max_oxygens && !oxygen_connected)
		{
			// First, make the oxygen a child of this magnesium atom.
			// Next, create a Fixed Joint component on the oxygen and stick it to the silicon
			atom_entering.transform.parent = this_atom.transform;
			atom_entering.AddComponent<FixedJoint>();
            atom_entering.GetComponent<FixedJoint>().connectedBody = this_atom.GetComponent<Rigidbody>();

			// Now the oxygen is connected
			atom_entering.GetComponent<OxygenCollider>().setConnectedStatus(true);

			// Play the electrical "buzz" sound
			GetComponent<AudioSource>().Play();
			BlueEMS.active().buzz(0.2f);

			// Increment the number of oxygens connected to the silicon
			num_oxygens += 1;
		}
	}
}
