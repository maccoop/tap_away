using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageConfig", menuName = "config/StageConfig")]
public class StageConfig : ScriptableObject
{
    public StageInfo data;
    public List<CubeData> cubes;
}
[System.Serializable]
public class StageInfo: AbstractData
{

}
