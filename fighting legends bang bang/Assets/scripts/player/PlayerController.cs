using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody body;
    private float speed = 10.0f;
    private float gravity = 15;
    private float inAirControl = 0.8f;
    private float maxVelocityChange = 10.0f;
    private bool canJump = true;
    private float jumpHeight = 2.0f;
    private bool grounded = false;
    private CapsuleCollider capsule;
    private Vector3 groundVelocity;

    private PhotonView photonViewer;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        body.freezeRotation = true;
        body.useGravity = false;
        photonViewer = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (!photonViewer.isMine)
            return;


            if (grounded)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(0, 0, Input.GetAxis("Horizontal"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= speed;

            // Apply a force that attempts to reach our target velocity
            var velocity = body.velocity;
            var velocityChange = (targetVelocity - velocity) + groundVelocity;
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
            Vector3 targetVelocity = new Vector3(0, 0, Input.GetAxis("Horizontal"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= speed * inAirControl;

            // Apply a force that attempts to reach our target velocity
            var velocity = body.velocity;
            var velocityChange = (targetVelocity - velocity) + groundVelocity;
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            body.AddForce(velocityChange, ForceMode.VelocityChange);
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
        if (!photonViewer.isMine)
            return;
        TrackGrounded(col);
    }

    void OnCollisionEnter(Collision col)
    {
        if (!photonViewer.isMine)
            return;
        TrackGrounded(col);
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
}
