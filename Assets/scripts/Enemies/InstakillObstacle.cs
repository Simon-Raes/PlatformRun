using UnityEngine;

public abstract class InstakillObstacle : MonoBehaviour
{

    protected new Renderer renderer;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        print("got " + other.gameObject);
        Creature creature = other.gameObject.GetComponent<Creature>();
        if (creature != null)
        {
            creature.die();
			CreatureKilled();
        }
    }

    protected abstract void CreatureKilled();
}
