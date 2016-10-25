using UnityEngine;

public class Ray : MonoBehaviour
{
    public LayerMask collisionMask;

    RaycastOrigins raycastOrigins;
    Vector3 movement;
    const float skinWidth = .015f;


    public float jumpSpeed = .1f;
    public float moveSpeed = 5;
    public float gravity = -20;
    public float terminalVelocity = -10;

    private BoxCollider2D boxCollider;
    private Renderer rend;

    private bool grounded;
    private bool bumpedHead;
    private bool leftWallSlide, rightWallSlide;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (grounded || bumpedHead)
        {
            movement.y = 0;
        }

        // ResetCollisions();

        RegisterInputs();

        // CheckForWallSlideLeft();
        // CheckForWallSlideRight();

        AttemptMove(movement * Time.deltaTime);

        SetPlayerDebugColor();
    }

    private void ResetCollisions()
    {
        leftWallSlide = false;
        rightWallSlide = false;
        grounded = false;
        bumpedHead = false;
    }

    private void RegisterInputs()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        movement.x = input.x * moveSpeed;

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && (grounded || leftWallSlide || rightWallSlide))
        {
            movement.y = jumpSpeed;
        }

        movement.y += gravity * Time.deltaTime;
    }

    private void CheckForWallSlideLeft()
    {
        float rayLength = .1f;

        Vector2 origin = raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.left, rayLength, collisionMask);

        if (hit && !grounded && hit.distance < .1f)
        {
            leftWallSlide = true;
        }
		else
		{
			leftWallSlide = false;
		}
    }

    private void CheckForWallSlideRight()
    {
        float rayLength = .1f;

        Vector2 origin = raycastOrigins.bottomRight;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right, rayLength, collisionMask);

        if (hit && !grounded && hit.distance < .1f)
        {
            rightWallSlide = true;
        }
		else{
			rightWallSlide = false;
		}
    }

    void SetPlayerDebugColor()
    {
        if (leftWallSlide)
        {
            rend.material.color = Color.blue;
        }
        else if (rightWallSlide)
        {
            rend.material.color = Color.green;
        }
        else
        {
            if (grounded)
            {
                rend.material.color = Color.red;
            }
            else
            {
                rend.material.color = Color.cyan;
            }
        }

    }

    void AttemptMove(Vector3 movement)
    {
        UpdateRaycastOrigins();

        // CapAtTerminalVelocity(ref movement);
        CheckVerticalCollisions(ref movement);

	

        CheckHorizontalCollisions(ref movement);

	CheckForWallSlideLeft();
        CheckForWallSlideRight();
		
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
            movement.y = (hit.distance - skinWidth) * direction;

            if (direction < 0)
            {
                grounded = true;
            }
            else
            {
                bumpedHead = true;
            }
        }
		else{
			grounded = false;
			bumpedHead = false;
		}
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
            movement.x = (hit.distance - skinWidth) * direction;

            // Can't wallslide if we're on the floor
            if (!grounded)
            {
                if (direction < 0)
                {
                    leftWallSlide = true;
                }
                else if (direction > 0)
                {
                    rightWallSlide = true;
                }
            }
			else{
				leftWallSlide = false;
				rightWallSlide = false;
			}
        }
		else{
			leftWallSlide = false;
			rightWallSlide = false;
		}
    }



    private void CapAtTerminalVelocity(ref Vector3 movement)
    {
        if (movement.y < terminalVelocity)
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
