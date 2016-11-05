using System;
using UnityEngine;

public class Saw : InstakillObstacle
{
    public Material sawBlood;

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

    protected override void CreatureKilled()
    {
        renderer.material = sawBlood;
    }
}
