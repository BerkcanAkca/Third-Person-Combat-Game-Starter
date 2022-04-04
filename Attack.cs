using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


public class Attackd : State
{
    ThirdPersonMovement player;
    float timeStamp;
    float slashCD;
    int slashCount = 0;
    public Attackd(ThirdPersonMovement controller) : base(controller)
    { }

    public override IEnumerator Start()
    {
        
        
        player = ThirdPersonMovement.instance;
        
        //AttackWithWeapon(player.currentWeapon);
        SetVignette();
        


        yield break;
    }
    // Attack Hud Enable
    // Target Closest
    //Slow Movement

    public override IEnumerator Attack()
    {
        ComboManager();
        AttackWithWeapon(player.currentWeapon);
        yield break;
    }

    public override IEnumerator Roam()
    {
        _controller.SetState(new Roam(_controller));
        yield break;
    }

    public override IEnumerator Defend()
    {
        _controller.SetState(new Defendd(_controller));
        yield break;
    }


    //Weapon Specific Special Attacks

    void Punch()
    {
        _controller.animator.SetTrigger("slash");
        

    }

    void Slash()
    {

        
        if (slashCount == 0)
        {
            
            _controller.animator.SetTrigger("slash");
            timeStamp = Time.time;
            
        }

        if (slashCount == 1)
        {
           
            _controller.animator.SetTrigger("slash01");
            timeStamp = Time.time;
            
          
        }

        if (slashCount == 2)
        {
            slashCount = -1; //minus 1 here because we increment the slash count after every slash and this way it will be zero after slash
            _controller.animator.SetTrigger("slash02");
            timeStamp = Time.time;
            
            

        }

        
    }

    void ComboManager()
    {
        if (slashCount != 0 && Time.time - timeStamp > _controller.timeBetweenAttacks + 1f)
        {
            slashCount = 0;
        }
    }

    void AttackWithWeapon(int weapon)
    {
        switch (weapon)
        {
            case 0:
                Punch();
                break;
            case 1:
                Slash();
                slashCount++;
                break;
            
        }
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
