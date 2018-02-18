using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody body;
    private float speed = 13f;
    private float maxspeed = 13f;
    private float gravity = 35f;
    private float VerticalVelocityMin = 25f;
    private float gravityAcceleration = 25f;

    private float inAirControl = 0.8f;
    private bool canJump = true;
    private bool sliding = false;
    private float jumpHeight = 15f;
    private float jumpsLeft = 1;
    private float maxJumps = 1;
    public CapsuleCollider capsule;
    private Vector3 groundVelocity;

    private bool right = false;

    private Vector2 lookDirection = new Vector2(-1, 0);

    private bool jumping = false;
    private bool _jumping = false;

    

    private PlayerBase pb;

    public GameObject[] touchingSides = new GameObject[4];
    public Vector2 KnockBack = new Vector2();

    public enum Direction { Top = 0, Bottom = 1, Left = 2, Right = 3 }

    public float VerticalVelocity = 0;

    // Use this for initialization
    void Start()
    {

        body = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        body.freezeRotation = true;
        body.useGravity = false;
        pb = GetComponent<PlayerBase>();
        
    }


    // Update is called once per frame
    public void PlayerUpdate()
    {

        LerpingKnockBack();
        sliding = CheckSide(Direction.Left) || CheckSide(Direction.Right);

        if (CheckSide(Direction.Bottom) && !CheckSide(Direction.Bottom))
            touchingSides[(int)Direction.Bottom] = null;

        if (Mathf.Abs(pb.Keys.Horizontal()) > 0.1f && !pb.animator.GetBool("IsRunning"))
            pb.photonViewer.RPC("RPC_DoRunning", PhotonTargets.All);
        else if (Mathf.Abs(pb.Keys.Horizontal()) < 0.1f && pb.animator.GetBool("IsRunning"))
            pb.photonViewer.RPC("RPC_StopRunning", PhotonTargets.All);

        if (pb.animator.GetBool("IsGrounded") != CheckSide(Direction.Bottom))
            pb.photonViewer.RPC("RPC_IsGrounded", PhotonTargets.All, CheckSide(Direction.Bottom));


        pb.CheckWithinArena();
        UpdateFaceDirection();

        if (CheckSide(Direction.Right) && KnockBack.x > 0)
            KnockBack.x = -KnockBack.x;
        if (CheckSide(Direction.Left) && KnockBack.x < 0)
            KnockBack.x = -KnockBack.x;
        if (CheckSide(Direction.Top) && KnockBack.y > 0)
            KnockBack.y = -KnockBack.y;
        if (CheckSide(Direction.Bottom) && KnockBack.y < 0)
            KnockBack.y = -KnockBack.y;


        VerticalVelocityMin = sliding ? gravity / 10 : gravity;


        if (VerticalVelocity > -VerticalVelocityMin)
            VerticalVelocity -= gravityAcceleration * Time.deltaTime;
        else
            VerticalVelocity = -VerticalVelocityMin;


        if ((CheckSide(Direction.Bottom) || jumpsLeft > 0) && pb.Keys.JumpButtonDown() && !jumping)
        {
            jumping = true;
            pb.animator.SetTrigger("IsJumping");
            pb.photonViewer.RPC("RPC_DoJump", PhotonTargets.Others);
        }

        if ((CheckSide(Direction.Bottom) && !jumping && !_jumping) || (VerticalVelocity > 0 && CheckSide(Direction.Top)) || KnockBack.y != 0)
            VerticalVelocity = 0;


        if (pb.Keys.AttackButtonDown())
        {

            Vector2 dir = new Vector2(lookDirection.y != 0 ? 0 : lookDirection.x, lookDirection.y);

            pb.RegularAttack(dir);
        }

        if (pb.Keys.SpecialAttackButtonDown())
        {
            pb.SpecialAttack();
        }

        if (pb.Keys.BlockButton())
        {
            pb.Block();
        }
    }

    private void LerpingKnockBack()
    {
        KnockBack = Vector2.Lerp(KnockBack, Vector2.zero, 2 * Time.deltaTime);

        if (KnockBack.y < 4 && KnockBack.y > -4)
            KnockBack.y = 0;

        if (KnockBack.x < 0 && pb.Keys.Horizontal() > 0)
        {
            if (KnockBack.x < 4 && KnockBack.x > -4)
                KnockBack.x = 0;
        }
        else if (KnockBack.x < 0 && pb.Keys.Horizontal() < 0)
        {
            if (KnockBack.x < speed && KnockBack.x > -speed)
                KnockBack.x = 0;
        }

        if (KnockBack.x > 0 && pb.Keys.Horizontal() < 0)
        {
            if (KnockBack.x < 4 && KnockBack.x > -4)
                KnockBack.x = 0;
        }
        else if (KnockBack.x > 0 && pb.Keys.Horizontal() > 0)
        {
            if (KnockBack.x < speed && KnockBack.x > -speed)
                KnockBack.x = 0;
        }

        if (KnockBack.x < 0 && pb.Keys.Horizontal() > 0)
            KnockBack.x += (speed) * Time.deltaTime;
        if (KnockBack.x > 0 && pb.Keys.Horizontal() < 0)
            KnockBack.x -= (speed) * Time.deltaTime;
    }

    private void UpdateFaceDirection()
    {
        if (pb.Keys.Horizontal() > 0.2 && !right)
        {
            right = true;
            pb.photonViewer.RPC("RPC_UpdateDirection", PhotonTargets.Others, true);
            pb.playerBody.transform.eulerAngles = new Vector3(0, 0, 0);
            lookDirection.x = 1;

        }
        else if (pb.Keys.Horizontal() < -0.2 && right)
        {
            right = false;
            pb.photonViewer.RPC("RPC_UpdateDirection", PhotonTargets.Others, false);
            pb.playerBody.transform.eulerAngles = new Vector3(0, 180, 0);
            lookDirection.x = -1;

        }
        if (pb.Keys.Vertical() > 0.2)
        {
            lookDirection.y = 1;

        }
        else if (pb.Keys.Vertical() < -0.2)
        {
            lookDirection.y = -1;
        }
        else
        {
            lookDirection.y = 0;
        }
    }

    void FixedUpdate()
    {
        if (!pb.photonViewer.isMine)
            return;


        TrackGrounded();

        // Calculate how fast we should be moving
        float MoveSpeed = pb.Keys.Horizontal() * speed;

        _jumping = false;
        // Jump
        if (jumping)
        {
            if (!CheckSide(Direction.Bottom) || (sliding && !CheckSide(Direction.Bottom)))
                jumpsLeft--;
            VerticalVelocity = jumpHeight;

            if (CheckSide(Direction.Left))
                KnockBack.x += 10;
            else if (CheckSide(Direction.Right))
                KnockBack.x -= 10;
            touchingSides[(int)Direction.Bottom] = null;
            jumping = false;
            _jumping = true;
        }

        if (CheckSide(Direction.Bottom) || (sliding && !_jumping))
            jumpsLeft = maxJumps;

        float velocityY = 0;
        float velocityZ = 0;

        velocityY = KnockBack.y != 0 ? KnockBack.y : VerticalVelocity;
        velocityZ = KnockBack.x != 0 ? KnockBack.x : MoveSpeed;


        // We apply gravity manually for more tuning control
        body.velocity = new Vector3(0, velocityY, velocityZ);

    }

    public bool CheckSide(Direction dir)
    {
        return touchingSides[(int)dir];
    }

    void TrackGrounded()
    {

        float capsH = capsule.height / 2;
        float radius = capsule.radius;

        touchingSides[0] = castGround(new Vector3(0, capsH - 0.3f, -radius + 0.3f), Vector3.up, 0.4f, false);
        touchingSides[1] = castGround(new Vector3(0, -capsH + 0.3f, -radius + 0.3f), Vector3.down, 0.43f, false);
        touchingSides[2] = castGround(new Vector3(0, capsH * 0.5f, -radius + 0.3f), -Vector3.forward, 0.35f, true);
        touchingSides[3] = castGround(new Vector3(0, capsH * 0.5f, radius - 0.3f), Vector3.forward, 0.35f, true);


    }

    private GameObject castGround(Vector3 pos, Vector3 dir, float lenght, bool s)
    {
        GameObject obj = null;

        var pos1 = new Vector3(pos.x, pos.y, pos.z);
        var pos2 = new Vector3(pos.x, s ? -pos.y : pos.y, !s ? -pos.z : pos.z);
        var pos3 = new Vector3(pos.x, s ? 0 : pos.y, !s ? 0 : pos.z);


        Ray rayLeft = new Ray(transform.position + pos1, dir);
        Ray rayRight = new Ray(transform.position + pos2, dir);
        Ray rayMiddel = new Ray(transform.position + pos3, dir);
        RaycastHit hitLeft;
        RaycastHit hitRight;
        RaycastHit hitMiddel;

        if (Physics.Raycast(rayMiddel, out hitMiddel, lenght))
        {
            if (hitMiddel.transform.gameObject.layer == 9)
                obj = hitMiddel.transform.gameObject;
        }
        else if (Physics.Raycast(rayLeft, out hitLeft, lenght))
        {
            if (hitLeft.transform.gameObject.layer == 9)
                obj = hitLeft.transform.gameObject;
        }
        else if (Physics.Raycast(rayRight, out hitRight, lenght))
        {
            if (hitRight.transform.gameObject.layer == 9)
                obj = hitRight.transform.gameObject;
        }


        Debug.DrawLine(rayLeft.origin, rayLeft.origin + dir * lenght, Color.red);
        Debug.DrawLine(rayRight.origin, rayRight.origin + dir * lenght, Color.blue);
        Debug.DrawLine(rayMiddel.origin, rayMiddel.origin + dir * lenght, Color.green);

        return obj;
    }

}
