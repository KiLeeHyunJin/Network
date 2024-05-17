using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MoveValue { get; private set; }
    public float MoveY { get; private set; }
    Action[] actions;
    Action<Vector2> moveAction;
    Action<Vector2> rotateAction;
    Action<bool> moveTypeAction;
    Controller owner;
    InputActionAsset inputs;
    public Controller Owner { set { owner = value; } }
    public Define.InputWeaponType CurrentWeapon { get; private set; }
    public Define.FireType MainFireType { get; private set; }
    public Define.FireType Fire { get; private set; }
    public Define.FireType ChangeFireType
    {
        set
        {
            Fire = value;
            if (CurrentWeapon == Define.InputWeaponType.MainWeapon)
                MainFireType = Fire;
        }
    }
    public bool InputActive
    {
        set
        {
            if (value)
                inputs.Enable();
            else
                inputs.Disable();
        }
    }
    public void SetKey(Action method, Define.Key key) => actions[(int)key] = method;
    public void SetMoveKey(Action<Vector2> moveMethod) => moveAction = moveMethod;
    public void SetRot(Action<Vector2> rotMethod) => rotateAction = rotMethod;
    public void SetMoveType(Action<bool> moveTypeMethod) => moveTypeAction = moveTypeMethod;

    public void Init()
    {
        inputs = GetComponent<PlayerInput>().actions;
        ChangeFireType = Define.FireType.One;
        actions = new Action[(int)Define.Key.END];
    }
    void OnLook(InputValue inputValue)
    {
        rotateAction?.Invoke(inputValue.Get<Vector2>());
    }
    void OnMove(InputValue inputValue)
    {
        Vector2 MoveValue = inputValue.Get<Vector2>().normalized;
        moveAction.Invoke(MoveValue);
        MoveY = MoveValue.y;
    }
    void OnJump(InputValue inputValue)
    {
        actions[(int)Define.Key.Space].Invoke();
    }

    void OnFirstWeapon(InputValue inputValue)
    {
        actions[(int)Define.Key.F1].Invoke();
        CurrentWeapon = Define.InputWeaponType.MainWeapon;
        ChangeFireType = MainFireType;
    }
    void OnSecondWeapon(InputValue inputValue)
    {
        actions[(int)Define.Key.F2].Invoke();
        CurrentWeapon = Define.InputWeaponType.SubWeapon;
    }
    void OnOtherWeapon(InputValue inputValue)
    {
        actions[(int)Define.Key.F3].Invoke();
        CurrentWeapon = Define.InputWeaponType.Default;
    }
    void OnInteraction(InputValue inputValue)
    {
        actions[(int)Define.Key.F]?.Invoke();
    }
    void OnC(InputValue inputValue)
    {
        actions[(int)Define.Key.C].Invoke();
    }
    void OnX(InputValue inputValue)
    {
        actions[(int)Define.Key.X]?.Invoke();
    }
    void OnZ(InputValue inputValue)
    {
        actions[(int)Define.Key.Z]?.Invoke();
    }
    void OnV(InputValue inputValue)
    {
        actions[(int)Define.Key.V]?.Invoke();
    }
    void OnR(InputValue inputValue)
    {
        actions[(int)Define.Key.R].Invoke();
    }
    void OnShift(InputValue inputValue)
    {
        bool value = inputValue.isPressed ? true : false;
        moveTypeAction.Invoke(value);
    }

    void OnAlt(InputValue inputValue)
    {
        actions[(int)Define.Key.Alt]?.Invoke();
    }
    void OnFireOne(InputValue inputValue)
    {
        if (Define.FireType.One == Fire)
            actions[(int)Define.Key.Press]?.Invoke();
    }
    void OnFirePress(InputValue inputValue)
    {
        if (Define.FireType.One == Fire)
            return;
        if (inputValue.isPressed)
            owner.StartCoroutined(PressRoutine(), ref pressCo);
        else
            owner.StopCoroutined(ref pressCo);
    }
    Coroutine pressCo;
    IEnumerator PressRoutine()
    {
        while (true)
        {
            actions[(int)Define.Key.Press]?.Invoke();
            yield return null;
        }
    }

}