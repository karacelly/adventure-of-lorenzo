using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public enum WeaponSlot
    {
        Primary = 0,
        Secondary = 1
    }

    public Transform crossHairTarget;
    public Animator rigController;
    public Transform[] weaponSlots;
    public CharacterAimingScript playerCamera;
    //public AmmoWidget ammoWidget;


    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
