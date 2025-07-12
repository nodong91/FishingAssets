using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public Unit_Player player;
    public UI_Inventory inventory;
    public Fishing_Manager fishingManager;
    public Camera_Manager cameraManager;
    public UI_Manager uiManager;
    public UI_Equip equip;

    public static Game_Manager current;

    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        inventory.SetStart();
    }

    void Update()
    {

    }
}
