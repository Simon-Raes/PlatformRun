using UnityEngine;

public class Mover : MonoBehaviour
{

    public float speed;
    public Transform start;
    public Transform end;

	private Vector3 target;

    // Use this for initialization
    void Start()
    {
		transform.position = start.position;
		target = end.position;
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

		if(Vector3.Distance(transform.position, target) <= .1f)
		{
			target = target == start.position ? end.position : start.position;
		}
    }
}
