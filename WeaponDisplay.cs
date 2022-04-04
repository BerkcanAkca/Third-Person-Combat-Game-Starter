using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDisplay : MonoBehaviour
{   
    public Weapon[] weapons;
    Weapon weapon;
    ThirdPersonMovement player;
    public string weaponName;
    public string weaponDescription;
    public GameObject weaponPrefab;
    public int weaponDamage;
    public float weaponCooldown;
    public float weaponMoveForce;
    public static WeaponDisplay instance;
    [SerializeField] Collider weaponCollider;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
        player = ThirdPersonMovement.instance;
        weapon = weapons[player.currentWeapon];
        weaponName = weapon.name;
        weaponDescription = weapon.weaponDescription;
        weaponPrefab = weapon.weaponPrefab;
        weaponDamage = weapon.weaponDamage;
        weaponCooldown = weapon.weaponCooldown;
        weaponMoveForce = weapon.weaponMoveForce;
        
    }

    private void Update()
    {
        ApplyCurrentWeapon(player.currentWeapon);
        
        WeaponColliderOn();
        WeaponColliderOff();


    }

    void ApplyCurrentWeapon(int index)
    {
        if (index == weapon.weaponIndex) return;

        weapon = weapons[index];
        weaponName = weapon.name;
        weaponDescription = weapon.weaponDescription;
        weaponPrefab = weapon.weaponPrefab;
        weaponDamage = weapon.weaponDamage;
        weaponCooldown = weapon.weaponCooldown;
        weaponMoveForce = weapon.weaponMoveForce;
        
    }

    public void WeaponColliderOn()
    {
        if (player.isAttacking)
        {
            if (weaponCollider.enabled == true) return;
            weaponCollider.enabled = true;
           
        }
        
    }

    public void WeaponColliderOff()
    {
        if (!player.isAttacking)
        {
            if (weaponCollider.enabled == false) return;
            weaponCollider.enabled = false;
           
        }
        
    }
}
