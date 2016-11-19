using UnityEditor;
using UnityEngine;

// TODO just remove the old mover once this is finished
// old mover functionality should also be possible with this new one

[ExecuteInEditMode]
public class PathMover : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private Transform[] points;

    private int movingTowards = 0;
    private Vector3 target;

    void Start()
    {
        transform.position = points[movingTowards].position;

        movingTowards++;

        target = points[movingTowards].position;
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        if (Vector3.Distance(transform.position, target) == 0)
        {
            if (movingTowards < points.Length - 1)
            {
                movingTowards++;
            }
            else
            {
                movingTowards = 0;
            }

            target = points[movingTowards].position;
        }

		Rotate();
    }

    private void Rotate()
    {
        int previousIndex = movingTowards == 0 ? points.Length - 1 : movingTowards - 1;
        Vector3 startPos = points[previousIndex].position;
        Vector3 currentPos = gameObject.transform.position;

        float distanceMoved = Vector3.Distance(startPos, currentPos);
        float totalDistance = Vector3.Distance(startPos, target);

		float percentageMoved = distanceMoved / totalDistance;

		transform.rotation = Quaternion.Lerp(points[previousIndex].transform.rotation, points[movingTowards].transform.rotation, percentageMoved);
    }

    // Draw the Mover's path in the scene editor
    void OnDrawGizmos()
    {
        if (points == null || points.Length < 2)
            return;

        for (int i = 0; i < points.Length - 1; i++)
        {
            Handles.DrawDottedLine(points[i].position, points[i + 1].position, 4);
        }

        // Close the loop
        Handles.DrawDottedLine(points[points.Length - 1].position, points[0].position, 4);
    }
}
