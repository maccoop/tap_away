using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerPref : MonoBehaviour
{
    public string playerPrefKey;
    public enum Type
    {
        Float, String, Integer
    }
    public Type type;
    [ShowIf("@this.type==Type.Float")]
    public float valueF;
    [ShowIf("@this.type==Type.String")]
    public string valueS;
    [ShowIf("@this.type==Type.Integer")]
    public int valueI;

    public void OnWrite()
    {
        switch (type)
        {

            case Type.Float:
                {
                    PlayerPrefs.SetFloat(playerPrefKey, valueF);
                    break;
                }
            case Type.String:
                {
                    PlayerPrefs.SetString(playerPrefKey, valueS);
                    break;
                }
            case Type.Integer:
                {
                    PlayerPrefs.SetInt(playerPrefKey, valueI);
                    break;
                }
        }
        PlayerPrefs.Save();
    }
}
