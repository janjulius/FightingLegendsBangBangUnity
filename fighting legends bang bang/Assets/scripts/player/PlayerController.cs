using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody body;
    [SerializeField] private GameObject playerBody;
    private float speed = 13.0f;
    private float gravity = 25;
    private float inAirControl = 0.8f;
    private float maxVelocityChange = 10.0f;
    private bool canJump = true;
    private float jumpHeight = 4.0f;
    private float jumpsLeft = 1;
    private float maxJumps = 1;
    private bool grounded = false;
    private CapsuleCollider capsule;
    private Vector3 groundVelocity;

    private bool right = false;

    private Vector2 lookDirection = new Vector2();

    private bool jumping = false;

    private PhotonView photonViewer;

    private PlayerBase pb;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        body.freezeRotation = true;
        body.useGravity = false;
        photonViewer = GetComponent<PhotonView>();
        pb = GetComponent<PlayerBase>();
    }

    // Update is called once per frame
    void Update()
    {


        if (!photonViewer.isMine)
            return;

        UpdateFaceDirection();

        TrackGrounded();

        if ((grounded || jumpsLeft > 0) && Input.GetButtonDown("Jump") && !jumping)
        {
            jumping = true;
        }

        Debug.Log(Input.GetButtonDown("RegularAttack") + " " + Input.GetKey(KeyCode.W));

        if (Input.GetButton("RegularAttack") && Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.S) && Input.GetButton("RegularAttack"))
        {
            pb.RegularAttack(2);
        }
        else if (Input.GetButton("RegularAttack") && Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.W) && Input.GetButton("RegularAttack"))
        {
            pb.RegularAttack(1);
        }
        else if (Input.GetButton("RegularAttack"))
        {
            int dir = lookDirection.y != 0 ? (int)lookDirection.y : (int)lookDirection.x;

            pb.RegularAttack(dir);
        }

        if (Input.GetButton("SpecialAttack"))
        {
            pb.SpecialAttack();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel(1);
        }

        if (grounded)
            jumpsLeft = maxJumps;
    }

    private void UpdateFaceDirection()
    {
        if (Input.GetAxis("Horizontal") > 0.2 && !right)
        {
            right = true;
            photonViewer.RPC("RPC_UpdateDirection", PhotonTargets.Others, true);
            playerBody.transform.eulerAngles = new Vector3(0, 0, 0);
            lookDirection.x = 0;

        }
        else if (Input.GetAxis("Horizontal") < -0.2 && right)
        {
            right = false;
            photonViewer.RPC("RPC_UpdateDirection", PhotonTargets.Others, false);
            playerBody.transform.eulerAngles = new Vector3(0, 180, 0);
            lookDirection.x = -1;

        }
        if (Input.GetAxis("Vertical") > 0.2)
        {
            lookDirection.y = 1;

        }
        else if (Input.GetAxis("Vertical") < -0.2)
        {
            lookDirection.y = 2;
        }
        else
        {
            lookDirection.y = 0;
        }
    }

    [PunRPC]
    public void RPC_UpdateDirection(bool dir)
    {
        playerBody.transform.eulerAngles = new Vector3(0, dir ? 180 : 0, 0);
    }

    void FixedUpdate()
    {
        if (!photonViewer.isMine)
            return;



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
        if (jumping)
        {
            if (!grounded)
                jumpsLeft--;
            body.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
            jumping = false;
        }

        grounded = false;

        // We apply gravity manually for more tuning control
        body.AddForce(new Vector3(0, -gravity * body.mass, 0));

    }

    void TrackGrounded()
    {
        Ray rayLeft = new Ray(transform.position + new Vector3(0, -0.7f, -0.4f), Vector3.down);
        Ray rayRight = new Ray(transform.position + new Vector3(0, -0.7f, 0.4f), Vector3.down);
        RaycastHit hitLeft;
        RaycastHit hitRight;

        if (Physics.Raycast(rayLeft, out hitLeft, 0.4f))
        {
            if (hitLeft.transform.gameObject.layer == 9)
                grounded = true;

        }
        else if (Physics.Raycast(rayRight, out hitRight, 0.4f))
        {
            if (hitRight.transform.gameObject.layer == 9)
                grounded = true;
        }
        Debug.DrawLine(rayLeft.origin, rayLeft.origin + new Vector3(0, -0.4f, 0), Color.red);
        Debug.DrawLine(rayRight.origin, rayRight.origin + new Vector3(0, -0.4f, 0), Color.red);

    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    public GameObject PlayerBody
    {
        get { return playerBody; }
        set
        {
            playerBody.SetActive(false);
            playerBody = value;
        }
    }
}
