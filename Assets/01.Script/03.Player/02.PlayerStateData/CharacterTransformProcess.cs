using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem.XInput;

public struct CharacterStat
{
    public int maxHp;
    public float moveSpeed;
    public float jumpPower;
}

public class CharacterTransformProcess
{
    CharacterController controller;
    Rigidbody rigid;
    GameObject foot;
    float limitAngle;
    int groundLayer;
    float groundCheckLength;

    public float jumpHeight = 2f;
    public const float gravity = -9.81f;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private float velocity;

    Vector3 moveDirection;
    Vector2 moveValue;

    public bool isRun;
    public bool isGround { get; private set; }
    public bool isStop { get; private set; }
    public bool isJumping { get; private set; }
    public bool isCrouch { get; private set; }

    Action[] motions;
    Action<Vector2> moveActionValue;
    Action<float>[] jumpActionValue;
    public CharacterTransformProcess(CharacterController characterController)
    {
        controller = characterController;
        motions = new Action[(int)AnimationController.MoveType.END];
        jumpActionValue = new Action<float>[(int)AnimationController.ValueType.END];
        isStop = true;
        isCrouch = false;
        isJumping = false;
    }
    public void SetMotions(AnimationController.MoveType type, Action action)
    {
        motions[(int)type] = action;
    }
    public void SetMoveType(bool state)
    {
        isRun = state;
    }
    public void SetMoveActionValue(AnimationController.ValueType idx,Action<float> action)
    {
        jumpActionValue[(int)idx] = action;
    }
    public void SetMoveActionValue(Action<Vector2> action)
    {
        moveActionValue = action;
    }
    public void Init(GameObject _foot,float _groundCheckLenth, int _groundLayer,float _limitAngle)
    {
        foot = _foot;
        groundCheckLength = _groundCheckLenth;
        groundLayer = _groundLayer;
        limitAngle = _limitAngle;
    }
    public void SetMoveValue(Vector2 _moveValue)
    {
        moveActionValue?.Invoke(_moveValue);
        if (_moveValue.x == 0 && _moveValue.y == 0)
        {
            motions[(int)AnimationController.MoveType.Stop]?.Invoke();
            moveValue = Vector2.zero;
            moveDirection = Vector3.zero;
            return;
        }
        if (isRun)
            motions[(int)AnimationController.MoveType.Run]?.Invoke();
        else
            motions[(int)AnimationController.MoveType.Walk]?.Invoke();
        moveValue = _moveValue * Time.deltaTime * 7;
    }

    public void Update()
    {
        Gravity();
        Direction();
        controller.Move(moveDirection);
    }
    public void FixedUpdate()
    {
        GroundCheck();
    }

    void Direction()
    {
        if (moveValue.x == 0 && moveValue.y == 0)
            return;
        (float x, float y) = (moveValue.x, moveValue.y);
        moveDirection = (x * controller.transform.right) + (y * controller.transform.forward);
        moveDirection.y += velocity;
    }

    void GroundCheck()
    {
        //하단에 레이 충돌 검사
        isGround = Physics.Raycast(
            foot.transform.position + Vector3.up * 0.1f, Vector3.down,
            out RaycastHit hitInfo,
            groundCheckLength,
            groundLayer);

        if (isGround)
        {
            float angle = Vector3.Angle(hitInfo.normal, Vector3.up);

            //Debug.DrawLine(foot.transform.position, foot.transform.position + Vector3.up, Color.red);
            //Debug.DrawLine(foot.transform.position, foot.transform.position + hitInfo.normal, Color.blue);
            if (isJumping) //떨어지고 있는 상태라면
            {
                if (angle < limitAngle) //미끄러지는 각도인지 확인
                {
                    motions[(int)AnimationController.MoveType.JumpFinish]?.Invoke(); //미끄러지는 각도가 아니라면 착지
                    isJumping = false;
                    Debug.Log("Jump slide Angle");
                }
                else //점프 모션 발동
                {
                    motions[(int)AnimationController.MoveType.JumpMove]?.Invoke();
                    Debug.Log("Maybe Jump");
                }
            }
            else if (angle > limitAngle) //미끄러지는 각도라면
            {
                motions[(int)AnimationController.MoveType.JumpSlide]?.Invoke();
                isJumping = true;
                Debug.Log("Slide Angle");
            }
        }
        else //땅이 아니라면
        {
            if (isJumping == false) //점프상태가 아니고 
            {
                if (isCrouch) //앉은 상태라면
                    Crouch(); //일어서게 하고
                motions[(int)AnimationController.MoveType.Jump]?.Invoke();
                isJumping = true;
                Debug.Log("Downing");
            }
        }
    }
    public void Crouch()
    {
        isCrouch = !isCrouch;
        if (isCrouch)
            motions[(int)AnimationController.MoveType.Crouch]?.Invoke();
        else
            motions[(int)AnimationController.MoveType.Stand]?.Invoke();
    }

    public void Jump()
    {
        if (isGround && isCrouch == false)
            velocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void Gravity()
    {
        if (isJumping)
        {
            velocity += gravity * Time.deltaTime;
            jumpActionValue[(int)AnimationController.ValueType.Velocity]?.Invoke(velocity);
        }
        else
        {
            if (velocity < 0)
                velocity = -2f; // Grounded, so reset velocity
        }
    }

}