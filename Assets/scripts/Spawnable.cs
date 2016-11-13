using UnityEngine;
using System.Collections;

public class Spawnable : MonoBehaviour {

	[SerializeField]
	private Transform spawnPoint;

	// Use this for initialization
	void Start () {
		Spawn();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Spawn()
	{
		transform.position = spawnPoint.position;
	}
}
