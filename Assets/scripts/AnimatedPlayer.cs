using UnityEngine;
using System.Collections;

public class AnimatedPlayer : Creature
{

    [SerializeField]
    private LayerMask collisionMask;

    // [SerializeField]
    // private ParticleSystem smoke;

    [SerializeField]
    private float jumpSpeed = .1f;
    [SerializeField]
    private float moveSpeed = 5;
    [SerializeField]
    private float gravity = -20;
    [SerializeField]
    private float terminalVelocity = -10;

    private float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;

    private BoxCollider2D boxCollider;
    private Renderer rend;
    // private Spawnable spawnable;
    private Animator animator;


    private const float skinWidth = .015f;

    private RaycastOrigins raycastOrigins;
    private Vector3 movement;

    private float velocityXSmoothing;

    // Status
    private bool alive = true;
    private bool grounded;
    private bool jumping;
    private bool bumpedHead;
    private bool leftWallSlide, rightWallSlide;

    private const float WALL_STICK_TIME = .25f;
    private float timeOnWall;

    private ParticleSystem smoke;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rend = GetComponent<Renderer>();
        // spawnable = GetComponent<Spawnable>();
        smoke = GetComponent<ParticleSystem>();

        animator = this.GetComponent<Animator>();

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

        SetPlayerAnimation();
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
        if (alive && Input.GetKeyDown(KeyCode.Space) && (grounded || leftWallSlide || rightWallSlide))
        {
            jumping = true;
            movement.y = jumpSpeed;

            // print("--------------------------jumping!!----------------------------");


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
            float targetVelocityX = (alive ? input.x : 0) * moveSpeed;
            movement.x = Mathf.SmoothDamp(movement.x, targetVelocityX, ref velocityXSmoothing, grounded ? accelerationTimeGrounded : accelerationTimeAirborne);
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
            // print("--------------------------wall hit!!----------------------------");
            movement.x = (hit.distance - skinWidth) * direction;

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
            leftWallSlide = true;
            jumping = false;
        }
        else
        {
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
            rightWallSlide = true;
            jumping = false;
        }
        else
        {
            rightWallSlide = false;
        }
    }


    public override void Kill(Cause cause)
    {
        if (!alive)
        {
            return;
        }

        switch (cause)
        {
            case Cause.Laser:
                smoke.Play();
                break;
            case Cause.Saw:
            case Cause.Spike:
                // TODO some blood particle effect
                break;
        }


        alive = false;

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1);

        movement.x = 0;
        movement.y = 0;

        smoke.Stop();
        // TODO stop the blood effect

        // spawnable.Spawn();

        alive = true;
    }


    void SetPlayerAnimation()
    {
        print(movement.x);

        if (Mathf.Abs(movement.x) < .1f)
        {
            // Always stays above 0 because of the damping thing.
            // Make a very small movement count as idle.
            animator.SetBool("movingLeft", false);
            animator.SetBool("movingRight", false);
        }
        else if (movement.x < 0)
        {
            animator.SetBool("movingLeft", true);
            animator.SetBool("movingRight", false);
        }
        else if (movement.x > 0)
        {
            animator.SetBool("movingLeft", false);
            animator.SetBool("movingRight", true);
        }

        if (leftWallSlide)
        {
            animator.SetBool("wallSlideLeft", true);
        }
        else if (rightWallSlide)
        {
            animator.SetBool("wallSlideRight", true);
        }
        else
        {
			animator.SetBool("wallSlideLeft", false);
			animator.SetBool("wallSlideRight", false);

            if (grounded)
            {
                animator.SetBool("jumping", false);
            }
            else
            {
                animator.SetBool("jumping", true);
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
