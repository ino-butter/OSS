using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobQueue : BaseManager
{
    private Queue<Action> jobQueue = new Queue<Action>();

    public void Enqueue(Action action)
    {
        jobQueue.Enqueue(action);
    }

    void Update()
    {
        if (jobQueue.Count == 0)
            return;
        while (jobQueue.Count != 0)
        {
            Action action = jobQueue.Dequeue();
            action.Invoke();
        }
    }
}
