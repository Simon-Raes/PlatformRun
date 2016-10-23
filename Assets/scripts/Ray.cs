using UnityEngine;

public class Ray : MonoBehaviour
{
    public LayerMask collisionMask;
    BoxCollider2D boxCollider;
    RaycastOrigins raycastOrigins;
    Vector3 movement;
    const float skinWidth = .015f;


    public float jumpSpeed = .1f;
    public float moveSpeed = 5;
    public float gravity = -20;
    public float terminalVelocity = -10;

	public bool grounded;


    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
		if(grounded)
		{
			movement.y = 0;
		}

		grounded = false;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        movement.x = input.x * moveSpeed;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.y = jumpSpeed;
        }

        movement.y += gravity * Time.deltaTime;

        AttemptMove(movement * Time.deltaTime);
    }

    void AttemptMove(Vector3 movement)
    {
        UpdateRaycastOrigins();

		// CapAtTerminalVelocity(ref movement);
        CheckHorizontalCollisions(ref movement);
        CheckVerticalCollisions(ref movement);

        transform.Translate(movement);
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    private void CheckHorizontalCollisions(ref Vector3 movement)
    {
        float direction = Mathf.Sign(movement.x);
        float rayLength = Mathf.Abs(movement.x) + skinWidth;

        Vector2 origin = direction < 0 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, rayLength, collisionMask);

        if (!hit)
        {
            // No hit at the bottom, check the top corner
            origin = direction < 0 ? raycastOrigins.topLeft : raycastOrigins.topRight;
            hit = Physics2D.Raycast(origin, Vector2.right * direction, rayLength, collisionMask);
        }

        if (hit)
        {
            print("horizontal hit");
            movement.x = (hit.distance - skinWidth) * direction;
        }
    }

    private void CheckVerticalCollisions(ref Vector3 movement)
    {
        float direction = Mathf.Sign(movement.y);
        float rayLength = Mathf.Abs(movement.y) + skinWidth;

        Vector2 origin = direction < 0 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up * direction, rayLength, collisionMask);

        if (!hit)
        {
            // No hit at the left, check the right corner
            origin = direction < 0 ? raycastOrigins.bottomRight : raycastOrigins.topRight;
            hit = Physics2D.Raycast(origin, Vector2.up * direction, rayLength, collisionMask);
        }

        if (hit)
        {
            print("vertical hit");
            movement.y = (hit.distance - skinWidth) * direction;

			grounded = true;
        }
    }

    private void CapAtTerminalVelocity(ref Vector3 movement)
    {
			if(movement.y < terminalVelocity)
			{
				movement.y = terminalVelocity;
			}
    }

    // private RaycastHit2D CheckVerticalHit(Vector2 origin)
    // {
    //    Vector2 origin = direction < 0 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
    //    return Physics2D.Raycast(origin, Vector2.up * direction, rayLength, collisionMask);
    // }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
