using UnityEngine;
using System.Collections;

public class AtomBehavior : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}

	// Drag the atom around the desk (using mouse)
	// Turn off rigidbody while doing so because otherwise, the physics engine updating
	// gets pretty crazy (disabling rigidbody by turning on/off isKinematic respectively)
	void OnMouseDrag()
	{
		this.GetComponent<Rigidbody>().isKinematic = true;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane))
		{
			float oldY = this.transform.position.y;
			this.transform.position = new Vector3(hit.point.x, oldY, hit.point.z);
		}
		this.GetComponent<Rigidbody>().isKinematic = false;
	}

}
