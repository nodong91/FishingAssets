using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controll_Manager : MonoBehaviour
{
    public bool clickLeft, isLeftDrag, inputMouseRight;
    public float clickTime;
    public Vector2 clickPosition;
    public Coroutine clickLefting;
    [Flags]
    public enum ControllDirection
    {
        None = 0, W = 1 << 0, A = 1 << 1, S = 1 << 2, D = 1 << 3
    }
    public ControllDirection controllDirection = ControllDirection.None;

    public LayerMask layerMask;

    void Start()
    {
        Singleton_Controller.INSTANCE.SetController();
        SetMouse();
        SetKeyCode();
    }

    void Key_Esc(bool _input)
    {
        if (_input == true)
            Application.Quit();
    }

    void SetMouse()
    {
        Singleton_Controller.INSTANCE.key_MouseLeft += InputMouseLeft;
        Singleton_Controller.INSTANCE.key_MouseRight += InputMouseRight;
        Singleton_Controller.INSTANCE.key_MouseWheel += InputMouseWheel;
    }

    void SetKeyCode()
    {
        Singleton_Controller.INSTANCE.key_W += Direction_UP;
        Singleton_Controller.INSTANCE.key_A += Direction_Left;
        Singleton_Controller.INSTANCE.key_S += Direction_Down;
        Singleton_Controller.INSTANCE.key_D += Direction_Right;

        Singleton_Controller.INSTANCE.key_SpaceBar += Key_SpaceBar;
        Singleton_Controller.INSTANCE.key_1 += Key_1;
        Singleton_Controller.INSTANCE.key_2 += Key_2;

        Singleton_Controller.INSTANCE.key_Esc += Key_Esc;
    }

    void RemoveKeyCode()
    {
        Singleton_Controller.INSTANCE.key_W -= Direction_UP;
        Singleton_Controller.INSTANCE.key_A -= Direction_Left;
        Singleton_Controller.INSTANCE.key_S -= Direction_Down;
        Singleton_Controller.INSTANCE.key_D -= Direction_Right;

        Singleton_Controller.INSTANCE.key_SpaceBar -= Key_SpaceBar;
        Singleton_Controller.INSTANCE.key_1 -= Key_1;
        Singleton_Controller.INSTANCE.key_2 -= Key_2;

        Singleton_Controller.INSTANCE.key_Esc -= Key_Esc;
    }

    public Vector2Int dirction;

    public void SetDirection()
    {
        if (controllDirection.HasFlag(ControllDirection.W) == false && controllDirection.HasFlag(ControllDirection.S) == false)
        {
            dirction.y = 0;
        }
        else if (controllDirection.HasFlag(ControllDirection.W))
        {
            dirction.y = 1;
        }
        else if (controllDirection.HasFlag(ControllDirection.S))
        {
            dirction.y = -1;
        }
        if (controllDirection.HasFlag(ControllDirection.A) == false && controllDirection.HasFlag(ControllDirection.D) == false)
        {
            dirction.x = 0;
        }
        else if (controllDirection.HasFlag(ControllDirection.A))
        {
            dirction.x = -1;
        }
        else if (controllDirection.HasFlag(ControllDirection.D))
        {
            dirction.x = 1;
        }
    }

    public void ResetControll()
    {
        dirction = Vector2Int.zero;
        controllDirection = 0;
        Game_Manager.current.PlayerMove();
    }

    public void InputMousetLeft(bool _input)
    {

    }

    void Direction_UP(bool _input)
    {
        if (_input == true)
        {
            controllDirection |= ControllDirection.W;// 넣기
            //dirction.y += 1;
        }
        else
        {
            controllDirection &= ~ControllDirection.W;// 제외
            //dirction.y -= 1;
        }
        Game_Manager.current.PlayerMove();
    }

    void Direction_Left(bool _input)
    {
        if (_input == true)
        {
            controllDirection |= ControllDirection.A;
            //dirction.x -= 1;
        }
        else
        {
            controllDirection &= ~ControllDirection.A;
            //dirction.x += 1;
        }
        Game_Manager.current.PlayerMove();
    }

    void Direction_Down(bool _input)
    {
        if (_input == true)
        {
            controllDirection |= ControllDirection.S;
            //dirction.y -= 1;
        }
        else
        {
            controllDirection &= ~ControllDirection.S;
            //dirction.y += 1;
        }
        Game_Manager.current.PlayerMove();
    }

    void Direction_Right(bool _input)
    {
        if (_input == true)
        {
            controllDirection |= ControllDirection.D;
        }
        else
        {
            controllDirection &= ~ControllDirection.D;
        }
        Game_Manager.current.PlayerMove();
    }

    void Key_1(bool _input)
    {
        if (_input == true)
            QuickSlot(0);
    }

    void Key_2(bool _input)
    {
        if (_input == true)
            QuickSlot(1);
    }

    public void QuickSlot(int _index)
    {
        //Game_Manager.current.QuickSlotAction(_index);
    }

    void Key_SpaceBar(bool _input)
    {
        // 방어 스킬
        // 방패를 가진 무기는 방패막기
        // 양손검은 패링
        // 한손검은 구르기
        // 무기 특징
        if (_input == true)
        {
            Game_Manager.current.PlayerEscape();
        }
    }

    void InputMouseLeft(bool _input)
    {
        Game_Manager.current.InputLeftMouse(_input);

        if (clickLefting != null)
            StopCoroutine(clickLefting);

        if (_input == true)
        {
            clickLeft = false;
            clickTime = Time.time;
            clickPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (isLeftDrag == false)
        {
            clickTime = Time.time - clickTime;
            if (clickTime < 0.15f)
            {
                clickLeft = true;
                //LeftRayCasting();
            }
        }
        clickLefting = StartCoroutine(MouseLeftDrag(_input));
    }

    IEnumerator MouseLeftDrag(bool _input)
    {
        if (_input == true)
        {
            isLeftDrag = false;
            while (isLeftDrag == false)
            {
                Vector2 tempPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                float dist = (tempPosition - clickPosition).magnitude;
                if (dist > 0.01f)
                {
                    isLeftDrag = true;
                }
                yield return null;
            }
        }
    }

    void InputMouseRight(bool _input)
    {
        inputMouseRight = _input;
        Game_Manager.current.InputRightMouse(_input);
        //else if (inputDir != 0)
        //{
        //    rotateType = RotateType.Normal;
        //}
    }

    void InputMouseWheel(bool _input)
    {
        float input = _input ? -1f : 1f;
        Game_Manager.current.cameraManager?.delegateInputScroll(input);
    }
}
