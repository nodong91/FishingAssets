using System.Collections;
using UnityEngine;

public class Controll_Manager : MonoBehaviour
{
    public bool clickLeft, isLeftDrag, inputMouseRight;
    public float clickTime;
    public Vector2 clickPosition;
    public Coroutine clickLefting;

    public LayerMask layerMask;
    public enum RotateType
    {
        Normal,
        Focus
    }
    [SerializeField] private RotateType rotateType;

    void Start()
    {
        Singleton_Controller.INSTANCE.SetController();
        SetMouse();
        SetKey();
    }

    void Key_Esc(bool _input)
    {
        if(_input == true)
            Application.Quit();
    }

    void SetMouse()
    {
        Singleton_Controller.INSTANCE.key_MouseLeft += InputMouseLeft;
        Singleton_Controller.INSTANCE.key_MouseRight += InputMouseRight;
        Singleton_Controller.INSTANCE.key_MouseWheel += InputMouseWheel;
    }

    void SetKey()
    {
        Singleton_Controller.INSTANCE.key_Esc = Key_Esc;
    }

    void InputMouseLeft(bool _input)
    {
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
        rotateType = RotateType.Normal;
        Camera_Manager.current?.InputRotate(_input);
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
        if (_input == true)
        {
            rotateType = RotateType.Focus;
        }
        //else if (inputDir != 0)
        //{
        //    rotateType = RotateType.Normal;
        //}
        Camera_Manager.current?.InputRotate(_input);
    }

    void InputMouseWheel(bool _input)
    {
        float input = _input ? -0.1f : 0.1f;
        Camera_Manager.current?.delegateInputScroll(input);
    }
}
