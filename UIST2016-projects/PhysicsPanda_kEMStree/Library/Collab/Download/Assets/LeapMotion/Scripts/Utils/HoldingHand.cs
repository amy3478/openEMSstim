using UnityEngine;
using System.Collections;
using Leap;

// Leap Motion hand script that detects pinches and grabs the closest rigidbody.
public class HoldingHand : MonoBehaviour {

	public enum HoldState {
		kReleased,
		kPrepared,
		kHold
	}

	public enum PinchState {
		kPinched,
		kReleased,
		kReleasing
	}

	// Preset fab that will use
	public GameObject oxygenPrefab;
	public GameObject hydrogenPrefab;
	public GameObject carbonPrefab;
	public GameObject sodiumPrefab;
	public GameObject chlorinePrefab;
	public GameObject calciumPrefab;
	public GameObject magnesiumPrefab;

	private GameObject clone;

	private GameObject prefab_to_instatiate;

	// Display ball position
	private Vector3 redSphereOPosition;
	private Vector3 whiteSphereOPosition;
	private Vector3 blackSphereOPosition;
	private Vector3 greenSphere2OPosition;
	private Vector3 pinkSphereOPosition;

	// Layers that we can grab.
	public LayerMask grabbableLayers = ~0;

	// Ratio of the length of the proximal bone of the thumb that will trigger a pinch.
	public float grabTriggerDistance = 0.2f;

	// Ratio of the length of the proximal bone of the thumb that will trigger a release.
	public float releaseTriggerDistance = 1.2f;

	// Maximum distance of an object that we can grab when pinching.
	public float grabObjectDistance = 4.0f;

	// If the object gets far from the pinch we'll break the bond.
	public float releaseBreakDistance = 0.3f;

	// Curve of the trailing off of strength as you release the object.
	public AnimationCurve releaseStrengthCurve;

	// Filtering the rotation of grabbed object.
	public float rotationFiltering = 0.4f;

	// Filtering the movement of grabbed object.
	public float positionFiltering = 0.4f;

	// Minimum tracking confidence of the hand that will cause a change of state.
	public float minConfidence = 0.1f;

	// Ball starting Y axis
	public float originYPosition = -2.12f;

	// Clamps the movement of the grabbed object.
	public Vector3 maxMovement = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
	public Vector3 minMovement = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);

	protected PinchState pinch_state_;
	protected Collider active_object_;
	protected Collider expired_object_;
	protected HoldState hold_state_;

	protected float last_max_angular_velocity_;
	protected Quaternion rotation_from_palm_;

	protected Vector3 current_pinch_position_;
	protected Vector3 filtered_pinch_position_;
	protected Vector3 object_pinch_offset_;
	protected Quaternion palm_rotation_;
	protected Vector3 palm_position_;

	void Start() {
		pinch_state_ = PinchState.kReleased;
		hold_state_ = HoldState.kReleased;
		active_object_ = null;
		expired_object_ = null;
		last_max_angular_velocity_ = 0.0f;
		rotation_from_palm_ = Quaternion.identity;
		current_pinch_position_ = Vector3.zero;
		filtered_pinch_position_ = Vector3.zero;
		object_pinch_offset_ = Vector3.zero;
		palm_rotation_ = Quaternion.identity;

		// Get the start ball position
		originYPosition =  GameObject.Find("redSphere").transform.position.y;

		redSphereOPosition = GameObject.Find ("redSphere").transform.position;
		whiteSphereOPosition = GameObject.Find ("whiteSphere").transform.position;
		blackSphereOPosition = GameObject.Find ("blackSphere").transform.position;
//		blueSphereOPosition = GameObject.Find ("blueSphere").transform.position;
//		greenSphereOPosition = GameObject.Find ("greenSphere").transform.position;
		greenSphere2OPosition = GameObject.Find ("greenSphere2").transform.position;
		pinkSphereOPosition = GameObject.Find ("pinkSphere").transform.position;


	}

	void OnDestroy() {
//		OnRelease();
	}

	// Finds the closest grabbable object within range of the pinch.
	protected Collider FindClosestGrabbableObject(Vector3 pinch_position) {
		Collider closest = null;
		float closest_sqr_distance = grabObjectDistance * grabObjectDistance;
		Collider[] close_things =
			Physics.OverlapSphere(pinch_position, grabObjectDistance, grabbableLayers);

		for (int j = 0; j < close_things.Length; ++j) {
			float sqr_distance = (pinch_position - close_things[j].transform.position).sqrMagnitude;

			// Finds the closest sqr distance in grabble 
			if (close_things[j].GetComponent<Rigidbody>() != null && sqr_distance < closest_sqr_distance &&
				!close_things[j].transform.IsChildOf(transform) &&
				close_things[j].tag != "NotGrabbable") {

				GrabbableObject grabbable = close_things[j].GetComponent<GrabbableObject>();
				if (grabbable == null || !grabbable.IsGrabbed()) {
					closest = close_things[j];
					closest_sqr_distance = sqr_distance;
				}
			}
		}

		return closest;
	}

	protected HoldState GetNewHoldState(){
		Collider hover = FindClosestGrabbableObject(current_pinch_position_);

		float sqr_distance = (current_pinch_position_ - hover.transform.position).sqrMagnitude;

		if (sqr_distance < 6 && sqr_distance > 1) 
		{
			return HoldState.kPrepared;
		}

		if (sqr_distance < 1) {
		
			return HoldState.kHold;
		}

		return HoldState.kReleased;

	}

	// Notify grabbable objects when they are ready to grab :)
	protected void Hover() {
		Collider hover = FindClosestGrabbableObject(current_pinch_position_);
			
		if (hover != active_object_ && active_object_ != null) {
			GrabbableObject old_grabbable = active_object_.GetComponent<GrabbableObject>();

			if (old_grabbable != null)
				old_grabbable.OnStopHover();
		}

		if (hover != null) {
			GrabbableObject new_grabbable = hover.GetComponent<GrabbableObject>();

			if (new_grabbable != null)
				new_grabbable.OnStartHover();
		}
		expired_object_ = active_object_;
		active_object_ = hover;
	}

	protected void SetBallBack() {
		if (expired_object_ == null)
			return;

		Vector3 backPos = new Vector3 (
			expired_object_.transform.position.x,
			originYPosition,
			expired_object_.transform.position.z
		);

		expired_object_.GetComponent<Rigidbody>().MovePosition (backPos);

	}

	// Put old expired ball back
	protected void SetBallPrepared () {
		if (active_object_ == null) {
			return;
		}
			
		// Setup
		Leap.Utils.IgnoreCollisions (gameObject, active_object_.gameObject, true);


		// Set expired ball back
		if(expired_object_ != null && expired_object_ != active_object_)
		{
			Vector3 backPos = new Vector3 (
				expired_object_.transform.position.x,
				originYPosition,
				expired_object_.transform.position.z
			);
				
			expired_object_.GetComponent<Rigidbody>().MovePosition (backPos);
		}

		// set new hovered ball position
		Vector3 newPos = new Vector3(
			active_object_.transform.position.x,
			-1.8f,
			active_object_.transform.position.z

		);
		active_object_.GetComponent<Rigidbody>().MovePosition (newPos);
			

	}

	protected void DestroyBall () {
		return;
	}

	protected void IgnoreCollisionsWithHands(GameObject gameObject,
											 GameObject to_ignore,
											 bool ignore = true)
	{
		Rigidbody[] boneObjects;

		Debug.Log ("I am in Ignore Collision");

		// ignore holdingHand object
		Leap.Utils.IgnoreCollisions (gameObject, to_ignore, true);

		// register ignore collisions for thumb
		GameObject thumbObject = GameObject.Find ("thumb");
		boneObjects = thumbObject.GetComponentsInChildren<Rigidbody> (thumbObject);
		for (int i = 0; i < boneObjects.Length; i++) {
			Leap.Utils.IgnoreCollisions (boneObjects[i].gameObject, to_ignore, true);
		}


		// register ignore collisions for index
		GameObject indexObject = GameObject.Find ("index");
		boneObjects = thumbObject.GetComponentsInChildren<Rigidbody> (indexObject);
		for (int i = 0; i < boneObjects.Length; i++) {
			Leap.Utils.IgnoreCollisions (boneObjects[i].gameObject, to_ignore, true);
		}

		// register ignore collisions for middle
		GameObject middleObject = GameObject.Find ("middle");
		boneObjects = thumbObject.GetComponentsInChildren<Rigidbody> (middleObject);
		for (int i = 0; i < boneObjects.Length; i++) {
			Leap.Utils.IgnoreCollisions (boneObjects[i].gameObject, to_ignore, true);
		}

		// register ignore collisions for pinky
		GameObject pinkyObject = GameObject.Find ("pinky");
		boneObjects = thumbObject.GetComponentsInChildren<Rigidbody> (pinkyObject);
		for (int i = 0; i < boneObjects.Length; i++) {
			Leap.Utils.IgnoreCollisions (boneObjects[i].gameObject, to_ignore, true);
		}

		// register ignore collisions for ring
		GameObject ringObject = GameObject.Find ("ring");
		boneObjects = thumbObject.GetComponentsInChildren<Rigidbody> (ringObject);
		for (int i = 0; i < boneObjects.Length; i++) {
			Leap.Utils.IgnoreCollisions (boneObjects[i].gameObject, to_ignore, true);
		}

		// register ignore collisions for palm
		GameObject palmObject = GameObject.Find ("palm");
		Leap.Utils.IgnoreCollisions (palmObject, to_ignore, true);

	}

	protected void StartHoldBall() {
		// No ball is detected
		if (active_object_ == null) {
			return;	
		}

//		active_object_.GetComponent<Rigidbody>().isKinematic = true;

//		Debug.Log ("Start Holding the ball");
//		Debug.Log (active_object_.gameObject.name);

		// Setup
		IgnoreCollisionsWithHands(gameObject, active_object_.gameObject, true);


		// Get the object name
		string go_name = active_object_.gameObject.name;

		// Create a new ball
		if (go_name == "redSphere")
		{
			prefab_to_instatiate = oxygenPrefab;
			active_object_.transform.position = redSphereOPosition;
		}

		if (go_name == "whiteSphere") 
		{
			prefab_to_instatiate = hydrogenPrefab;
			active_object_.transform.position = whiteSphereOPosition;
		}
		if (go_name == "blackSphere") 
		{
			prefab_to_instatiate = carbonPrefab;
			active_object_.transform.position = blackSphereOPosition;
		}
		if (go_name == "greenSphere2") 
		{
			prefab_to_instatiate = calciumPrefab;
			active_object_.transform.position = greenSphere2OPosition;
		}
		if (go_name == "pinkSphere") 
		{
			prefab_to_instatiate = magnesiumPrefab;
			active_object_.transform.position = pinkSphereOPosition;
		}

		expired_object_ = active_object_;
		SetBallBack ();

		if(prefab_to_instatiate != null)
		{
			GameObject go = (GameObject)Instantiate(prefab_to_instatiate, current_pinch_position_, Quaternion.identity);
			active_object_ = go.GetComponent<Collider> ();
			IgnoreCollisionsWithHands (gameObject, go, true);
//			Leap.Utils.IgnoreCollisions (gameObject, go, true);
		}
		hold_state_ = HoldState.kHold;
	}

	protected void BallHolding ()
	{
		if (active_object_ == null)
		{
			return;
		}
		active_object_.transform.position = current_pinch_position_;

		Leap.Utils.IgnoreCollisions (gameObject, active_object_.gameObject, true);

		// set display ball back
		if (active_object_.gameObject.name == "redSphere") {
			active_object_.transform.position = redSphereOPosition;
			Hover ();
		}
		if (active_object_.gameObject.name == "whiteSphere") {
			active_object_.transform.position = whiteSphereOPosition;
			Hover ();
		}
		if (active_object_.gameObject.name == "blackSphere") {
			active_object_.transform.position = blackSphereOPosition;
			Hover ();
		}
		if (active_object_.gameObject.name == "greenSphere2") {
			active_object_.transform.position = greenSphere2OPosition;
			Hover ();
		}
		if (active_object_.gameObject.name == "pinkSphere") {
			active_object_.transform.position = pinkSphereOPosition;
			Hover ();
		}


	}



	protected void OnRelease() {

		active_object_ = null;

		Hover();
	}

	protected void UpdatePinchPosition() {
		HandModel hand_model = GetComponent<HandModel>();
		current_pinch_position_ = 0.5f * (hand_model.fingers[0].GetTipPosition() + 
			hand_model.fingers[1].GetTipPosition());

		Vector3 delta_pinch = current_pinch_position_ - filtered_pinch_position_;
		filtered_pinch_position_ += (1.0f - positionFiltering) * delta_pinch;
	}

	protected void UpdatePalmRotation() {
		HandModel hand_model = GetComponent<HandModel>();
		palm_rotation_ = Quaternion.Slerp(palm_rotation_, hand_model.GetPalmRotation(),
			1.0f - rotationFiltering);
	}

	void FixedUpdate() {
		UpdatePalmRotation();
		UpdatePinchPosition();
		HandModel hand_model = GetComponent<HandModel>();
		Hand leap_hand = hand_model.GetLeapHand();

		if (leap_hand == null)
			return;

		HoldState new_hold_state = GetNewHoldState ();


		if (hold_state_ == HoldState.kReleased) {
			// kReleased --> kPrepared, the display ball need to be with hand
			if (new_hold_state == HoldState.kPrepared) {
				SetBallPrepared ();
			}
			// kReleased --> kReleased, keep hovering
			else {
				Hover ();
			}
		} else if (hold_state_ == HoldState.kPrepared) {
			// kPrepared --> kPrepared, check whether need to change the ball
			if (new_hold_state == HoldState.kPrepared) {
				SetBallPrepared ();
			}
			// kPrepared --> kHold, create a new ball
			if (new_hold_state == HoldState.kHold) {
//				Debug.Log ("Creating New ball");
				StartHoldBall ();
				hold_state_ = HoldState.kHold;
				new_hold_state = HoldState.kHold;
			}
			if(new_hold_state == HoldState.kReleased) {
				SetBallBack();
			}
		} else if (hold_state_ == HoldState.kHold) {
			// TODO: kHold - kReleased
			// Put Trash bin code here.

			// kHold --> kHold
			if( new_hold_state == HoldState.kHold) {
				BallHolding ();
			}

		} else {
			// Hover will check the closest active_object_
			Hover ();
		}
	
		hold_state_ = new_hold_state;
		if (active_object_ != expired_object_) {
			SetBallBack ();
		}
			
	}
}
