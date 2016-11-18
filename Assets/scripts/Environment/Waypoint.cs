using UnityEngine;

public class Waypoint : MonoBehaviour {

	void OnDrawGizmos() {
        Gizmos.DrawIcon(transform.position, "waypoint.png", true);
    }
}
