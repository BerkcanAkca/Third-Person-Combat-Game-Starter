using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class Roam : State
{
    public Roam(ThirdPersonMovement controller) : base(controller)
    { }

    public override IEnumerator Start()
    {

        Debug.Log("Roam mode on");
        _controller.inCombat = false;
        _controller.targetMode = false;
        SetVignette();
        yield break;
    }

    public override IEnumerator Attack()
    {
         _controller.SetState(new Attackd(_controller));        
        yield break;
    }

    public override IEnumerator Defend()
    {
        _controller.SetState(new Defendd(_controller));
        yield break;
    }

    void SetVignette()
    {
        _controller.volume = _controller.volume.GetComponent<Volume>();
        if (_controller.volume.profile.TryGet<Vignette>(out var vignette))
        {
            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0f;
            vignette.intensity.overrideState = false;

        }
    }
}