using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Wrapper class for Weapons
// Contains quantity of weapon and weapon itself
public class WeaponWrapper
{
    Weapon weapon;
    int quantity;

    // Constructor - Initializes weapon as weapon_input and sets quantity to 1
    public WeaponWrapper(Weapon weaponInput) {
        weapon = weaponInput;
        quantity = 1;
    }

    // Constructor - Initializes weapon as weapon_input and quantity as quantity_input
    public WeaponWrapper(Weapon weaponInput, int quantityInput) {
        weapon = weaponInput;
        quantity = quantityInput;
    }

    public Weapon_Type GetWeaponType() { return weapon.type; }
    public Weapon GetWeapon() { return weapon; }
    public int GetStrength() { return weapon.strength; }
    // Returns quantity of weapon
    public int GetQuantity() { return quantity; }

    // Loses a weapon by decreasing quantity
    public void Lose() { --quantity; }

    // Obtain a weapon by increasing its quantity
    public void Obtain() {
        ++quantity;
    }

    public void Obtain(int numObtained)
    {
        quantity += numObtained;
    }

    public string GetName()
    {
        return weapon.weaponName;
    }

    /*public Sprite GetIcon() {
        return weapon.icon;
    }*/
}
