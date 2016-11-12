using System;
using UnityEngine;

public class Saw : MonoBehaviour
{
    public Material sawBlood;
    private new Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        rotate();
    }

    private void rotate()
    {
        transform.Rotate(0, 0, 6.0f * 90 * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        print("got " + other.gameObject);
        Creature creature = other.gameObject.GetComponent<Creature>();
        if (creature != null)
        {
            creature.Kill(Cause.Saw);
            renderer.material = sawBlood;
        }
    }
}
