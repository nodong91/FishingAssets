using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//============================================================================================================
// ±Û·Î¹ú Å¬·¡½º
//============================================================================================================
// ½Ì±ÛÅæ ¸¸µé±â
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance = null;

    public static T INSTANCE
    {
        get
        {
            instance = FindFirstObjectByType(typeof(T)) as T;

            if (instance == null)
            {
                instance = new GameObject("@" + typeof(T).ToString(), typeof(T)).GetComponent<T>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }
}