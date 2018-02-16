using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody body;
    [SerializeField] private GameObject playerBody;
    private float speed = 13.0f;
    private float gravity = 35f;
    private float VerticalVelocityMin = 25f;
    private float gravityAcceleration = 25f;

    private float inAirControl = 0.8f;
    private float maxVelocityChange = 10.0f;
    private bool canJump = true;
    private float jumpHeight = 15f;
    private float jumpsLeft = 1;
    private float maxJumps = 1;
    private CapsuleCollider capsule;
    private Vector3 groundVelocity;

    private bool right = true;

    private Vector2 lookDirection = new Vector2();

    private bool jumping = false;
    private bool _jumping = false;

    private PhotonView photonViewer;

    private PlayerBase pb;
    private Animator animator;

    public GameObject[] touchingSides = new GameObject[4];

    public enum HitDirection { None = -1, Top = 0, Bottom = 1, Left = 2, Right = 3 }

    public float VerticalVelocity = 0;

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

        if (CheckSide(HitDirection.Bottom) && !CheckSide(HitDirection.Bottom))
            touchingSides[(int)HitDirection.Bottom] = null;

        if (Mathf.Abs(body.velocity.z) > 0.1f && !animator.GetBool("IsRunning"))
            photonViewer.RPC("DoRunning", PhotonTargets.All);
        else if (Mathf.Abs(body.velocity.z) < 0.1f && animator.GetBool("IsRunning"))
            photonViewer.RPC("StopRunning", PhotonTargets.All);

        if (animator.GetBool("IsGrounded") != CheckSide(HitDirection.Bottom))
            photonViewer.RPC("RPC_IsGrounded", PhotonTargets.All, CheckSide(HitDirection.Bottom));


        pb.CheckWithinArena();
        UpdateFaceDirection();


        VerticalVelocityMin = CheckSide(HitDirection.Left) || CheckSide(HitDirection.Right) ? gravity / 10 : gravity;


        if (VerticalVelocity > -VerticalVelocityMin)
            VerticalVelocity -= gravityAcceleration * Time.deltaTime;
        else
            VerticalVelocity = -VerticalVelocityMin;


        if ((CheckSide(HitDirection.Bottom) || jumpsLeft > 0) && Input.GetButtonDown("Jump") && !jumping)
        {
            jumping = true;
            animator.SetTrigger("IsJumping");
            photonViewer.RPC("DoJump", PhotonTargets.Others);
        }

        if (CheckSide(HitDirection.Bottom) && !jumping && !_jumping)
            VerticalVelocity = 0;

        _jumping = false;


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

        if (CheckSide(HitDirection.Bottom) || (CheckSide(HitDirection.Left) || CheckSide(HitDirection.Right)))
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


        TrackGrounded();

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
            if (!CheckSide(HitDirection.Bottom))
                jumpsLeft--;
            VerticalVelocity = jumpHeight;

            if (CheckSide(HitDirection.Left))
                velocity.z += 40;
            else if (CheckSide(HitDirection.Right))
                velocity.z -= 40;
            touchingSides[(int)HitDirection.Bottom] = null;
            jumping = false;
            _jumping = true;
        }



        // We apply gravity manually for more tuning control
        body.velocity = new Vector3(velocity.x, VerticalVelocity, velocity.z);

    }

    public bool CheckSide(HitDirection dir)
    {
        return touchingSides[(int)dir];
    }


    void TrackGrounded()
    {

        touchingSides[0] = castGround(new Vector3(0, 0.7f, -0.3f), Vector3.up, 0.4f, false);
        touchingSides[1] = castGround(new Vector3(0, -0.6f, -0.3f), Vector3.down, 0.43f, false);
        touchingSides[2] = castGround(new Vector3(0, 0.5f, -0.4f), -Vector3.forward, 0.4f, true);
        touchingSides[3] = castGround(new Vector3(0, 0.5f, 0.4f), Vector3.forward, 0.4f, true);


    }

    private GameObject castGround(Vector3 pos, Vector3 dir, float lenght, bool s)
    {
        GameObject obj = null;

        var pos1 = new Vector3(pos.x, pos.y, pos.z);
        var pos2 = new Vector3(pos.x, s ? -pos.y : pos.y, !s ? -pos.z : pos.z);


        Ray rayLeft = new Ray(transform.position + pos1, dir);
        Ray rayRight = new Ray(transform.position + pos2, dir);
        RaycastHit hitLeft;
        RaycastHit hitRight;

        if (Physics.Raycast(rayLeft, out hitLeft, lenght))
            if (hitLeft.transform.gameObject.layer == 9)
                obj = hitLeft.transform.gameObject;
            else if (Physics.Raycast(rayRight, out hitRight, lenght))
                if (hitRight.transform.gameObject.layer == 9)
                    obj = hitRight.transform.gameObject;


        Debug.DrawLine(rayLeft.origin, rayLeft.origin + dir * lenght, Color.red);
        Debug.DrawLine(rayRight.origin, rayRight.origin + dir * lenght, Color.red);

        return obj;
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
