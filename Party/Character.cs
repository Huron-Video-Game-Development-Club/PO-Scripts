using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base Character Model
// Used by character runtime as reference to base stats
[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Character : ScriptableObject
{
    public string char_name;

    //stats
    public int level;

    public int hp_stat;
    public int sp_stat;
    public int attack_stat;
    public int defense_stat;
    public int magic_stat;
    public int magic_defense_stat;
    public int dexterity_stat;
    public int speed_stat;
    public int con_stat;
    public int crit_stat;
    public int evasion_stat;
    public int attack_break;
    public int shield_repair;
    public int shields;

    // Weapon
    /*public Weapon weapon;
    public Weapon_Type weaponType;
    public Armor headArmor;
    public Armor bodyArmor;
    public Armor armArmor;
    public Armor accessory;*/
    public List<Skill> skills;
    public SkillList skillList;

    // Active time variables
    //!Deprecated
    private float wait_time;
    private float start_time;

    // Bounds for leveling up
    // Unsure of how the leveling up process will work at this time
    public int hp_low;
    public int hp_high;
    public int attack_low;
    public int attack_high;
    public int defense_low;
    public int defense_high;
    public int speed_low;
    public int speed_high;
    public int magic_low;
    public int magic_high;
    public int magic_defense_low;
    public int magic_defense_high;
    public int crit_low;
    public int crit_high;
    public int evasion_low;
    public int evasion_high;

    // For menu display
    public Sprite profile;

    public RuntimeAnimatorController overworldAnimator;
    public RuntimeAnimatorController battleAnimator;

    //---IN BATTLE FUNCTIONS---//
    //---D E P R E C A T E D---//
    //calculate wait time needed for one turn
    public void calculateWaitTime() {
        wait_time = 50f / speed_stat;
        start_time = Time.deltaTime;
    }

    //adjust wait time so that it can be measured
    public void adjustWaitTime() {
        //Debug.Log(Time.deltaTime - start_time);
        //wait_time -= Time.deltaTime - start_time;
        wait_time -= 0.01f;
    }

    //returns wait time
    public float getWaitTime() {
        return wait_time;
    }

    //Attacks Enemy
    /*public void Attack(GameObject enemy)
    {
        int enemy_defense = enemy.GetComponent<EnemyRuntime>().defense;
        float damage = attack_stat * (attack_stat + level) / enemy_defense;
        enemy.GetComponent<Enemy>().hp_stat -= Mathf.RoundToInt(damage);
    }*/
    //---IN BATTLE FUNCTIONS---//


    //---GENERAL FUNCTIONS---//
    //Levels up Character
    public void levelUp() {
        ++level;
        hp_stat += Random.Range(hp_low, hp_high);
        attack_stat += Random.Range(attack_low, attack_high);
        defense_stat += Random.Range(defense_low, defense_high);
        speed_stat += Random.Range(speed_low, speed_high);
    }

    // Returns Total Attack stat
    /*public int GetMight() { return attack_stat + weapon.strength; }
    public int GetDefense() { return defense_stat + headArmor.defenseUp + bodyArmor.defenseUp + armArmor.defenseUp + accessory.defenseUp; }
    public int GetMagic() { return magic_stat +  weapon.magicStrength; }
    public int GetMagicDefense() { return magic_defense_stat + headArmor.magicDefenseUp + bodyArmor.magicDefenseUp + armArmor.magicDefenseUp + accessory.magicDefenseUp; }
    public int GetSpeed() { return speed_stat + headArmor.speedUp + bodyArmor.speedUp + armArmor.speedUp + accessory.speedUp; }
    public int GetDexterity() { return dexterity_stat + headArmor.dexterityUp + bodyArmor.dexterityUp + armArmor.dexterityUp + accessory.dexterityUp; }
    public int GetEvasion() { return evasion_stat + headArmor.evasionUp + bodyArmor.evasionUp + armArmor.evasionUp + accessory.evasionUp; }
    public int GetCrit() { return crit_stat + headArmor.critUp + bodyArmor.critUp + armArmor.critUp + accessory.critUp; }
    public int GetCon() { return con_stat; }*/

   

    //---GENERAL FUNCTIONS---//

    /*
    public Character DeepCopy() {
        Character character = new Character();
        character.hp_stat = hp_stat;
        character.sp_stat = sp_stat;
        character.attack_stat = attack_stat;
        character.defense_stat = defense_stat;
        character.magic_stat = magic_stat;
        character.magic_defense_stat = magic_defense_stat;
        character.dexterity_stat = dexterity_stat;
        character.speed_stat = speed_stat;
        character.con_stat = con_stat;
        character.crit_stat = crit_stat;
        character.evasion_stat = evasion_stat;

        character.weapon = weapon;
        character.headArmor = headArmor;
        character.bodyArmor = bodyArmor;
        character.armArmor = armArmor;
        character.accessory = accessory;
        return character;
    }*/
}
