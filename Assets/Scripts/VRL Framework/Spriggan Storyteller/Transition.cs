using UnityEngine;

public class Transition
{
    [SerializeField] bool conditionsMet = false;

    public async Awaitable<int> func()
    {
        await Awaitable.WaitForSecondsAsync(5);
        return 1;
    }


}
