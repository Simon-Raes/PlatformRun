using UnityEngine;

public class Ray : MonoBehaviour
{
    public LayerMask collisionMask;

    RaycastOrigins raycastOrigins;
    Vector3 movement;
    const float skinWidth = .015f;

    float velocityXSmoothing;

    public float jumpSpeed = .1f;
    public float moveSpeed = 5;
    public float gravity = -20;
    public float terminalVelocity = -10;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;

    private BoxCollider2D boxCollider;
    private Renderer rend;

    private bool grounded;
    private bool jumping;
    private bool bumpedHead;
    private bool leftWallSlide, rightWallSlide;

    private const float WALL_STICK_TIME = .25f;
    private float timeOnWall;

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

        // if (leftWallSlide || rightWallSlide)
        // {
        //     print ("set movement to 0 because of wall slide");
        //     movement.x = 0;
        // }

        RegisterInputs();

        AttemptMove(movement * Time.deltaTime);

        SetPlayerDebugColor();
    }

    // private void ResetCollisions()
    // {
    //     leftWallSlide = false;
    //     rightWallSlide = false;
    //     grounded = false;
    //     bumpedHead = false;
    // }

    private void RegisterInputs()
    {
        RegisterVerticalInputs();
        RegisterHorizontalInputs();
    }

    private void RegisterVerticalInputs()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && (grounded || leftWallSlide || rightWallSlide))
        {
            jumping = true;
            movement.y = jumpSpeed;

            print("--------------------------jumping!!----------------------------");


            if (leftWallSlide || rightWallSlide)
            {
                int wallDirection = leftWallSlide ? -1 : 1;
                float inputDirection = Mathf.Sign(input.x);

                float xMovement;
                float yMovement;

                if (inputDirection == input.x)
                {
                    // hop up
                    xMovement = 1.5f * moveSpeed;
                    yMovement = 15;
                }
                else if (input.x == 0)
                {
                    // hop off
                    xMovement = 1.5f * moveSpeed;
                    yMovement = 0;
                }
                else
                {
                    // jump faaar
                    xMovement = 2.5f * moveSpeed;
                    yMovement = 8;
                }






                if (leftWallSlide)
                {
                    leftWallSlide = false;
                    movement.x = xMovement;
                }
                else if (rightWallSlide)
                {
                    rightWallSlide = false;
                    movement.x = -xMovement;
                }
                movement.y = yMovement;
            }
        }

        movement.y += gravity * Time.deltaTime;
    }

    private void RegisterHorizontalInputs()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        bool stuckOnWall = IsStuckOnWall(input.x);

        if (!stuckOnWall)
        {
            // movement.x = input.x * moveSpeed;
            print("lerping from " + movement.x + " to " + input.x * moveSpeed);

            float targetVelocityX = input.x * moveSpeed;
            movement.x = Mathf.SmoothDamp(movement.x, targetVelocityX, ref velocityXSmoothing, grounded ? accelerationTimeGrounded : accelerationTimeAirborne);
            print("result " + movement.x);
        }
    }

    private bool IsStuckOnWall(float xInput)
    {
        if (jumping || (!leftWallSlide && !rightWallSlide))
        {
            timeOnWall = 0;
            return false;
        }

        float inputDirection = Mathf.Sign(xInput);
        float requiredPushDirection = leftWallSlide ? 1 : rightWallSlide ? -1 : 0;

        if (xInput == 0)
        {
            timeOnWall = 0;
            return false;
        }

        if (inputDirection == requiredPushDirection)
        {
            // Pushing away from the wall, count down to unsticking
            timeOnWall += Time.deltaTime;
        }
        else
        {
            // Not pushing away, reset unstick timer.
            timeOnWall = 0;
        }

        print("time on wall" + timeOnWall);
        return timeOnWall < WALL_STICK_TIME;
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

        if (leftWallSlide || rightWallSlide)
        {
            print("set movement to 0 because of wall slide");
            movement.x = 0;
        }
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

            jumping = false;

            if (direction < 0)
            {
                grounded = true;
            }
            else
            {
                bumpedHead = true;
            }
        }
        else
        {
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
            print("--------------------------wall hit!!----------------------------");
            movement.x = (hit.distance - skinWidth) * direction;
            print("set movement to " + movement.x + " because of wall hit");

            jumping = false;

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
            else
            {
                leftWallSlide = false;
                rightWallSlide = false;
            }
        }
        else
        {
            leftWallSlide = false;
            rightWallSlide = false;
        }
    }


    private void CheckForWallSlideLeft()
    {
        float rayLength = skinWidth + .01f;

        Vector2 origin = raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.left, rayLength, collisionMask);

        if (!hit)
        {
            origin = raycastOrigins.topLeft;
            hit = Physics2D.Raycast(origin, Vector2.left, rayLength, collisionMask);
        }

        if (movement.x > 0 && hit.distance > 0)
        {
            leftWallSlide = false;
            return;
        }

        if (hit && !grounded && hit.distance <= rayLength)
        {
            print("found wall slide left!");
            leftWallSlide = true;
            jumping = false;
        }
        else
        {
            print("nah we ain't sliding left!");
            leftWallSlide = false;
        }
    }

    private void CheckForWallSlideRight()
    {
        float rayLength = skinWidth + .01f;

        Vector2 origin = raycastOrigins.bottomRight;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right, rayLength, collisionMask);

        if (!hit)
        {
            origin = raycastOrigins.topRight;
            hit = Physics2D.Raycast(origin, Vector2.right, rayLength, collisionMask);
        }

        if (movement.x < 0 && hit.distance > 0)
        {
            rightWallSlide = false;
            return;
        }

        if (hit && !grounded && hit.distance <= rayLength)
        {
            print("found wall slide right!");
            rightWallSlide = true;
            jumping = false;
        }
        else
        {
            print("nah we ain't sliding right!");
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
