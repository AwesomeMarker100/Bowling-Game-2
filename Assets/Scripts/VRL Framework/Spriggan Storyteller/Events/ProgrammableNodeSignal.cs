using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exception = System.Exception;
using System.Data;

public class ProgrammableNodeSignal
{

    private float WAIT_OUT_TIME = 30;

    private List<bool> taskStarted;
    private List<bool> taskCompleted;

    private int expectedSize;
    private int[] hashCodes;

    private TaskCompletionSource<bool> tcs;

    public bool allListenersValid;
    public bool timedOut = false;



    public ProgrammableNodeSignal(int expectedSize, TaskCompletionSource<bool> tcs)
    {
        taskCompleted = new List<bool>();
        taskStarted = new List<bool>();
        allListenersValid = false;
        //hashCodes = ChronosEngine.GenerateUniqueHashCodes(expectedSize, expectedSize * 3);

        this.expectedSize = expectedSize;
        this.tcs = tcs;

        StartTimer();

    }
    public void SetTaskCompleted()
    {
        if (!allListenersValid || tcs.Task.IsCompleted || timedOut) return;
        taskCompleted.Add(true);
        if (taskCompleted.Count == expectedSize) { tcs.SetResult(true); }
    }

    public void SetTaskStarted()
    {
        if (tcs.Task.IsCompleted || timedOut) return;

        taskStarted.Add(true);
        if(taskStarted.Count > expectedSize)
        {
            Debug.LogException(new Exception("Received too many task started calls!"));
            tcs.SetResult(false);
            
        } else if(taskStarted.Count == expectedSize)
        {
            allListenersValid = true;
        }
    }

    public async Awaitable StartTimer()
    {
        await Awaitable.WaitForSecondsAsync(3);
        Debug.Log("timed out");
        timedOut = true;
        tcs.SetResult(false);
    }
}
