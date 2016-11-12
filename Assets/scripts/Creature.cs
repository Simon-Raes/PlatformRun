using UnityEngine;

public class Creature : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Kill(Cause cause){
		print("arghgg I die");
		GameObject.Destroy(gameObject);
	}
}
