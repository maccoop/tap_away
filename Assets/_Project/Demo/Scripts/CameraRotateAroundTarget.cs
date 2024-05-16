using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraRotateAroundTarget : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 10f;
    private Vector2 swipeDelta;

    private void Start()
    {
        var childs = MapController.instance.GetComponentsInChildren<CubeObject>().ToList();
        Int3 result = Int3.zero;
        var Mx = childs.Max(x => x.data.position.x);
        var mx = childs.Min(x => x.data.position.x);
        var My = childs.Max(x => x.data.position.y);
        var my = childs.Min(x => x.data.position.y);
        var Mz = childs.Max(x => x.data.position.z);
        var mz = childs.Min(x => x.data.position.z);
        result = new Int3((Mx - mx) / 2 + mx, (My - my) / 2 + my, (Mz - mz) / 2 + mz);
        target = new GameObject("target").transform;
        Debug.Log(result);
        target.transform.position = MapConfig.Instance.GetPosition(result);
    }

    void OnEnable()
    {
        LeanTouch.OnFingerUpdate += HandleFingerSwipe;
    }

    void OnDisable()
    {
        LeanTouch.OnFingerUpdate -= HandleFingerSwipe;
    }

    private void HandleFingerSwipe(LeanFinger finger)
    {
        if (target != null && (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Moved) || (Input.GetMouseButton(0)))
        {
            if (finger.ScreenPosition == Vector2.zero || finger.LastScreenPosition == Vector2.zero)
                return;
            swipeDelta = finger.ScreenPosition - finger.LastScreenPosition;
            float rotationX = swipeDelta.y * rotationSpeed * Time.deltaTime;
            float rotationY = -swipeDelta.x * rotationSpeed * Time.deltaTime;

            // Rotate the pivot object around the target
            transform.RotateAround(target.position, Vector3.down, rotationY);
            transform.RotateAround(target.position, -transform.right, rotationX);
        }
    }
}
