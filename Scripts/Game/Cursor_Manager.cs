using UnityEngine;

public class Cursor_Manager : MonoBehaviour
{
    public enum Cursor_Type
    {
        NORMAL,
        HAND,
        REPAIR
    }
    public Cursor_Type type;
    public Texture2D origin, hand, repair;
    public static Cursor_Manager current;

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        OnMouseExit();
    }

    public void OnMouseOver()
    {
        if (type != Cursor_Type.REPAIR)
        {
            type = Cursor_Type.HAND;
            Cursor.SetCursor(hand, new Vector2(hand.width / 3f, 0f), CursorMode.ForceSoftware);
        }
    }

    public void OnMouseExit()
    {
        if (type != Cursor_Type.REPAIR)
        {
            type = Cursor_Type.NORMAL;
            Cursor.SetCursor(origin, new Vector2(0f, 0f), CursorMode.ForceSoftware);
        }
    }

    public void OnMouseRepair(bool _repair)
    {
        if (_repair == true)
        {
            type = Cursor_Type.REPAIR;
            Cursor.SetCursor(repair, new Vector2(repair.width / 3f, 0f), CursorMode.ForceSoftware);
        }
        else
        {
            type = Cursor_Type.NORMAL;
            Cursor.SetCursor(origin, new Vector2(0f, 0f), CursorMode.ForceSoftware);
        }
    }
}
