using UnityEngine;
using System.Collections;

public class TurretGun : MonoBehaviour
{

    [SerializeField]
    private Transform bulletSpawnPoint;
    [SerializeField]
    private float fireRate = 1;
    [SerializeField]
    private float bulletSpeed = 2;
    [SerializeField]
    private Bullet bullet;
    [SerializeField]
    private float range = 5;


    private float timeSinceLastShot;



    // private GameObject target;



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		// TODO figure out why this ray always hits the barrel
		// debug ray looks fine and should be the same, no?
        RaycastHit2D ray = Physics2D.Raycast(transform.position + transform.up * 3, transform.up, range);
		
		Debug.DrawRay(transform.position + transform.up * 3, transform.up * 20, Color.red, 1f);

        if (ray)
        {
			print(ray.collider.gameObject);
            GameObject target = ray.collider.gameObject;

            if (target != null && target.tag.Equals("Player"))
            {
                if (timeSinceLastShot > fireRate)
                {
                    timeSinceLastShot = 0;
                    Bullet newBullet = GameObject.Instantiate(bullet, bulletSpawnPoint.transform.position, Quaternion.identity) as Bullet;

                    Vector3 bulletDirection = transform.up;
                    bulletDirection *= bulletSpeed;
                    newBullet.SetDirection(bulletDirection);
                }
                else
                {
                    timeSinceLastShot += Time.deltaTime;
                }
            }
        }
    }

}
