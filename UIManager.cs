using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] CanvasGroup combatMenuCG;
    //[SerializeField] Canvas combatMenu;
    public static UIManager instance;
    ThirdPersonMovement player;
    [SerializeField] Slider healthSlider;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Image cross;
    Vector3 screenPos;
    Camera cam;

    
    private void Awake()
    {
        instance = this;
        
    }

    IEnumerator StartCombatMenu()
    {
        combatMenuCG.gameObject.SetActive(true);
        while (player.inCombat && combatMenuCG.alpha != 1)
        {
            
            //if (combatMenuCG.gameObject.activeInHierarchy == true) yield break;   
            combatMenuCG.alpha += Time.deltaTime * 0.05f;
            
            yield return new WaitForFixedUpdate();
          



        }
        
    }

    IEnumerator CloseCombatMenu()
    {
        while (!player.inCombat && combatMenuCG.alpha != 0)
        {
            
            //if (combatMenuCG.gameObject.activeInHierarchy == true) yield break;


            
            combatMenuCG.alpha -= Time.deltaTime * 0.05f;

            yield return new WaitForFixedUpdate();
            
        }
        combatMenuCG.gameObject.SetActive(false);

    }

  



    private void Start()
    {
        player = ThirdPersonMovement.instance;
        cam = Camera.main;
    }
    private void Update()
    {
        if (player.inCombat)
            StartCoroutine(StartCombatMenu());

        if (!player.inCombat)
            StartCoroutine(CloseCombatMenu());

        TargetCross();
        healthSlider.value = player.currentHP;
        levelText.text = ToRoman(player.level);
    }

    void TargetCross()
    {
        if (player.targetMode == false && cross.enabled == true)
        {
            cross.enabled = false;
        }

        if (player.targetMode == true)
        {
            if (cross.enabled == false) cross.enabled = true;


            screenPos = cam.WorldToScreenPoint(player.targetEnemy.transform.position);

            cross.transform.position = screenPos;
            //Vector3 direction = (cross.transform.position - cam.transform.position).normalized;
            //cross.transform.rotation = Quaternion.Slerp(cross.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5);
        }
        

    }

    public static string ToRoman(int number)
    {
        if ((number < 0) || (number > 3999)) return string.Empty;
        if (number < 1) return string.Empty;
        if (number >= 1000) return "M" + ToRoman(number - 1000);
        if (number >= 900) return "CM" + ToRoman(number - 900);
        if (number >= 500) return "D" + ToRoman(number - 500);
        if (number >= 400) return "CD" + ToRoman(number - 400);
        if (number >= 100) return "C" + ToRoman(number - 100);
        if (number >= 90) return "XC" + ToRoman(number - 90);
        if (number >= 50) return "L" + ToRoman(number - 50);
        if (number >= 40) return "XL" + ToRoman(number - 40);
        if (number >= 10) return "X" + ToRoman(number - 10);
        if (number >= 9) return "IX" + ToRoman(number - 9);
        if (number >= 5) return "V" + ToRoman(number - 5);
        if (number >= 4) return "IV" + ToRoman(number - 4);
        if (number >= 1) return "I" + ToRoman(number - 1);
        return string.Empty;
    }

}
