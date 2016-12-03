using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem smoke;
    [SerializeField]
    private ParticleSystem sparks;

    private LineRenderer lineRenderer;

    private ParticleSystem smokeObj;
    private ParticleSystem sparksObj;

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

        print("hit? " + ray.point);

        if (ray)
        {
            DrawParticles(ray);

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

    private void DrawParticles(RaycastHit2D ray)
    {
        DrawLaser(ray);
        DrawSparks(ray);
    }

    private void DrawSmoke(RaycastHit2D ray)
    {
        if (smokeObj == null)
        {
            smokeObj = GameObject.Instantiate(smoke, transform.position, Quaternion.identity) as ParticleSystem;
        }
        smokeObj.transform.position = ray.point;
    }

    private void DrawSparks(RaycastHit2D ray)
    {
        if (sparksObj == null)
        {
            sparksObj = GameObject.Instantiate(sparks, transform.position, Quaternion.identity) as ParticleSystem;
        }
        sparksObj.transform.position = ray.point;
    }


    // public void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawSphere(transform.position, 1f);
    // }
}
