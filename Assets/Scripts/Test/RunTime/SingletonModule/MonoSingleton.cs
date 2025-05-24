using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static volatile T instance;

    public static T Instance => instance ?? (instance = FindObjectOfType(typeof(T)) as T);
}