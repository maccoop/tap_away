using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Threading.Tasks;

public class AppTimer : MonoBehaviour, IDisposable
{
    public UnityEvent OnCompleted = new UnityEvent();
    public UnityEvent OnStart = new UnityEvent();
    public UnityEvent OnStick = new UnityEvent();
    public double Elapse => elapse;
    public double ElapsePercent => elapse / duration;
    public double Remaining => duration - elapse;
    public double RemainingPercent => (duration - elapse) / duration;

    private double elapse;

    private string key;

    private double duration;
    private Func<bool> condition = null;

    private static Dictionary<string, AppTimer> timerDict = new Dictionary<string, AppTimer>();
    private static WaitForFixedUpdate waitForUpdate = new WaitForFixedUpdate();

    public string Key => key;

    private void AddCondition(Func<bool> conditionPlay)
    {
        condition = conditionPlay;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="duration">set -1 to no limit</param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static AppTimer Do(double duration, string key, bool dontDestroy = false, Func<bool> conditionPlay = null)
    {
        if (timerDict.ContainsKey(key))
        {
            Cancle(key);
        }
        var obj = new GameObject();
        //obj.hideFlags = HideFlags.HideAndDontSave;
        obj.name = "Timer " + key;
        var time = obj.AddComponent<AppTimer>();
        time.duration = duration == -1 ? double.MaxValue : duration;
        time.key = key;
        time.AddCondition(conditionPlay);
        time.StartCountDown();
        timerDict.Add(key, time);
        if (dontDestroy)
            DontDestroyOnLoad(obj);
        return time;
    }

    public static async Task<AppTimer> GetTimerAsync(string key)
    {
        AppTimer timer = null;
        while (timer == null)
        {
            if (timerDict.ContainsKey(key))
            {
                timer = timerDict[key];
            }
            await Task.Delay(5);
        }
        return timer;
    }
    public static AppTimer GetTimer(string key)
    {
        if (timerDict.ContainsKey(key))
        {
            return timerDict[key];
        }
        return null;
    }

    public static AppTimer Do(double duration, bool dontDestroy = false)
    {
        return AppTimer.Do(duration, Guid.NewGuid().ToString(), dontDestroy);
    }

    private static void Stop(string key)
    {
        if (timerDict.ContainsKey(key))
        {
            var time = timerDict[key];
            Destroy(time.gameObject);
            timerDict.Remove(key);
            time.OnCompleted?.Invoke();
            time.StopAllCoroutines();
        }
    }

    public static void Cancle(string key)
    {
        if (timerDict.ContainsKey(key))
        {
            var time = timerDict[key];
            timerDict.Remove(key);
            try
            {
                time?.StopAllCoroutines();
                Destroy(time?.gameObject);
            }
            catch
            {

            }
            //time.OnCompleted?.Invoke();
        }
    }

    public static bool HasValue(string key)
    {
        return timerDict.ContainsKey(key);
    }

    public void StopCoundown()
    {
        Stop(key);
    }

    private void StartCountDown()
    {
        OnStart?.Invoke();
        StartCoroutine(CountDown(this.duration));
    }

    private IEnumerator CountDown(double duration)
    {
        while (elapse < duration && gameObject != null)
        {
            if (condition != null && !condition.Invoke())
            {
                yield return waitForUpdate;
                continue;
            }
            elapse += Time.deltaTime;
            if (elapse >= duration)
                elapse = duration;
            OnStick?.Invoke();
            yield return waitForUpdate;
        }
        Stop(key);
    }

    public static double GetElapse(string key)
    {
        if (timerDict.ContainsKey(key))
        {
            var time = timerDict[key];
            return time.Elapse;
        }
        return 0;
    }
    public static double GetRemaining(string key)
    {
        if (timerDict.ContainsKey(key))
        {
            var time = timerDict[key];
            return time.Remaining;
        }
        return 0;
    }
    public static double GetElapsePercent(string key)
    {
        if (timerDict.ContainsKey(key))
        {
            var time = timerDict[key];
            return time.ElapsePercent;
        }
        return 0;
    }
    public static double GetRemainingPercent(string key)
    {
        if (timerDict.ContainsKey(key))
        {
            var time = timerDict[key];
            return time.RemainingPercent;
        }
        return 0;
    }

    public void Dispose()
    {
        Cancle(this.key);
    }

    public void OnDestroy()
    {
        Cancle(this.key);
    }
}

public static class TimeHelper
{
    public static string ToTimer(this double second)
    {
        var day = Mathf.FloorToInt((float)second / 3600f / 24f);
        if (day > 0)
        {
            if (day > 1)
                return $"{day} days";
            return $"{day} day";
        }
        var hour = Mathf.FloorToInt((float)second / 3600f);
        var minute = Mathf.FloorToInt((float)(second - hour * 3600f) / 60f);
        var _second = (int)(second - (hour * 3600f) - (minute * 60));
        if (hour == 0)
        {
            if (minute == 0)
            {
                return $"00:{_second}";
            }
            return $"{minute}:{_second:00}";
        }
        return $"{hour}:{minute:00}:{_second:00}";
    }
}
