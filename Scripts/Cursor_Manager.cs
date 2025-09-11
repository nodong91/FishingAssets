using UnityEngine;

public class Cursor_Manager : MonoBehaviour
{
    public Texture2D origin, hand;
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
        //Cursor.SetCursor(hand, new Vector2(hand.width / 3f, 0f), CursorMode.Auto);
        Cursor.SetCursor(hand, new Vector2(0f, 0f), CursorMode.ForceSoftware);
    }

    public void OnMouseExit()
    {
        Cursor.SetCursor(origin, new Vector2(0f, 0f), CursorMode.ForceSoftware);
    }
}
