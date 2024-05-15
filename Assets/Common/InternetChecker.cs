using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetChecker : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
        var obj = new GameObject(nameof(InternetChecker)).AddComponent<InternetChecker>();
        DontDestroyOnLoad(obj.gameObject);
    }

    private bool noInternet = false;

    public void Update()
    {
        if (noInternet) return;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            noInternet = true;
            //User.AddTracking(ActionType.open_dialog, "no_internet", 0);
            StartCoroutine(DelayCheckInternet());
        }
    }

    private IEnumerator DelayCheckInternet()
    {
        while (Application.internetReachability != NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            yield return new WaitForSeconds(500);
        }
        //User.AddTracking(ActionType.open_dialog, "no_internet", 1);
        noInternet = false;
    }
}
