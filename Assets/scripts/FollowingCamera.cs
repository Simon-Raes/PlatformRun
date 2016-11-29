using UnityEngine;
using System.Collections;

public class FollowingCamera : MonoBehaviour {

	[SerializeField]
	private GameObject target;

	[SerializeField]
	private bool lockedX;

	[SerializeField]
	private bool lockedY;

	private float offset;

	// Use this for initialization
	void Start () {
		// need this as a vector, not float
		offset = Vector3.Distance(transform.position, target.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
