
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Lotto", menuName = "Scriptable Objects/Data_Lotto")]
public class Data_Lotto : ScriptableObject
{
    public enum LotteryType
    {
        Spito,

    }
    public LotteryType lotteryType;
    public int slotCount = 6; // 슬롯 개수
    public Sprite[] sprites;
    public LottoReward[] rewards;

    [System.Serializable]
    public struct LottoReward
    {
        public int price;
        [Range(0, 100)]
        public float value;
    }

    public struct LottoSlot
    {
        public Sprite sprite;
        public int reward;
    }
    List<LottoSlot> tempSlotList = new List<LottoSlot>();
    // 랜덤 슬롯 세팅
    public List<LottoSlot> SetRandom(out Sprite _mainSprite, out int _sellPrice)
    {
        // 당첨 금액 먼저
        // 당첨 금액이 0일 경우 - 메인 슬롯 모양 정하고 리스트에서 뺀 다음 랜덤
        // 0이 아닌경우 - slotCount번까지 중 랜덤 슬롯 정하고 
        int reward = rewards[ChanceReward()].price;
        List<Sprite> tempSpriteList = new List<Sprite>(sprites);// 리스트 새로 만들기
        Sprite mainSprite = sprites[Random.Range(0, sprites.Length)];
        tempSpriteList.Remove(mainSprite);// 정답 없는 리스트
        tempSlotList.Clear();
        if (reward > 0)
        {
            int randomNode = Random.Range(0, slotCount);// 정답 위치 잡기
            for (int i = 0; i < slotCount; i++)
            {
                if (i == randomNode)
                {
                    LottoSlot lottoSlot = new LottoSlot { reward = reward, sprite = mainSprite };
                    tempSlotList.Add(lottoSlot);
                }
                else
                {
                    Sprite tempSprite = tempSpriteList[Random.Range(0, tempSpriteList.Count)];
                    int tempReward = rewards[Random.Range(1, rewards.Length)].price;// 가격 0 빼고
                    LottoSlot lottoSlot = new LottoSlot { reward = tempReward, sprite = tempSprite };
                    tempSlotList.Add(lottoSlot);
                }
            }
        }
        else
        {
            for (int i = 0; i < slotCount; i++)
            {
                Sprite tempSprite = tempSpriteList[Random.Range(0, tempSpriteList.Count)];
                int tempReward = rewards[Random.Range(1, rewards.Length)].price;// 가격 0 빼고
                LottoSlot lottoSlot = new LottoSlot { reward = tempReward, sprite = tempSprite };
                tempSlotList.Add(lottoSlot);
            }
        }
        _mainSprite = mainSprite;
        _sellPrice = reward;
        return tempSlotList;
    }

    // 확률 구하기
    int ChanceReward()
    {
        float total = 0;
        foreach (var elem in rewards)
        {
            total += elem.value;
        }

        float randomPoint = Random.value * total;
        for (int i = 0; i < rewards.Length; i++)
        {
            if (randomPoint < rewards[i].value)
            {
                return i;
            }
            else
            {
                randomPoint -= rewards[i].value;
            }
        }
        return rewards.Length - 1;
    }
}
