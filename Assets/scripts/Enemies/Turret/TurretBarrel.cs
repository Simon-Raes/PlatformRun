using UnityEngine;


public class TurretBarrel : MonoBehaviour
{
    [SerializeField]
    private float maxRotationSpeed = 20;

    private GameObject target;


    void Start()
    {

    }

    void Update()
    {
        if (target != null)
        {
            float currentRotation = transform.rotation.eulerAngles.z;


            Vector3 dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

            angle = positiviseAngle(angle);
            angle = adjustRotationForLimit(currentRotation, angle);


            // rotate aroun Z axis (=forward)

            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // print("we did it curr rot is now " + barrel.transform.rotation.z + " aka " + barrel.transform.rotation.eulerAngles.z);
            // print("---");


            // TODO raycast first to check if player is in sight. Needed for slow rotating turrets. 
            // TODO move to own script on the barrel?
            // would make it easy to make laser and bullet and flame and... turrets!

        }
    }

    // TODO not quite right yet, switches rotation angle around the top/0 angle, not around the current rotation
    private float adjustRotationForLimit(float currentRotation, float desiredRotation)
    {
        if (maxRotationSpeed > 0)
        {
            print(currentRotation + " vs " + desiredRotation + " und " + maxRotationSpeed * Time.deltaTime);
            if (Mathf.Abs(currentRotation - desiredRotation) > maxRotationSpeed * Time.deltaTime)
            {
                if ((desiredRotation - currentRotation + 360) % 360 < 180)
                {
                    desiredRotation = currentRotation + maxRotationSpeed * Time.deltaTime;
                }
                else
                {
                    desiredRotation = currentRotation - maxRotationSpeed * Time.deltaTime;
                }
            }
        }

        return desiredRotation;
    }

    private float positiviseAngle(float angle)
    {
        if (angle < 0)
        {
            angle += 360;
        }
        return angle;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (target == null)
        {
            print("got " + other.gameObject);
            target = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == target)
        {
            target = null;
        }
    }
}
