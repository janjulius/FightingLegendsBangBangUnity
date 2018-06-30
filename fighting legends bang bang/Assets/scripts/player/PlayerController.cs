using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody body;
    private float VerticalVelocityMin;

    private bool canJump = true;
    private bool sliding = false;
    private float jumpsLeft = 1;
    public CapsuleCollider capsule;
    private Vector3 groundVelocity;

    internal bool right = false;

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
        if (VerticalVelocity > 0 || pb.Keys.Vertical() < -0.8)
            gameObject.layer = 11;
        else if (gameObject.layer == 11)
            gameObject.layer = 8;

        TrackGrounded();

        LerpingKnockBack();
        sliding = CheckSide(Direction.Left) || CheckSide(Direction.Right);


        pb.animator.SetBool("IsRunning", Mathf.Abs(pb.Keys.Horizontal()) > 0.1f && !pb.CanNotMove);

        pb.animator.SetBool("IsGrounded", CheckSide(Direction.Bottom));

        pb.CheckWithinArena();
        UpdateFaceDirection();


        VerticalVelocityMin = sliding ? pb.currentCharacter.maxGravity / 13 : pb.currentCharacter.maxGravity;


        if (VerticalVelocity > -VerticalVelocityMin)
            VerticalVelocity -= pb.currentCharacter.gravitySpeed * Time.deltaTime;
        else
            VerticalVelocity = -VerticalVelocityMin;


        controlUpdate();
    }

    private void controlUpdate()
    {
        if (pb.CanNotMove)
            return;


        if ((CheckSide(Direction.Bottom) || jumpsLeft > 0) && pb.Keys.JumpButtonDown() && !jumping && canJump)
        {
            jumping = true;
            pb.animator.SetTrigger("IsJumping");
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

        if (pb.Keys.BlockButtonDown())
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
            if (KnockBack.x < pb.currentCharacter.speed && KnockBack.x > -pb.currentCharacter.speed)
                KnockBack.x = 0;
        }

        if (KnockBack.x > 0 && pb.Keys.Horizontal() < 0)
        {
            if (KnockBack.x < 4 && KnockBack.x > -4)
                KnockBack.x = 0;
        }
        else if (KnockBack.x > 0 && pb.Keys.Horizontal() > 0)
        {
            if (KnockBack.x < pb.currentCharacter.speed && KnockBack.x > -pb.currentCharacter.speed)
                KnockBack.x = 0;
        }

        if (KnockBack.x < 0 && pb.Keys.Horizontal() > 0)
            KnockBack.x += (pb.currentCharacter.speed) * Time.deltaTime;
        if (KnockBack.x > 0 && pb.Keys.Horizontal() < 0)
            KnockBack.x -= (pb.currentCharacter.speed) * Time.deltaTime;

        if (CheckSide(Direction.Right) && KnockBack.x > 0)
            KnockBack.x = -KnockBack.x;
        if (CheckSide(Direction.Left) && KnockBack.x < 0)
            KnockBack.x = -KnockBack.x;
        if (CheckSide(Direction.Top) && KnockBack.y > 0)
            KnockBack.y = -KnockBack.y;
        if (CheckSide(Direction.Bottom) && KnockBack.y < 0)
            KnockBack.y = -KnockBack.y;
    }

    private void UpdateFaceDirection()
    {
        if (pb.CanNotMove)
            return;

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

        // Calculate how fast we should be moving
        float MoveSpeed = pb.Keys.Horizontal() * pb.currentCharacter.speed;

        _jumping = false;
        // Jump
        if (jumping)
        {
            if (!CheckSide(Direction.Bottom) || (sliding && !CheckSide(Direction.Bottom)))
                jumpsLeft--;
            VerticalVelocity = pb.currentCharacter.jumpForce;

            if (CheckSide(Direction.Left))
                KnockBack.x += 10;
            else if (CheckSide(Direction.Right))
                KnockBack.x -= 10;
            touchingSides[(int)Direction.Bottom] = null;
            jumping = false;
            _jumping = true;
        }

        if (CheckSide(Direction.Bottom) || (sliding && !_jumping))
            jumpsLeft = pb.currentCharacter.maxJumps;

        float velocityY = 0;
        float velocityZ = 0;

        velocityY = KnockBack.y != 0 ? KnockBack.y : VerticalVelocity;
        velocityZ = KnockBack.x != 0 ? KnockBack.x : MoveSpeed;


        // We apply gravity manually for more tuning control
        body.velocity = new Vector3(0, velocityY, velocityZ);

        if (pb.CanNotMove)
            body.velocity = Vector3.zero;
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
            if (hitMiddel.transform.gameObject.layer == 9 || (hitMiddel.transform.gameObject.layer == 10 && gameObject.layer != 11))
                obj = hitMiddel.transform.gameObject;
        }
        else if (Physics.Raycast(rayLeft, out hitLeft, lenght))
        {
            if (hitLeft.transform.gameObject.layer == 9 || (hitLeft.transform.gameObject.layer == 10 && gameObject.layer != 11))
                obj = hitLeft.transform.gameObject;
        }
        else if (Physics.Raycast(rayRight, out hitRight, lenght))
        {
            if (hitRight.transform.gameObject.layer == 9 || (hitRight.transform.gameObject.layer == 10 && gameObject.layer != 11))
                obj = hitRight.transform.gameObject;
        }


        Debug.DrawLine(rayLeft.origin, rayLeft.origin + dir * lenght, Color.red);
        Debug.DrawLine(rayRight.origin, rayRight.origin + dir * lenght, Color.blue);
        Debug.DrawLine(rayMiddel.origin, rayMiddel.origin + dir * lenght, Color.green);

        return obj;
    }

    public void SetCanJump(bool b)
    {
        canJump = b;
    }

    public bool GetCanJump()
    {
        return canJump;
    }

}
