using UnityEngine;

public class Spike : MonoBehaviour
{


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
            creature.Kill(Cause.Spike);
            // TODO blood texture, but can't cover the entire spike thing mhhmm

        }
    }
}
