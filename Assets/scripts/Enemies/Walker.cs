using UnityEngine;
using System.Collections;
using System;

public class Walker : Creature
{
    public LayerMask collisionMask;

    public bool staysOnPlatform;
    public float gravity = -20;
    public bool startMovementLeft;
    public float moveSpeed;

    RaycastOrigins raycastOrigins;
    Vector3 movement;
    const float skinWidth = .015f;
    private BoxCollider2D boxCollider;

    private bool reversing;
    private bool grounded;
    private bool hitEdge;


    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        movement.x = moveSpeed * (startMovementLeft ? -1 : 0);
    }


    void Update()
    {
        if (grounded)
        {
            movement.y = 0;
        }

        movement.y += gravity * Time.deltaTime;

        if (hitEdge)
        {
            // Invert movement (reset 1 or -1 because last movement might be a different value because of a wall collision);
            movement.x = moveSpeed * (Mathf.Sign(movement.x) > 0 ? -1 : 1);
            hitEdge = false;
        }

        AttemptMove(movement * Time.deltaTime);
    }

    void AttemptMove(Vector3 movement)
    {
        UpdateRaycastOrigins();

        // CapAtTerminalVelocity(ref movement);
        CheckVerticalCollisions(ref movement);
        CheckHorizontalCollisions(ref movement);

        transform.Translate(movement);
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
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        // return movement;
    }

    private void CheckHorizontalCollisions(ref Vector3 movement)
    {
        float direction = Mathf.Sign(movement.x);
        float rayLength = Mathf.Abs(movement.x) + skinWidth;

        Vector2 origin = direction < 0 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, rayLength, collisionMask);

        if (!hit)
        {
            // No hit at the bottom corner, check the top corner
            origin = direction < 0 ? raycastOrigins.topLeft : raycastOrigins.topRight;
            hit = Physics2D.Raycast(origin, Vector2.right * direction, rayLength, collisionMask);
        }

        if (hit)
        {
            movement.x = (hit.distance - skinWidth) * direction;
            hitEdge = true;
        }

        if (staysOnPlatform)
        {
            // Also check if we'll go over the edge because of this movement.
            Vector2 vectorLeft = raycastOrigins.bottomLeft;
            vectorLeft.x += movement.x;
            Vector2 vectorRight = raycastOrigins.bottomRight;
            vectorRight.x += movement.x;

            RaycastHit2D hitLeft = Physics2D.Raycast(vectorLeft, Vector2.down, skinWidth * 2, collisionMask);
            RaycastHit2D hitRight = Physics2D.Raycast(vectorRight, Vector2.down, skinWidth * 2, collisionMask);

            if ((hitLeft && !hitRight) || (!hitLeft && hitRight))
            {
                if (!reversing)
                {
                    reversing = true;
                    hitEdge = true;
                }
            }
            else
            {
                reversing = false;
            }
        }

        // return movement;
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

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

}
