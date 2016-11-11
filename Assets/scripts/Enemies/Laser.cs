using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{

    private LineRenderer lineRenderer;

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetVertexCount(2);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right);

        if (ray)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, ray.point);

			Player player = ray.collider.gameObject.GetComponent<Player>();

            if (player != null)
            {
				player.die();
            }

            // if(hit player)
            // {
            // kill him
            // }
        }
    }
}
