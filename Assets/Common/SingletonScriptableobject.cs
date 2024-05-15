using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SingletonScriptableObject", menuName = "config/SingletonScriptableObject")]
public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    public static readonly string path = typeof(T).Name;

    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<T>(path);
            }
            return instance;
        }
    }
}
