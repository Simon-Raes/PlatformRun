using UnityEngine;

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
            DrawLaser(ray);

            Creature player = ray.collider.gameObject.GetComponent<Creature>();

            if (player != null)
            {
                player.Kill(Cause.Laser);
            }
            else
            {
                DrawSmoke(ray);
            }
        }
    }

    private void DrawLaser(RaycastHit2D ray)
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, ray.point);
    }

    private void DrawSmoke(RaycastHit2D ray)
    {
        if (smokeObj == null)
        {
            smokeObj = GameObject.Instantiate(smoke, transform.position, Quaternion.identity) as ParticleSystem;
        }
        smokeObj.transform.position = ray.point;
    }
}
