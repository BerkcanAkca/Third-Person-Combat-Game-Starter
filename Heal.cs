using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : State
{
    public Heal(ThirdPersonMovement controller) : base(controller)
    { }

    public override IEnumerator Start()
    {

        Debug.Log("Heal mode on");
        yield return new WaitForSeconds(1);
        _controller.SetState(new Heal(_controller));
    }
}
