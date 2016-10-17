using UnityEngine;
using System.Collections;

public class UpdateSkinMesh : MonoBehaviour {

	public SkinnedMeshRenderer meshRenderer;
	public MeshCollider collider;

	void Start() {
		
	}

	void Update() {
		UpdateCollider ();
	}

	public void UpdateCollider() {
		Mesh colliderMesh = new Mesh ();
		meshRenderer.BakeMesh (colliderMesh);
		collider.sharedMesh = null;
		collider.sharedMesh = colliderMesh;
	}
}
