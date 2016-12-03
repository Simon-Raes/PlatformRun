using UnityEngine;
using System.Collections;

/*
* Shoots out a raycast straight ahead, shoots bullet when a player passed through it. 
*/


public class TurretGun : MonoBehaviour
{
    [SerializeField]
    private float fireRate = 1;
    [SerializeField]
    private float bulletSpeed = 2;
    [SerializeField]
    private Bullet bullet;
    [SerializeField]
    private float range = 5;


    private float timeSinceLastShot;



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.up, 20);

        if (ray)
        {
			print(ray.collider.gameObject);
            GameObject target = ray.collider.gameObject;

            if (target != null && target.tag.Equals("Player"))
            {
                if (timeSinceLastShot > fireRate)
                {
                    timeSinceLastShot = 0;
                    Bullet newBullet = GameObject.Instantiate(bullet, transform.position, Quaternion.identity) as Bullet;

                    Vector3 bulletDirection = transform.up;
                    bulletDirection *= bulletSpeed;
                    newBullet.SetDirection(bulletDirection);
                }
            }
        }
		timeSinceLastShot += Time.deltaTime;
    }

}
