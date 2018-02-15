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

    private bool right = true;

    private Vector2 lookDirection = new Vector2();

    private bool jumping = false;

    private PhotonView photonViewer;

    private PlayerBase pb;
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        body.freezeRotation = true;
        body.useGravity = false;
        photonViewer = GetComponent<PhotonView>();
        pb = GetComponent<PlayerBase>();
        animator = GetComponentInChildren<Animator>();
        playerBody = animator.transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {


        if (!photonViewer.isMine)
            return;

        TrackGrounded();

        if (Mathf.Abs(body.velocity.z) > 0.1f && !animator.GetBool("IsRunning"))
            photonViewer.RPC("DoRunning", PhotonTargets.All);
        else if (Mathf.Abs(body.velocity.z) < 0.1f && animator.GetBool("IsRunning"))
            photonViewer.RPC("StopRunning", PhotonTargets.All);

        if(animator.GetBool("IsGrounded") != grounded)
            photonViewer.RPC("RPC_IsGrounded",PhotonTargets.All,grounded);


        pb.CheckWithinArena();
        UpdateFaceDirection();

        

        

        if ((grounded || jumpsLeft > 0) && Input.GetButtonDown("Jump") && !jumping)
        {
            jumping = true;
            animator.SetTrigger("IsJumping");
            photonViewer.RPC("DoJump", PhotonTargets.Others);
        }

        if (Input.GetButtonDown("RegularAttack") && Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.S) && Input.GetButtonDown("RegularAttack"))
        {
            pb.RegularAttack(2);
            DoPunch(3);
        }
        else if (Input.GetButtonDown("RegularAttack") && Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.W) && Input.GetButtonDown("RegularAttack"))
        {
            pb.RegularAttack(1);
            DoPunch(2);
        }
        else if (Input.GetButtonDown("RegularAttack"))
        {
            int dir = lookDirection.y != 0 ? (int)lookDirection.y : (int)lookDirection.x;

            pb.RegularAttack(dir);
            DoPunch(1);
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
        playerBody.transform.eulerAngles = new Vector3(0, dir ? 0 : 180, 0);
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

    bool TrackGrounded()
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
        return grounded;

    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    [PunRPC]
    public void DoJump()
    {
        animator.SetTrigger("IsJumping");
    }

    [PunRPC]
    void DoRunning()
    {
        animator.SetBool("IsRunning", true);
    }

    [PunRPC]
    void StopRunning()
    {
        animator.SetBool("IsRunning", false);
    }

    [PunRPC]
    void RPC_IsGrounded(bool g)
    {
        animator.SetBool("IsGrounded", g);
    }

    public void DoPunch(int a)
    {
        Debug.Log("punch anim " + a);
        animator.SetInteger("AttackState", a);
    }

    public GameObject PlayerBody
    {
        get { return playerBody; }
        set
        {
            playerBody.SetActive(false);
            playerBody = value;
            animator = playerBody.GetComponent<Animator>();
        }
    }
}
