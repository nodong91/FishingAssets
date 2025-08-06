using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Title_Manager : MonoBehaviour
{
    public Button startButton, closeButton, unloadButton;
    public LoadingManager loadingManager;

    void Start()
    {
        startButton.onClick.AddListener(StartButton);
        closeButton.onClick.AddListener(CloseButton);
        unloadButton.onClick.AddListener(UnloadButton);

        StartCoroutine(MovingUnit());
    }

    void StartButton()
    {
        loadingManager.OpenLoading();
    }

    void CloseButton()
    {
        loadingManager.CloseLoading();
        OffAll();
    }

    void UnloadButton()
    {
        loadingManager.Unloading();
    }

    public Transform startPoint, endPoint;
    public Transform player;
    public float speed = 0.1f;
    public Material reflectionMaterial;
    public GameObject all;

    public void OffAll()
    {
        StopAllCoroutines();
        all.SetActive(false);
    }

    IEnumerator MovingUnit()
    {
        while (true)
        {
            player.gameObject.SetActive(true);
            float normalize = 0f;
            while (normalize < 1f)
            {
                normalize += Time.deltaTime * speed;
                Vector3 startPosition = new Vector3(startPoint.position.x, player.transform.position.y, startPoint.position.z);
                Vector3 endPosition = new Vector3(endPoint.position.x, player.transform.position.y, endPoint.position.z);
                player.transform.position = Vector3.Lerp(startPosition, endPosition, normalize);
                yield return null;

                string shipPosition = "_ShipPosition";
                reflectionMaterial.SetVector(shipPosition, player.position);
            }
            player.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);

            player.position = startPoint.position;
        }
    }
}
