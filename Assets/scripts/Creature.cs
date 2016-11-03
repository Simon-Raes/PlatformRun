using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void die(){
		print("arghgg I die");
		GameObject.Destroy(gameObject);
	}
}
