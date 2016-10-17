using UnityEngine;
using System.Collections;

public class CalciumCollider : MonoBehaviour
{
	private int max_oxygens = 3;
	private int num_oxygens;

	private int max_carbons = 1;
	private int num_carbons;

	// Use this for initialization
	void Start ()
	{
		num_oxygens = 0;
		num_carbons = 0;
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
		bool carbon_connected = false;

		// Test if it is oxygen that is coming in and check whether or not it already has a connection
		if(atom_entering.GetComponent<OxygenCollider>() != null)
			oxygen_connected = atom_entering.GetComponent<OxygenCollider>().getConnectedStatus();

		// Test if it is carbon that is coming in and check whether or not it already has a connection
		if(atom_entering.GetComponent<CarbonCollider>() != null)
			carbon_connected = atom_entering.GetComponent<CarbonCollider>().getConnectedStatus();

		if(atom_entering.name == "OxygenPrefab(Clone)" && num_oxygens < max_oxygens && !oxygen_connected)
		{
			// First, make the oxygen a child of this calcium atom.
			// Next, create a Fixed Joint component on the oxygen and stick it to the calcium
			atom_entering.transform.parent = this_atom.transform;
			atom_entering.AddComponent<FixedJoint>();
            atom_entering.GetComponent<FixedJoint>().connectedBody = this_atom.GetComponent<Rigidbody>();

			// Now the oxygen is connected
			atom_entering.GetComponent<OxygenCollider>().setConnectedStatus(true);

			// Play the electrical "buzz" sound
			GetComponent<AudioSource>().Play();
			BlueEMS.active().buzz(0.2f);

			// Increment the number of oxygens connected to the calcium
			num_oxygens += 1;
		}

		if(atom_entering.name == "CarbonPrefab(Clone)" && num_carbons < max_carbons && !carbon_connected)
		{
			// First, make the carbon a child of this calcium atom.
			// Next, create a Fixed Joint component on the carbon and stick it to the calcium
			atom_entering.transform.parent = this_atom.transform;
			atom_entering.AddComponent<FixedJoint>();
            atom_entering.GetComponent<FixedJoint>().connectedBody = this_atom.GetComponent<Rigidbody>();

			// Now the carbon is connected
			atom_entering.GetComponent<CarbonCollider>().setConnectedStatus(true);

			// Play the electrical "buzz" sound
			GetComponent<AudioSource>().Play();
			BlueEMS.active().buzz(0.2f);

			// Increment the number of carbons connected to the calcium
			num_carbons += 1;
		}
	}
}
