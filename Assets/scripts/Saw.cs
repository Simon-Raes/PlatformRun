using UnityEngine;

public class Saw : MonoBehaviour
{
	// public Material saw;
	public Material sawBlood;

	private new Renderer renderer;

    // Use this for initialization
    void Start()
    {
		renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        rotate();
    }

	private void rotate(){
		transform.Rotate(0, 0, 6.0f * 90 * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D other) {
        Creature creature = other.gameObject.GetComponent<Creature>();
		if(creature != null){
			
			renderer.material = sawBlood;
			creature.die();
		}

    }
}
