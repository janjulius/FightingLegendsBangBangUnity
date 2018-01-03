using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody body;
    public float speed = 10.0f;
    public float gravity = 10.0f;
    public float inAirControl = 0.1f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    private bool grounded = false;
    private CapsuleCollider capsule;
    private Vector3 groundVelocity;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        body.freezeRotation = true;
        body.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (grounded)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(0, 0, Input.GetAxis("Horizontal"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= speed;

            // Apply a force that attempts to reach our target velocity
            var velocity = body.velocity;
            var velocityChange = (targetVelocity - velocity) + groundVelocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            body.AddForce(velocityChange, ForceMode.VelocityChange);


            // Jump
            if (grounded && Input.GetButton("Jump"))
            {
                body.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
            }

            grounded = false;
        }
        else
        {
            // Add in air
            var targetVelocity = new Vector3(0, 0, Input.GetAxis("Horizontal"));
            targetVelocity = transform.TransformDirection(targetVelocity) * inAirControl;

            body.AddForce(targetVelocity, ForceMode.VelocityChange);
        }

        // We apply gravity manually for more tuning control
        body.AddForce(new Vector3(0, -gravity * body.mass, 0));

    }

    void TrackGrounded(Collision col)
    {
        var minimumHeight = capsule.bounds.min.y + capsule.radius;
        foreach (var c in col.contacts)
        {
            if (c.point.y < minimumHeight)
            {
                if (col.rigidbody)
                    groundVelocity = col.rigidbody.velocity;
                else
                    groundVelocity = Vector3.zero;
                grounded = true;
            }
        }
    }


    void OnCollisionStay(Collision col)
    {
        TrackGrounded(col);
    }

    void OnCollisionEnter(Collision col)
    {
        TrackGrounded(col);
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
}
