using UnityEngine;
using UnityEngine.UI;

public class FishingSet : MonoBehaviour
{
    public TMPro.TMP_InputField fishingAmount;
    public TMPro.TMP_InputField lodPower;
    public TMPro.TMP_InputField reelingSpeed;
    public TMPro.TMP_InputField reelingSlip;
    public TMPro.TMP_InputField hitPoint;
    public TMPro.TMP_InputField hitBobberSpeed;

    public TMPro.TMP_InputField fishPower;
    public TMPro.TMP_InputField fishAddAngle;
    public TMPro.TMP_InputField fishSpeed;
    public TMPro.TMP_InputField fishDelayMin;
    public TMPro.TMP_InputField fishDelayMax;
    public TMPro.TMP_InputField fishBobberLength;

    public Button resetButton;
    FishingGame fishingGame;

    void Start()
    {
        fishingGame = GetComponent<FishingGame>();
        resetButton.onClick.AddListener(ResetButton);
        SetDefault();
    }

    void SetDefault()
    {
        FishingGame.FishingLodStruct fishingLodStruct = fishingGame.fishingLodStruct;
        fishingAmount.text = fishingLodStruct.fishingAmount.ToString();
        lodPower.text = fishingLodStruct.lodPower.ToString();
        reelingSpeed.text = fishingLodStruct.reelingSpeed.ToString();
        reelingSlip.text = fishingLodStruct.reelingSlip.ToString();
        hitPoint.text = fishingLodStruct.hitPoint.ToString();
        hitBobberSpeed.text = fishingLodStruct.hitBobberSpeed.ToString();

        Data_Manager.FishStruct fishStruct = fishingGame.fishStruct;
        fishPower.text = fishStruct.fishPower.ToString();
        fishAddAngle.text=fishStruct.fishAddAngle.ToString();
        fishSpeed.text = fishStruct.fishSpeed.ToString();
        fishDelayMin.text = fishStruct.fishDelay.x.ToString();
        fishDelayMax.text = fishStruct.fishDelay.y.ToString();
        fishBobberLength.text = fishStruct.fishBobberLength.ToString();
    }

    void ResetButton()
    {
        FishingGame.FishingLodStruct fishingLodStruct = new FishingGame.FishingLodStruct
        {
            fishingAmount = FloatParse(fishingAmount.text),
            lodPower = FloatParse(lodPower.text),
            reelingSpeed = FloatParse(reelingSpeed.text),
            reelingSlip = FloatParse(reelingSlip.text),
            hitPoint = FloatParse(hitPoint.text),
            hitBobberSpeed = FloatParse(hitBobberSpeed.text),
        };

        Data_Manager.FishStruct fishStruct = new Data_Manager.FishStruct
        {
            fishPower = FloatParse(fishPower.text),
            fishAddAngle = FloatParse(fishAddAngle.text),
            fishSpeed = FloatParse(fishSpeed.text),
            fishDelay = new Vector2(FloatParse(fishDelayMin.text), FloatParse(fishDelayMax.text)),
            fishBobberLength = FloatParse(fishBobberLength.text),
        };

        fishingGame.ResetGame(fishingLodStruct, fishStruct);
    }

    float FloatParse(string _index)
    {
        if (float.TryParse(_index, out float returnIndex))
        {
            return returnIndex;
        }
        return 0.0f;
    }
}
