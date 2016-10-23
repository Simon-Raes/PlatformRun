using UnityEngine;
using System.Collections;

public class Controller2D : MonoBehaviour
{

	public LayerMask collisionMask;

	const float skinWidth = .015f;
	public int horizontalRayCount = 4;



	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 17;

	float gravity;
	float jumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;


	BoxCollider2D collider;
	RaycastOrigins raycastOrigins;
	// public CollisionInfo collisions;

	void Start ()
	{
		collider = GetComponent<BoxCollider2D> ();
		
			// controller = GetComponent<Controller2D> ();

		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		print ("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
	}

	// void Update()
	// {
	// 	Move ();	
	// }

void Update() {

		// if (collisions.above || collisions.below) {
		// 	velocity.y = 0;
		// }

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		// if (Input.GetKeyDown (KeyCode.Space) && collisions.below) {
		// 	velocity.y = jumpVelocity;
		// }

		// float targetVelocityX = input.x * moveSpeed;
		// velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		velocity.x = input.x * moveSpeed;
		// velocity.y += gravity * Time.deltaTime;
		Move (velocity * Time.deltaTime);

	}

	public void Move (Vector3 velocity)
	{
		UpdateRaycastOrigins ();
		// collisions.Reset ();

		// Debug.DrawRay (raycastOrigins.topLeft, Vector2.right * -1 * 3, Color.blue);


		// Vector2 rayOrigin = raycastOrigins.topLeft;
		// rayOrigin.y += .1f;

		// Debug.DrawRay (rayOrigin, Vector2.right * -1 * 3, Color.red);




		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}
		// if (velocity.y != 0) {
		// 	VerticalCollisions (ref velocity);
		// }

		transform.Translate (velocity);
	}

	void HorizontalCollisions (ref Vector3 velocity)
	{
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
		RaycastHit2D hitBottom = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

		Debug.DrawRay (rayOrigin, Vector2.right * directionX * rayLength, Color.red);

		if (hitBottom) {
			velocity.x = (hitBottom.distance - skinWidth) * directionX;
			rayLength = hitBottom.distance;

			// collisions.left = directionX == -1;
			// collisions.right = directionX == 1;

			Debug.DrawRay (rayOrigin, Vector2.right * directionX * rayLength, Color.red);

		}
		//  else 
		//  {

		// 	// Assuming no part of the level will be smaller than the charecter


		// 	rayOrigin = (directionX == -1) ? raycastOrigins.topLeft : raycastOrigins.topRight;
		// 	RaycastHit2D hitTop = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

		// 	Debug.DrawRay (rayOrigin, Vector2.right * directionX * rayLength, Color.red);

		// 	if (hitTop) {
		// 		velocity.x = (hitTop.distance - skinWidth) * directionX;
		// 		rayLength = hitTop.distance;

		// 		collisions.left = directionX == -1;
		// 		collisions.right = directionX == 1;
		// 	}
		// }
	}

	// void VerticalCollisions (ref Vector3 velocity)
	// {
	// 	// Get movement direction (up or down)
	// 	float directionY = Mathf.Sign (velocity.y);
	// 	float rayLength = Mathf.Abs (velocity.y) + skinWidth;


	// 	Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
	// 	RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

	// 	Debug.DrawRay (rayOrigin, Vector2.up * directionY * rayLength, Color.red);

	// 	if (hit) {
	// 		velocity.y = (hit.distance - skinWidth) * directionY;
	// 		rayLength = hit.distance;

	// 		collisions.below = directionY == -1;
	// 		collisions.above = directionY == 1;
	// 	} else {

	// 		rayOrigin = (directionY == -1) ? raycastOrigins.bottomRight : raycastOrigins.topRight;
	// 		RaycastHit2D hitRight = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

	// 		Debug.DrawRay (rayOrigin, Vector2.up * directionY * rayLength, Color.red);

	// 		if (hitRight) {
	// 			velocity.y = (hitRight.distance - skinWidth) * directionY;
	// 			rayLength = hitRight.distance;

	// 			collisions.below = directionY == -1;
	// 			collisions.above = directionY == 1;
	// 		}
	// 	}
	// }

	void UpdateRaycastOrigins ()
	{
		Bounds bounds = collider.bounds;
//		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	// public struct CollisionInfo
	// {
	// 	public bool above, below;
	// 	public bool left, right;

	// 	public void Reset ()
	// 	{
	// 		above = below = false;
	// 		left = right = false;
	// 	}
	// }
}
