using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Reward
{
    [Title("If isIAP, reward will include ProductData")]
    public RewardType type;
    private string alias;
    public int amount;

    public string Alias
    {
        get
        {
            return alias;
        }
    }


    [System.Serializable]
    public enum RewardType
    {

    }
}