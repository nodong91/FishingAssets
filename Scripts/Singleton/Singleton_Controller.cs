using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Singleton_Controller : MonoSingleton<Singleton_Controller>
{
    public bool outOfControll;
    public delegate void Holder(bool _input);
    Dictionary<KeyCode, Holder> keyCodeSets;

    private void Update()
    {
        if (outOfControll == true)
            return;

        foreach (var dic in keyCodeSets)
        {
            if (Input.GetKeyUp(dic.Key))
            {
                dic.Value(false);
            }
            else if (Input.GetKeyDown(dic.Key))
            {
                dic.Value(true);
                Debug.Log(dic.Key);
            }
        }
        Mouse_Wheel(Input.GetAxis("Mouse ScrollWheel"));
    }

    public void SetController()
    {
        keyCodeSets = new Dictionary<KeyCode, Holder>
        {
            { KeyCode.Q, Key_Q },
            { KeyCode.W, Key_W },
            { KeyCode.E, Key_E },
            { KeyCode.R, Key_R },
            { KeyCode.A, Key_A },
            { KeyCode.S, Key_S },
            { KeyCode.D, Key_D },
            { KeyCode.F, Key_F },
            { KeyCode.Tab, Key_Tab },
            { KeyCode.Escape, Key_Esc },
            { KeyCode.LeftShift, Key_LeftShift },
            { KeyCode.Space, Key_SpaceBar },
            { KeyCode.Alpha1, Key_1 },
            { KeyCode.Alpha2, Key_2 },
            { KeyCode.Alpha3, Key_3 },
            { KeyCode.Alpha4, Key_4 },
            { KeyCode.Alpha5, Key_5 },
            { KeyCode.Alpha6, Key_6 },
            { KeyCode.Alpha7, Key_7 },
            { KeyCode.Alpha8, Key_8 },
            { KeyCode.Mouse0, Mouse_Left },
            { KeyCode.Mouse1, Mouse_Right }
        };
    }

    public delegate void Key_Bool(bool _input);
    public Key_Bool key_Q;
    public Key_Bool key_W;
    public Key_Bool key_E;
    public Key_Bool key_R;
    public Key_Bool key_A;
    public Key_Bool key_S;
    public Key_Bool key_D;
    public Key_Bool key_F;

    public Key_Bool key_Tab;
    public Key_Bool key_Esc;
    public Key_Bool key_LeftShift;
    public Key_Bool key_SpaceBar;

    public Key_Bool key_MouseLeft;
    public Key_Bool key_MouseRight;
    public Key_Bool key_MouseWheel;

    public Key_Bool key_1;
    public Key_Bool key_2;
    public Key_Bool key_3;
    public Key_Bool key_4;
    public Key_Bool key_5;
    public Key_Bool key_6;
    public Key_Bool key_7;
    public Key_Bool key_8;

    void Key_Q(bool _input) { key_Q?.Invoke(_input); }
    void Key_W(bool _input) { key_W?.Invoke(_input); }
    void Key_E(bool _input) { key_E?.Invoke(_input); }
    void Key_R(bool _input) { key_R?.Invoke(_input); }
    void Key_A(bool _input) { key_A?.Invoke(_input); }
    void Key_S(bool _input) { key_S?.Invoke(_input); }
    void Key_D(bool _input) { key_D?.Invoke(_input); }
    void Key_F(bool _input) { key_F?.Invoke(_input); }
    void Key_Tab(bool _input) { key_Tab?.Invoke(_input); }
    void Key_Esc(bool _input) { key_Esc?.Invoke(_input); }
    void Key_LeftShift(bool _input) { key_LeftShift?.Invoke(_input); }
    void Key_SpaceBar(bool _input) { key_SpaceBar?.Invoke(_input); }

    // Äü½½·Ô
    void Key_1(bool _input) { key_1?.Invoke(_input); }
    void Key_2(bool _input) { key_2?.Invoke(_input); }
    void Key_3(bool _input) { key_3?.Invoke(_input); }
    void Key_4(bool _input) { key_4?.Invoke(_input); }
    void Key_5(bool _input) { key_5?.Invoke(_input); }
    void Key_6(bool _input) { key_6?.Invoke(_input); }
    void Key_7(bool _input) { key_7?.Invoke(_input); }
    void Key_8(bool _input) { key_8?.Invoke(_input); }

    // ¸¶¿ì½º
    void Mouse_Left(bool _input)
    {
        if (EventSystem.current.IsPointerOverGameObject() == false || _input == false)
        {
            key_MouseLeft?.Invoke(_input);
        }
    }

    void Mouse_Right(bool _input)
    {
        if (EventSystem.current.IsPointerOverGameObject() == false || _input == false)
            key_MouseRight?.Invoke(_input);
    }

    void Mouse_Wheel(float _value)
    {
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            if (_value > 0)
            {
                key_MouseWheel?.Invoke(true);
            }
            else if (_value < 0)
            {
                key_MouseWheel?.Invoke(false);
            }
        }
    }
}
