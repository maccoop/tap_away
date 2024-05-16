using Lean.Touch;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class MapController : MonoBehaviour
{
    public static MapController instance;
    public Transform content;
    public CubeObject prefab;
    public Material materialError;
    public float rotationSpeed;
    public bool isEditor = false;

    //private Dictionary<Int3, CubeObject> items = new();

    private void OnEnable()
    {
        isEditor = false;
        instance = this;
    }

    private void OnDisable()
    {
        instance = null;
    }

    private void OnDestroy()
    {
        instance = null;
    }


    [HideIf("@isEditor"), Button]
    public void Begin()
    {
        isEditor = true;
        SceneView.duringSceneGui += SceneView_duringSceneGui;
    }

    [Button("Stop"), ShowIf("@isEditor")]
    public void Stop()
    {
        isEditor = false;
        SceneView.duringSceneGui -= SceneView_duringSceneGui;
    }

    private void SceneView_duringSceneGui(SceneView obj)
    {
        Event @event = Event.current;
        if (@event.OnKeyUp(KeyCode.LeftShift))
        {
            var ray = HandleUtility.GUIPointToWorldRay(@event.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo))
            {
                var u = hitInfo.point;
                u.y = 0;
                int index = 0;
                var script = hitInfo.collider.GetComponent<CubeObject>();
                if (script != null)
                {
                    index = script.data.position.y + 1;
                }
                CreateObject(u, index);
            }
        }
        if (@event.OnKeyUp(KeyCode.LeftControl))
        {
            var ray = HandleUtility.GUIPointToWorldRay(@event.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo))
            {
                var u = hitInfo.point;
                u.y = 0;
                var script = hitInfo.collider.GetComponent<CubeObject>();
                if (script != null)
                {
                    DestroyImmediate(script.gameObject);
                }
            }
        }
    }

    private void CreateObject(Vector3 position, int yIndex)
    {
        var index = MapConfig.Instance.GetInt3(position, yIndex);
        var items = transform.GetComponentsInChildren<CubeObject>().ToList();
        while (items.Find(x => x.data.position.Equals(index)) != null)
        {
            yIndex += 1;
            index = MapConfig.Instance.GetInt3(position, yIndex);
        }
        var value = MapConfig.Instance.GetPosition(index);
        var item = PrefabUtility.InstantiatePrefab(prefab, content) as CubeObject;
        item.transform.position = value;
        item.data = new CubeData() { direction = Direction.Left, position = index };
        item.UpdateUI();
        //items.Add(index, item);
    }

    /*

    [Button, FoldoutGroup("Action")]
    public void Save(string stageId)
    {
        var data = Resources.Load<StageConfig>("StageData/" + stageId);
        if (data == null)
        {
            data = ScriptableObject.CreateInstance<StageConfig>();
            data.cubes = items.Select(x => x.Value.data).ToList();
            AssetDatabase.CreateAsset(data, $"Assets/Resources/StageData/{stageId}.asset");
        }
        else
        {
            data.cubes = items.Select(x => x.Value.data).ToList();
        }
        AssetDatabase.SaveAssets();
    }

    [Button, FoldoutGroup("Action")]
    public void Load(string stageId)
    {
        foreach (var e in items)
        {
            if (Application.isPlaying)
                Destroy(e.Value.gameObject);
            else
            {
                DestroyImmediate(e.Value.gameObject);
            }
        }
        for (int i = 0; i < content.childCount; i++)
        {
            if (Application.isPlaying)
                Destroy(content.GetChild(i).gameObject);
            else
                DestroyImmediate(content.GetChild(i).gameObject);
        }
        items = new Dictionary<Int3, CubeObject>();
        var data = Resources.Load<StageConfig>("StageData/" + stageId);
        foreach (var e in data.cubes)
        {
            var index = e.position;
            if (items.ContainsKey(index))
            {
                continue;
            }
            var value = MapConfig.Instance.GetPosition(index);
            var item = Instantiate(prefab, content);
            item.transform.position = value;
            item.data = e;
            item.ChangeDirection();
            items.Add(index, item);
        }
    }

    */

    [Button]
    public void RandomDirection()
    {
        var items = transform.GetComponentsInChildren<CubeObject>();
        foreach (var item in items)
        {
            item.data.direction = Direction.None;
            item.UpdateUI();
        }
        foreach (var e in items)
        {
            RandomDirection(e);
        }
    }

    private static bool CheckDirectionError(Int3 a, Int3 b, Direction ad, Direction bd)
    {
        var errorDirection = ad.GetDirectionError();
        switch (ad)
        {
            case Direction.Right:
                {
                    if (a.x > b.x)
                        return false;
                    break;
                }
            case Direction.Up:
                {
                    if (a.y > b.y)
                        return false;
                    break;
                }
            case Direction.Forward:
                {
                    if (a.z > b.z)
                        return false;
                    break;
                }
            case Direction.Left:
                if (a.x < b.x)
                    return false;
                break;
            case Direction.Down:
                {
                    if (a.y < b.y)
                        return false;
                    break;
                }
            case Direction.Back:
                {
                    if (a.z < b.z)
                        return false;
                    break;
                }
        }
        if (bd == errorDirection)
        {
            //Debug.LogError($"Has error direction: {a}><{b}");
            return true;
        }
        return false;
    }


    private void RandomDirection(CubeObject e)
    {
        var list = transform.GetComponentsInChildren<CubeObject>().ToList();
        var outDirection = new List<Direction>();
        while (true)
        {
            var vectorDirection = e.data.GetVectorDirection();
            var direction = DirectionExtension.GetDirectionRandom(outDirection);
            outDirection.Add(direction);
            e.ChangeDirection(direction);
            var sameItems = list.FindAll(x => x.data.position.Contains(e.data.GetVectorDirection()));
            bool success = true;
            foreach (var i in sameItems)
            {
                if (CheckDirectionError(e.data.position, i.data.position, e.data.direction, i.data.direction))
                {
                    success = false;
                    break;
                }
            }
            if (!success)
            {

            }
            else
            {
                break; ;
            }
        }
    }

    [Button]
    public void CheckAvailable()
    {
        var list = transform.GetComponentsInChildren<CubeObject>().ToList();
        foreach (var e in list)
        {
            var sameItems = list.FindAll(x => x.data.position.Contains(e.data.GetVectorDirection()));
            foreach (var i in sameItems)
            {
                if (CheckDirectionError(e.data.position, i.data.position, e.data.direction, i.data.direction))
                {
                    Debug.LogError($"Error between {e.data.position} and {i.data.position}", e.gameObject);
                    e.ChangeColor(materialError);
                }
                else
                {
                    //e.Value.ChangeColor(null);
                }
            }

        }
    }

    [Button]
    public void UpdatePosition()
    {
        var list = transform.GetComponentsInChildren<CubeObject>().ToList();
        foreach (var e in list)
        {

            e.transform.position = MapConfig.Instance.GetPosition(e.data.position);
        }
    }

    [Button]
    public void DestroyAllChild()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
    private void Start()
    {
        RandomDirection();

    }
}
