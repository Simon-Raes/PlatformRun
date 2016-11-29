using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private Vector3 direction;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (direction != null)
        {
			transform.Translate(direction * Time.deltaTime);
        }
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }
}
