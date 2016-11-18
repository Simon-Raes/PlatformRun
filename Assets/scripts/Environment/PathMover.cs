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

        if (Vector3.Distance(transform.position, target) <= .01f)
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
