using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using static Data_Manager;

public class P01_Utility
{
    // 랜덤 배열 구하기
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);
        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        return array;
    }

    public static List<T> ShuffleList<T>(List<T> list, int seed)
    {
        System.Random prng = new System.Random(seed);
        for (int i = 0; i < list.Count - 1; i++)
        {
            int randomIndex = prng.Next(i, list.Count);
            T tempItem = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = tempItem;
        }
        return list;
    }

    public static Queue<T> ShuffleQueue<T>(List<T> list, int seed)
    {
        Queue<T> tempQueue = new Queue<T>();
        System.Random prng = new System.Random(seed);
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = prng.Next(i, list.Count);
            T tempItem = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = tempItem;
            tempQueue.Enqueue(tempItem);
        }
        return tempQueue;
    }

    // 확률 구하기
    public static int Chance(float[] probs)
    {
        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }

    // 네비메쉬 위치 리턴
    public static Vector3 NavmeshSamplePosition(Vector3 targetPos)
    {
        // 충돌 위치와 가장 가까운 네비메쉬 위치 리턴
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 100.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return targetPos;
    }

    // 가까운 오브젝트 찾기
    public static GameObject FindClosest(Vector3 center, List<GameObject> objList)
    {
        GameObject closestObject = null;
        float closestDistSqr = Mathf.Infinity;

        for (int i = 0; i < objList.Count; i++)
        {
            Vector3 objectPos = objList[i].transform.position;
            float dist = (objectPos - center).sqrMagnitude;

            if (dist < closestDistSqr)
            {
                closestObject = objList[i].gameObject;
                closestDistSqr = dist;
            }
        }
        return closestObject;
    }

    // UI클릭 체크
    public static bool TryUIClick()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public static Color HexToColor(string hex)// 6 or 8자리
    {
        hex = "#" + hex;
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    public static string ColorToHex(Color color)
    {
        return ColorUtility.ToHtmlStringRGBA(color);
    }

    public static float GetAngle(Vector2 start, Vector2 end)
    {
        Vector2 v2 = end - start;
        return Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
    }

    public static string ClassColor(ItemStruct.ItemClass _class)
    {
        string color = "000000";
        switch (_class)
        {
            case ItemStruct.ItemClass.Common:
                color = "B3B3B3";
                break;
            case ItemStruct.ItemClass.Uncommon:
                color = "00FF29";
                break;
            case ItemStruct.ItemClass.Rare:
                color = "005CFF";
                break;
            case ItemStruct.ItemClass.Epic:
                color = "FF1300";
                break;
            case ItemStruct.ItemClass.Legendary:
                color = "FFDD00";
                break;
        }
        return color;
    }

    public static void FindFolder(string folderName)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(folderName);
        if (dirInfo.Exists == false)
        {
            // 없으면 만들기
            dirInfo.Create();
        }
    }
}