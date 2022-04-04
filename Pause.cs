using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : State
{
    public Pause(ThirdPersonMovement controller) : base(controller)
    { }

    public override IEnumerator Start()
    {

        Debug.Log("Pause mode on");
        yield return new WaitForSeconds(1);
        _controller.SetState(new Pause(_controller));
    }
}