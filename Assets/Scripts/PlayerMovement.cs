using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float maxSpeed = 5f;
    public float acceleration = 4f;
    public float stopDistance = 0.1f;
    public float accelerationThreshold = 0.75f;

    public LayerMask solidObjectsLayer;

    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private float currentMoveSpeed;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        // Calculate the direction vector from the player's current position to the target.
        Vector2 moveDirection = (targetPosition - rb.position).normalized;
        SetAnimationParameters(moveDirection);

        animator.SetBool("isMoving", isMoving);
    }

    private void FixedUpdate()
    {
        // Calculate the distance to the target position.
        float distanceToTarget = Vector2.Distance(rb.position, targetPosition);

        // Check if the player is close enough to the target to stop moving.
        if (distanceToTarget <= stopDistance)
        {
            Stop();
        }
        else
        {
            Vector2 moveDirection = (targetPosition - rb.position).normalized;

            // Check if the player is continuously moving in one direction to increase speed.
            if (Vector2.Dot(rb.velocity.normalized, moveDirection) >= accelerationThreshold)
            {
                currentMoveSpeed += acceleration * Time.fixedDeltaTime;
                currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, moveSpeed, maxSpeed);
            }
            else
            {
                currentMoveSpeed = moveSpeed;
            }

            Vector2 desiredVelocity = moveDirection * currentMoveSpeed;
            desiredVelocity = Vector2.ClampMagnitude(desiredVelocity, maxSpeed);

            // Calculate the velocity change required to reach the desired velocity.
            Vector2 velocityChange = desiredVelocity - rb.velocity;

            Rigidbody2D tempRB = rb;
            tempRB.AddForce(velocityChange * acceleration, ForceMode2D.Force);

            if (isWalkable(GetComponent<CapsuleCollider2D>(), tempRB.position))
            {
                // Apply force to achieve the desired velocity change.
                rb.AddForce(velocityChange * acceleration, ForceMode2D.Force);

                isMoving = true;
            }
            else
            {
                Stop();
            }

        }
    }

    // Set animation parameters based on the direction.
    private void SetAnimationParameters(Vector2 direction)
    {
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
    }

    private bool isWalkable(CapsuleCollider2D playerCol, Vector3 pos)
    {
        if (Physics2D.OverlapCapsule(pos, playerCol.size*2, playerCol.direction, 0, solidObjectsLayer) != null)
        {
            Debug.Log("false");
            return false;
        }
            Debug.Log("true");
        return true;
    }

    private void Stop()
    {
        rb.velocity = Vector2.zero; // Stop moving.
        SetAnimationParameters(Vector2.zero);

        isMoving = false;
    }
}
