using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour
{

    [SerializeField]
    private Transform target;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

        // print((transform.rotation.eulerAngles.z) + " - " + angle);
        // print(angle);


        // print(Quaternion.AngleAxis(angle, Vector3.forward));

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        print(Quaternion.AngleAxis(angle, Vector3.forward) + " - " + Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward));
    }
}
