using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClickToCubeObject : MonoBehaviour
{
    void Start()
    {
        LeanTouch.OnFingerTap += LeanTouch_OnFingerTap;
    }

    private void LeanTouch_OnFingerTap(LeanFinger obj)
    {
        var ray = Camera.main.ScreenPointToRay(obj.ScreenPosition);
        if (Physics.Raycast(ray, out var hitInfo))
        {
            var script = hitInfo.collider.GetComponent<CubeObject>();
            if (script != null)
            {
                script.MoveObject();
            }
        }
    }
}
