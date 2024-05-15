using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class QueueService<T>
{
    public int timeDelay => 0;
    private bool isDequeue = false;
    private Queue<T> queue;
    public UnityEvent<T> OnDequeue;
    T current;

    public QueueService()
    {
        OnDequeue = new UnityEvent<T>();
        queue = new Queue<T>();
    }

    public void AddQueue(T obj)
    {
        queue.Enqueue(obj);
        StartQueue();
    }

    private void StartQueue()
    {
        if (isDequeue)
        {
            return;
        }
        if (queue.Count == 0)
        {
            isDequeue = false;
            return;
        }
        isDequeue = true;
        current = queue.Dequeue();
        OnDequeue.Invoke(current);
    }

    internal bool Contains(T value)
    {
        foreach (var e in queue)
        {
            if (e.Equals(value))
            {
                return true;
            }
        }
        return false;
    }

    public void Clean()
    {
        current = default(T);
        isDequeue = false;
        queue.Clear();
    }

    public void EndQueue()
    {
        current = default(T);
        isDequeue = false;
        StartQueue();
    }

    public void RemoveAll()
    {
        queue = new Queue<T>();
        isDequeue = false;
    }
}
