using UnityEngine;

public class Coin : MonoBehaviour
{

    [SerializeField]
    private float rotateSpeed = 50f;

    void Start()
    {

    }

    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject.Destroy(gameObject);

		// todo save this somewhere? let the player or gamemanager know about this?
    }
}
