using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 wishDir;

    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;

    private int moveX;
    private int moveZ;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float moveForce;
    [SerializeField] private int jumpForce;

    [SerializeField] private bool isGrounded;
    [SerializeField] private Transform orientation;
    [SerializeField] private LayerMask environmentMask;
    [SerializeField] private float gravityMagnitude;

    private RaycastHit groundHit;

    private void Start()
    {
        Physics.gravity = -gravityMagnitude * Vector3.up;
    }

    private void Update()
    {
        MoveInputVector();
        MovementCalculations();
        Jump();
    }

    private void FixedUpdate()
    {
        rb.AddForce(moveForce * wishDir, ForceMode.Acceleration);
    }

    private void MoveInputVector()
    {
        if (Input.GetKey(KeyCode.W))
        {
            moveZ = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveZ = -1;
        }
        else
        {
            moveZ = 0;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveX = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveX = -1;
        }
        else
        {
            moveX = 0;
        }
    }

    private void MovementCalculations()
    {
        isGrounded = Physics.CheckSphere(orientation.position + new Vector3(0.0f, 0.1f, 0.0f), /*0.15f*/0.2f, environmentMask);
        if (isGrounded)
        {
            Physics.Raycast(orientation.position + new Vector3(0.0f, 0.1f, 0.0f), Vector3.down, out groundHit, /*0.2f*/1f, environmentMask);
            Physics.gravity = -gravityMagnitude * groundHit.normal;
            rb.drag = groundDrag;
        }
        else
        {
            Physics.gravity = -gravityMagnitude * Vector3.up;
            rb.drag = airDrag;
        }

        wishDir = orientation.forward * moveZ + orientation.right * moveX;
        if (isGrounded)
        {
            wishDir = Vector3.ProjectOnPlane(wishDir, groundHit.normal);
        }
        wishDir.Normalize();

        Vector3 currentVel = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
        if (currentVel.magnitude > maxMoveSpeed)
        {
            Vector3 targetVel = maxMoveSpeed * currentVel.normalized;
            rb.velocity = new Vector3(targetVel.x, rb.velocity.y, targetVel.z);
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        }
    }
}
