using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    protected readonly ThirdPersonMovement _controller;
    public State(ThirdPersonMovement controller)
    {
        _controller = controller;
    }

    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator Attack()
    {
        yield break;
    }

    public virtual IEnumerator Defend()
    {
        yield break;
    }

    public virtual IEnumerator Roam()
    {
       
        yield break;
    }

    public virtual IEnumerator Heal()
    {
        yield break;
    }

    public virtual IEnumerator Pause()
    {
        yield break;
    }
}
