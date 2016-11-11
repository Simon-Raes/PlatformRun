using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{
    public ParticleSystem smoke;

    private LineRenderer lineRenderer;

    private ParticleSystem smokeObj;

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
            RenderLaser(ray);
            RenderSmoke(ray);

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

    private void RenderLaser(RaycastHit2D ray)
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, ray.point);
    }

    private void RenderSmoke(RaycastHit2D ray)
    {
        if (smokeObj == null)
        {
            smokeObj = GameObject.Instantiate(smoke, transform.position, Quaternion.identity) as ParticleSystem;
        }
        smokeObj.transform.position = ray.point;
    }
}
