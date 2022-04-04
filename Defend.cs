using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class Defendd : State
{
    public Defendd(ThirdPersonMovement controller) : base(controller)
    { }

    public override IEnumerator Start()
    {

        Debug.Log("Defend mode on");
        GuardPose();
        SetVignette();
        yield break;
        
    }

    public override IEnumerator Defend()
    {
        GuardPose();
        yield break;
    }

    void GuardPose()
    {
        //Debug.Log("Guarding");
    }
    public override IEnumerator Attack()
    {
        _controller.SetState(new Attackd(_controller));
        yield break;
    }
    public override IEnumerator Roam()
    {
        _controller.SetState(new Roam(_controller));
        yield break;
    }

    void SetVignette()
    {
        _controller.volume = _controller.volume.GetComponent<Volume>();
        if (_controller.volume.profile.TryGet<Vignette>(out var vignette))
        {
            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0.35f;
        }
    }
}
