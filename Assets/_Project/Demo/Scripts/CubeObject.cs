using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public enum Direction
{
    Left = 0, Up = 1, Right = 2, Down = 3, Forward = 4, Back = 5, None = -1
}

[System.Serializable]
public class CubeData
{
    public Direction direction;
    public Int3 position;

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }

    public Int3 GetVectorDirection()
    {
        var result = Int3.one;
        switch (direction)
        {
            case Direction.Left:
            case Direction.Right:
                result = new Int3(int.MaxValue, position.y, position.z);
                break;
            case Direction.Up:
            case Direction.Down:
                result = new Int3(position.x, int.MaxValue, position.z);
                break;
            case Direction.Forward:
            case Direction.Back:
                result = new Int3(position.x, position.y, int.MaxValue);
                break;
        }
        //Debug.Log($"Get Direction for {ToString()}: {result}");
        return result;
    }

    internal Vector3 GetRaycast()
    {
        switch (direction)
        {
            case Direction.Left:
                return Vector3.left;
            case Direction.Right:
                return Vector3.right;
            case Direction.Up:
                return Vector3.up;
            case Direction.Down:
                return Vector3.down;
            case Direction.Forward:
                return Vector3.forward;
            case Direction.Back:
                return Vector3.back;
        }
        return Vector3.zero;
    }
}

public static class DirectionExtension
{
    public static float GetRotation(this Direction os)
    {
        switch (os)
        {
            case Direction.Up: return -90;
            case Direction.Left: return 0;
            case Direction.Down: return 90;
            case Direction.Right: return -180;
        }
        return 0;
    }

    public static Direction GetDirectionError(this Direction os)
    {
        switch (os)
        {
            case Direction.Up: return Direction.Down;
            case Direction.Left: return Direction.Right;
            case Direction.Down: return Direction.Up;
            case Direction.Right: return Direction.Left;
            case Direction.Forward: return Direction.Back;
            case Direction.Back: return Direction.Forward;
        }
        return Direction.None;
    }

    public static Direction GetDirectionRandom(List<Direction> outDirection)
    {
        if (outDirection.Count >= 6)
        {
            throw new ArgumentException("Can't find Direction for object");
        }
        Direction direction = Direction.None;
        while (direction == Direction.None || outDirection.Contains(direction))
        {
            direction = (Direction)Random.Range(0, 6);
        }
        return direction;
    }
}

[ExecuteInEditMode]
public class CubeObject : MonoBehaviour
{
    private struct CubeObjectChangePosition
    {
        public Vector3 current, last, now;
    }

    public CubeData data;
    public RectTransform ui;
    private bool isRunning = false;
    private CubeObjectChangePosition cubePosition;
    Task<int> coroutine;
    bool isWaitUpdate = false;

    public void Update()
    {
        if (MapController.instance != null && MapController.instance.isEditor)
        {
            cubePosition.now = transform.position;
            if (cubePosition.last != cubePosition.now)
            {
                cubePosition.last = cubePosition.now;
                isWaitUpdate = false;
                coroutine?.Dispose();
            }
            else if (!isWaitUpdate)
            {
                isWaitUpdate = true;
                coroutine = DelayToUPdateInt3();
            }
        }
    }

    private async Task<int> DelayToUPdateInt3()
    {
        await Task.Delay(500);
        if (!isWaitUpdate)
            return 0;
        data.position = MapConfig.Instance.GetInt3(cubePosition.now);
        transform.position = MapConfig.Instance.GetPosition(data.position);
        return 1;
    }

    public void ChangeDirection()
    {
        var index = (int)data.direction;
        index++;
        if (index > 5)
        {
            index = 0;
        }
        data.direction = (Direction)index;
        UpdateUI();
    }
    public void ChangeDirection(Direction direction)
    {
        data.direction = direction;
        UpdateUI();
    }

    public void UpdateUI()
    {
        UpdateDirection();
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        float target = 0;
        switch (data.direction)
        {
            case Direction.Back:
                {
                    target = -90;
                    break;
                }
            case Direction.Forward:
                {
                    target = 90;
                    break;
                }
            default:
                {
                    break;
                }
        }
        var rotation = transform.localRotation;
        rotation.eulerAngles = Vector3.up * target;
        transform.localRotation = rotation;
    }

    public void UpdateDirection()
    {
        var rotation = ui.localRotation;
        rotation.eulerAngles = Vector3.forward * data.direction.GetRotation();
        ui.localRotation = rotation;
    }

    internal void ChangeColor(Material material)
    {
        var renderer = GetComponent<Renderer>();
        var current = renderer.materials[0];
        if (material == null)
            renderer.materials = new Material[1] { current };
        else
            renderer.materials = new Material[2] { current, material };
    }


    internal void MoveObject()
    {
        if (isRunning)
            return;
        isRunning = true;
        gameObject.transform.DOMove(transform.position + data.GetRaycast() * 10, 1f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        }).OnUpdate(() =>
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, data.GetRaycast(), out hit, 0.5f))
            {
                hit.rigidbody.gameObject.GetComponent<CubeObject>().FakeMoveObject(data.GetRaycast() * 0.5f);
                gameObject.transform.DOKill(false);
                data.position = MapConfig.Instance.GetInt3(transform.position);
                var target = MapConfig.Instance.GetPosition(data.position);
                gameObject.transform.DOMove(target, 0.2f).OnComplete(() =>
                {
                    isRunning = false;
                });
                return;
            }
        });
    }

    private void FakeMoveObject(Vector3 direction)
    {
        if (isRunning) return;
        isRunning = true;
        var current = data.position;
        gameObject.transform.DOMove(transform.position + direction * 0.5f, 0.2f).OnComplete(() =>
        {
            isRunning = false;
            gameObject.transform.DOMove(MapConfig.Instance.GetPosition(data.position), 0.2f);
        }).OnUpdate(() =>
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, 0.5f))
            {
                hit.rigidbody.gameObject.GetComponent<CubeObject>().FakeMoveObject(direction * 0.75f);
                gameObject.transform.DOKill(true);
                var target = MapConfig.Instance.GetPosition(data.position);
                gameObject.transform.DOMove(target, 0.2f);
                return;
            }
        });
    }
}
