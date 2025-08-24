using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Exception = System.Exception;

public class ProgrammableNode : SprigganNode
{
    public UnityEvent<ProgrammableNodeSignal> actions;
    public bool isComplete;

    private TaskCompletionSource<bool> tcs;

    public override async Awaitable Execute()
    {
        tcs = new TaskCompletionSource<bool>();

        ProgrammableNodeSignal nodeSignal = new ProgrammableNodeSignal(actions.GetPersistentEventCount(), tcs);
        Debug.Log("started");
        actions.Invoke(nodeSignal);

        if(!nodeSignal.allListenersValid)
        {
            Debug.LogException(new Exception("LISTENERS NOT ALL VALID. EXECUTION OF PROGRAMMABLE NODE FAILED!"));
            return;
        }

        //await tcs.Task;
        await tcs.Task;
        if(tcs.Task.Result) Debug.Log("ended succesfully");
        if (!tcs.Task.Result) Debug.Log("failed");
  
    }

}
