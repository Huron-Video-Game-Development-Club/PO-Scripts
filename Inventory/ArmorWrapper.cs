using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Wrapper class for armor
// Contains quantity of armor and armor itself
public class ArmorWrapper
{
    Armor armor;
    int quantity;

    // Constructor - Initializes armor as armor_input and sets quantity to 1
    public ArmorWrapper(Armor armorInput) {
        armor = armorInput;
        quantity = 1;
    } // ArmorWrapper()

    // Constructor - Initializes armor as armor_input and quantity as quantity_input
    public ArmorWrapper(Armor armorInput, int quantityInput) {
        armor = armorInput;
        quantity = quantityInput;
    } // ArmorWrapper()

    public Armor_Type GetArmorType() { return armor.type; }
    public Armor GetArmor() { return armor; }
    // Returns quantity of armor
    public int GetQuantity() { return quantity; }

    // Loses an armor by decreasing quantity
    public void Lose() { --quantity; }

    // Obtain an armor by increasing its quantity
    public void Obtain() { ++quantity; }

    public void Obtain(int numObtained) { quantity += numObtained; }

    public string GetName() { return armor.armorName; }

    /* To be implemented in the future when we have sprite icons for equiptables
    public Sprite GetIcon() {
        return armor.icon;
    }*/
}

