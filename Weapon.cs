using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public string weaponDescription;
    public GameObject weaponPrefab;
    public int weaponDamage;
    public float weaponCooldown;
    public float weaponMoveForce;
    public int weaponIndex;

    
}
