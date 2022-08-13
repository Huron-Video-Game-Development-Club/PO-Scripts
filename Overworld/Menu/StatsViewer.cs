using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsViewer {
    public int attack;
    public int defense;
    public int speed;
    public int magic;
    public int magicDefense;
    public int dexterity;
    public int con;
    public int crit;
    public int evasion;

    public Weapon weapon;
    public Weapon_Type weaponType;
    public Armor headArmor;
    public Armor bodyArmor;
    public Armor armArmor;
    public Armor accessory;

    public int GetMight() { return attack + weapon.strength; }
    public int GetDefense() { return defense + headArmor.defenseUp + bodyArmor.defenseUp + armArmor.defenseUp + accessory.defenseUp; }
    public int GetMagic() { return magic +  weapon.magicStrength; }
    public int GetMagicDefense() { return magicDefense + headArmor.magicDefenseUp + bodyArmor.magicDefenseUp + armArmor.magicDefenseUp + accessory.magicDefenseUp; }
    public int GetSpeed() { return speed + headArmor.speedUp + bodyArmor.speedUp + armArmor.speedUp + accessory.speedUp; }
    public int GetDexterity() { return dexterity + headArmor.dexterityUp + bodyArmor.dexterityUp + armArmor.dexterityUp + accessory.dexterityUp; }
    public int GetEvasion() { return evasion + headArmor.evasionUp + bodyArmor.evasionUp + armArmor.evasionUp + accessory.evasionUp; }
    public int GetCrit() { return crit + headArmor.critUp + bodyArmor.critUp + armArmor.critUp + accessory.critUp; }
    public int GetCon() { return con; }
}
