using System.Collections;
using System;
using System.Collections.Generic;
using Application;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Hell incarnate
// Class for managing character turn / stats
public class CharacterRuntime : MonoBehaviour {
    // Reference to character (base stats)
    public Character character;

    // Current target
    private GameObject target;

    // UI References
    private TagUI character_tag;
    private ButtonWrapper character_button;
    
    public Animator animator;

    // Stats
    private int attack;
    public int defense;
    private int speed;
    public int magic;
    public int magicDefense;
    public int dexterity;
    public int con;
    public int crit;
    public int evasion;
    public int hp = 0;
    public int sp = 0;
    public int shields;
    private int level;
    private int exp;
    private int expMax;
    private int attack_break;
    private int shield_repair;

    private bool defending;
    public string char_name;
    private int max_shields;

    // Equipment
    public Weapon weapon;
    public Weapon_Type weaponType;
    public Armor headArmor;
    public Armor bodyArmor;
    public Armor armArmor;
    public Armor accessory;

    // Old code - may be useful in the future
    // =============================================
    // Checks if a non-stackable skill is active
    // private bool nonStackableSkillActive = false;
    // =============================================
    
    private bool onAttackedEventsReady = false;
    public UnityEvent onAttacked;

    // Used to check if the character is attack ready
    private bool attack_ready = false;
    private bool inAnimation = false;

    // This should be just the queue, but oh well
    // Used to store output damage, so it can be accessed later 
    // to output damage to the player
    // See more in DamageEffect.cs
    public int damage_out;
    public Queue<int> damageOut;

    private int moveGauge;
    public List<Buff> buffs;
    
    // Can't remember if this was important
    //list of skills
    //public List<Skill> skills;

    // Start is called before the first frame update
    void Start() {
        char_name = character.char_name;
        attack = character.attack_stat + weapon.strength;
        defense = character.defense_stat + headArmor.defenseUp + bodyArmor.defenseUp + armArmor.defenseUp + accessory.defenseUp; 
        speed = character.speed_stat + headArmor.speedUp + bodyArmor.speedUp + armArmor.speedUp + accessory.speedUp;
        magic = character.magic_stat + weapon.magicStrength;
        magicDefense = character.magic_defense_stat + headArmor.magicDefenseUp + bodyArmor.magicDefenseUp + armArmor.magicDefenseUp + accessory.magicDefenseUp;
        dexterity = character.dexterity_stat + headArmor.dexterityUp + bodyArmor.dexterityUp + armArmor.dexterityUp + accessory.dexterityUp;
        evasion = character.evasion_stat + headArmor.evasionUp + bodyArmor.evasionUp + armArmor.evasionUp + accessory.evasionUp;
        crit = character.crit_stat + headArmor.critUp + bodyArmor.critUp + armArmor.critUp + accessory.critUp;
        con = character.con_stat;
        
        if(hp == 0) {
            hp = character.hp_stat;
        }
        
        if(sp == 0) {
            sp = character.sp_stat;
        }

        level = character.level;
        exp = 0;
        expMax = 10;
        shields = character.shields;
        attack_break = character.attack_break;
        shield_repair = character.shield_repair;
        max_shields = shields;
        character.skillList.Setup(this);
    } // Start()

    // Prepare character runtime to enter battle
    // Called at the beginning of every battle in BattleManager.Start()
    public void EnterBattle() {
        attack_ready = false;
        GetComponent<Animator>().runtimeAnimatorController = character.battleAnimator;
        animator = GetComponent<Animator>();
        
        // Subject to change
        attack = character.attack_stat + weapon.strength;
        defense = character.defense_stat + headArmor.defenseUp + bodyArmor.defenseUp + armArmor.defenseUp + accessory.defenseUp; 
        speed = character.speed_stat + headArmor.speedUp + bodyArmor.speedUp + armArmor.speedUp + accessory.speedUp;
        magic = character.magic_stat + weapon.magicStrength;
        magicDefense = character.magic_defense_stat + headArmor.magicDefenseUp + bodyArmor.magicDefenseUp + armArmor.magicDefenseUp + accessory.magicDefenseUp;
        dexterity = character.dexterity_stat + headArmor.dexterityUp + bodyArmor.dexterityUp + armArmor.dexterityUp + accessory.dexterityUp;
        evasion = character.evasion_stat + headArmor.evasionUp + bodyArmor.evasionUp + armArmor.evasionUp + accessory.evasionUp;
        crit = character.crit_stat + headArmor.critUp + bodyArmor.critUp + armArmor.critUp + accessory.critUp;
        con = character.con_stat;
        // hp = character.hp_stat;
        // sp = character.sp_stat;
        // level = character.level;
        shields = character.shields;
        attack_break = character.attack_break;
        shield_repair = character.shield_repair;
        max_shields = shields;
        character.skillList.Setup(this);
        buffs = new List<Buff>();
        damageOut = new Queue<int>();
       
        // Wrapped these in try catch blocks because I'm scared of errors
        // If this fails, player may be able to still move around
        try {
            GetComponent<FollowMain>().enabled = false;
        } catch (NullReferenceException) {
            return;
        }
        
        try {
            GetComponent<PlayerController>().enabled = false;
        } catch (NullReferenceException) {
            return;
        }
    } // EnterBattle()

    // Re-enable movement at the end of battle
    // Called just before the battle scene manager switches scenes and suicides
    //!Consider removing all buffs here instead of resetting stats at beginning of battle
    public void ExitBattle() {
        GetComponent<Animator>().runtimeAnimatorController = character.overworldAnimator;
        if (gameObject == Party.Instance.current_party[0]) {
            try {
                GetComponent<PlayerController>().enabled = true;
            } catch (NullReferenceException) {
                return;
            }
        } else {
            try {
                GetComponent<FollowMain>().enabled = true;
            } catch (NullReferenceException) {
                return;
            }
        }
    } // ExitBattle()

    // Equip a weapon
    // Input: wrappedWeapon -> weapon to be equipped
    public void Equip(WeaponWrapper wrappedWeapon) {
        Party.Instance.inventory.AddWeapon(weapon);
        weapon = wrappedWeapon.GetWeapon();
        UpdateStats();
    } // Equip() | weapon

    // Equip an armor
    // Input: armor -> armor to be equipped
    public void Equip(ArmorWrapper armor) {
        switch(armor.GetArmorType()) {
            case Armor_Type.Head:
                Party.Instance.inventory.AddArmor(headArmor);
                headArmor = armor.GetArmor();
                Debug.Log(headArmor.armorName);
                Debug.Log("HI");
                break;
            
            case Armor_Type.Body:
                Party.Instance.inventory.AddArmor(bodyArmor);
                bodyArmor = armor.GetArmor();
                break;
            
            case Armor_Type.Arm:
                Party.Instance.inventory.AddArmor(armArmor);
                armArmor = armor.GetArmor();
                break;
            
            case Armor_Type.Accessory:
                Party.Instance.inventory.AddArmor(accessory);
                accessory = armor.GetArmor();
                break;
        }
        UpdateStats();
    } // Equip() | armor

    // Ends defend
    // Called just before a character moves
    public void StartTurn() {
        if(defending == true) {
            defending = false;
            bool exists = false;
            
            /* Since not every character has a defend animation yet (*cough* ace *cough*),
             * check whether the animator controller has a param named "isDefending"
             *   > "isDefending" would associate with a defend animation
             * before trying to disable the defend animation
             */
            foreach (AnimatorControllerParameter param in animator.parameters) {
                if (param.name == "isDefending") {
                    exists = true;
                    break;
                }   
            }
            
            if(exists) {
                animator.SetBool("isDefending", false);
            } 
            // EndAnimation("isDefending");
        }
    } // StartTurn()

    public int GetMoveGauge() { return moveGauge; }
    public void ResetMoveGauge() { moveGauge = 0; }

    public void AdjustMoveGauge() {
      moveGauge += (int)Mathf.Ceil(((speed+20)*6) * Time.deltaTime);
      // Debug.Log(moveGauge + " out of " + BattleManager.Instance.MAX_MOVE_GAUGE);
      character_tag.UpdateATB();
    } // AdjustMoveGauge()

    // DEPRECATED?
    // =====================================
    // Calculates wait time until next turn
    public void calculateWaitTime() {
        character.calculateWaitTime();
    }

    // Updates wait time until next turn
    public void adjustWaitTime() {
        character.adjustWaitTime();
    }

    //returns wait time until next turn
    public float getWaitTime() {
        return character.getWaitTime();
    }
    // =====================================

    // Sets target reference for skills, combos, and items
    public void SetTarget(GameObject target_input) {
        target = target_input;
    } // SetTarget()

    // =============
    // BASE COMMANDS
    // =============
   
    // Attacks enemy game object
    // Corresponds with attack function in command grid
    // Todo: Implement critical hits
    // Todo: Implement chain break calculations
    public void Attack(GameObject enemy) {
        int enemy_defense = enemy.GetComponent<EnemyRuntime>().defense;
        int damage_upper = Mathf.RoundToInt(attack * (attack + level + 1) / enemy_defense);
        int damage_lower = Mathf.RoundToInt(attack * (attack + level - 1) / enemy_defense);
        int damage = UnityEngine.Random.Range(damage_lower, damage_upper);
        
        EnemyRuntime enemy_stats = enemy.GetComponent<EnemyRuntime>();
        enemy_stats.hp -= damage;
        enemy_stats.LoseShields(attack_break);
        target = enemy;
        damage_out = damage;
        Debug.Log(animator);
        inAnimation = true;
        animator.SetBool("isAttacking", true);
        Debug.Log(animator.GetBool("isAttacking"));
        Debug.Log("ENEMY SHIELDS: " + enemy.GetComponent<EnemyRuntime>().GetShields());
    } // Attack()

    // Sets defending to be true
    // Enemy calculation checks this switch and halves damage if on
    // Corresponds with defend function in command grid
    public void Defend() {
        defending = true;
        shields += shield_repair;
        if(shields > max_shields)
        {
            shields = max_shields;
        }
        bool exists = false;
        foreach (AnimatorControllerParameter param in animator.parameters) {
            if (param.name == "Defend") {
                exists = true;
            }   
        }
        if(exists) {
            animator.SetBool("Defend", true);
        } else {
            FinishAttack();
            BattleManager.Instance.allowMovement();
            Debug.Log("HI!");
        }
    } // Defend()

    // Use a skill with character as a reference
    public void UseSkill(int skill_index) {
        /* Massive deprecation that I'm scared to delete
         * I don't trust plastic scm lmao
        switch (character.skills[skill_index].use_type)
        {
            case Use_Type.Enemy:
                target.GetComponent<EnemyRuntime>().LoseShields(character.skills[skill_index].breakPower);
                break;
            case Use_Type.Party:
                //target.GetComponent<CharacterRuntime>().LoseShields(breakPower);
                break;
            case Use_Type.Both:
                if (target.GetComponent<EnemyRuntime>() != null)
                {
                    //target.GetComponent<EnemyRuntime>().AddShields(shieldRepair);
                }
                break;
            case Use_Type.Enemy_All:
                if (character.skills[skill_index].randomize_targets)
                {
                    for (int i = 0; i < character.skills[skill_index].attack_times; ++i)
                    {
                        GameObject randTarget = battleManager.GetEnemies()[Random.Range(0, battleManager.GetEnemies().Count)];
                        if (randTarget.GetComponent<EnemyRuntime>().hasWeakness(character.skills[skill_index].affinity))
                        {
                            randTarget.GetComponent<EnemyRuntime>().LoseShields(character.skills[skill_index].breakPower);
                            //BOOST POWER (list?) then turn off
                        }
                        character.skillList.AddRandomTarget(randTarget);
                    }
                }
                break;
        }
        */

        character.skillList.SetPower(character.skills[skill_index].breakPower, character.skills[skill_index].shieldRepair, character.skills[skill_index].affinity);
        character.skills[skill_index].Use(this);
    } // UseSkill()



    // Applies a buff to character
    // Buff buff -> buff being applied
    public void ApplyBuff(Buff buff) {
        if (buff.type == Buff.Buff_Type.ATTACK)
        {
            attack = (int)(attack*buff.power);
        }
        else if (buff.type == Buff.Buff_Type.DEFENSE)
        {
            defense = (int)(defense*buff.power);
        }
        else if (buff.type == Buff.Buff_Type.SPEED)
        {
            speed = (int)(speed*buff.power);
        } else if (buff.type == Buff.Buff_Type.ACCURACY)
        {
            speed = (int)(speed*buff.power);
        }
        buffs.Add(buff);
    } // Buff()

    // Undos buff once turn limit is up and removes it from "buffs"
    // Buff buff -> buff being removed
    // int buffIndex -> index of buff
    public void UnBuff(Buff buff, int buffIndex) {
        if (buff.type == Buff.Buff_Type.ATTACK)
        {
            attack = (int)(attack/buff.power);
        }
        else if (buff.type == Buff.Buff_Type.DEFENSE)
        {
            defense = (int)(defense/buff.power);
        }
        else if (buff.type == Buff.Buff_Type.SPEED)
        {
            speed = (int)(speed/buff.power);
        }
        buffs.RemoveAt(buffIndex);
    } // UnBuff()

    // End an animation without finishing the current turn
    // Input: transition_bool -> bool controlling current animation in animator 
    public void EndAnimation(string transition_bool) {
        animator.SetBool(transition_bool, false);
        inAnimation = false;
    } // EndAnimation()

    // End an animation and finish the current turn
    // Input: transition_bool -> bool controlling current animation in animator
    public void EndAnimationFinishTurn(string transition_bool) {
        animator.SetBool(transition_bool, false);
        FinishAttack();
        inAnimation = false;
    } // EndAnimationFinishTurn()

    // End an animation while still attacking
    // Used in skills like Nico's lightning dance
    void EndAnimationStillAttacking(string transitionBool) {
        animator.SetBool(transitionBool, false);
    } // EndAnimationStillAttacking()

    // Dunno where this is used, but okay
    // Starts an animation and allows next object to move
    public void StartAnimation(string transition_bool_on) {
        animator.SetBool(transition_bool_on, true);
        BattleManager.Instance.allowMovement(); // Shitty naming conventions btw
    } // StartAnimation()

    // Nah, I'm not explaining this one
    public void StartAnimationStillAttacking(string transition_bool_on) {
        animator.SetBool(transition_bool_on, true);
    } // StartAnimationStillAttacking()

    // Spawn a slash animation over target
    // Sets spawnMultiple to false, indicating only one slash animation will spawn
    void StartSlashAnim() {
        SlashEffectManager.Spawn(target, this, damage_out, false);
        try {
            AudioManager.Instance.PlayNoNext("Slash");
        } catch {
            return;
        }
    } // StartSlashAnim()

    // Spawn a slash animation on multiple targets at once
    void StartSlashAnimAll() {
        foreach(GameObject enemy in BattleManager.Instance.GetEnemies()) {
            SlashEffectManager.Spawn(enemy, this, damageOut.Peek(), false);
            damageOut.Dequeue();
        }
        try {
            AudioManager.Instance.PlayNoNext("Slash");
        } catch {
            return;
        }
    } // StartSlashAnimAll()

    // Spawn a slash animation that will correspond with a skill to start others
    // Sets spawnMultiple to true, indicating more than one slash animation will spawn
    void StartSlashAnimMultiple() {
        SlashEffectManager.Spawn(character.skillList.GetRandTargets()[character.skillList.GetRandTargets().Count - 1], this, damage_out, true);
        try {
            AudioManager.Instance.PlayNoNext("Slash");
        } catch {
            return;
        }
    } // StartSlashAnimMultiple()

    // Indicate a character is ready to attack
    // Updates buffs
    // Called once ATBGauge is full
    public void AttackReady() {
        attack_ready = true;
        for(int i = 0; i < buffs.Count; ++i) {
            buffs[i].UpdateBuff();
            if(buffs[i].GetTurnsLeft() <= 0) {
                int buffIndex = i;
                this.UnBuff(buffs[i], buffIndex);
            }
        }
    } // AttackReady()

    // Resets move guage after attack is finished
    // Called at the end of a spawned slash animation (through an animation event) 
    public void FinishAttack() {
        attack_ready = false;
        ResetMoveGauge();
    } // FinishAttack()

    //!Shitty namimg conventions
    //!Change to camal case
    public bool isAttackReady() {
        return attack_ready;
    } // IsAttackReady()

    // I legit have no clue why this is here
    // This is sad
    // I think I wanted to spawn certain effects when a character was attacked(?)
    // This has something to do with that
    public void SetOnAttackedEventsReady() {
        onAttackedEventsReady = true;
    }

    // Same here
    public bool HasOnAttackedEvents() {
        return onAttackedEventsReady;
    }

    /* WARNING: returns through base character reference
     * 
     * Why is this important?
     * ----------------------
     * I still don't know if I want to update character reference on level up, 
     * but I probably won't be doing that.
     * 
     * This should probably be changed to a variable stored locally in this class
     */
    // Return character skills
    public List<Skill> GetSkills() {
        return character.skills;
    } // GetSkills()

    // ==
    // UI
    // ==
    // Sets Tag to be used for updating hp and sp values on screen
    public void SetCharTag(TagUI tag) {
        character_tag = tag;
    } // SetCharTag()

    /* In order to manage UI Tags (top right corner of battle)
     * References to them must be told to update informational bars every time that information is affected
     * i.e.: When a character uses a skill, it costs sp -> sp bar must be updated
     */
    public void UpdateHP() { character_tag.UpdateHP(); }
    public void UpdateSP() { character_tag.UpdateSP(); }

    // Sets button for selecting (items, skills, combos)
    public void SetButton(ButtonWrapper button) {
        character_button = button;
    } // SetButton()

    // Returns button associated with character
    // (The button that points to the character when they are being selected for skill/item use)
    // Can't remember what this is used for, probably in the BattleMenuManager somewhere
    public Button GetButton() {
        return character_button.gameObject.GetComponentInChildren<Button>();
    } // GetButton()

    // Specify directional input
    // Tells eventsystem to migrate to inputted character when characters are being selected and right and up keys are pressed 
    // Input: character_ref -> character to direct to on right and up key presses
    public void ButtonSetRightAndUp(CharacterRuntime character_ref) {
        //Debug.Log(character_button == null);
        Navigation nav = character_button.gameObject.GetComponentInChildren<Button>().navigation;
        nav.selectOnRight = character_ref.GetButton();
        nav.selectOnUp = character_ref.GetButton();
        character_button.gameObject.GetComponentInChildren<Button>().navigation = nav;
    } // ButtonSetRightAndUp()

    // Specify directional input
    // Tells eventsystem to migrate to inputted character when characters are being selected and left and down keys are pressed 
    // Input: character_ref -> character to direct to on left and down key presses
    public void ButtonSetLeftAndDown(CharacterRuntime character_ref) {
        Navigation nav = character_button.gameObject.GetComponentInChildren<Button>().navigation;
        nav.selectOnLeft = character_ref.GetButton();
        nav.selectOnDown = character_ref.GetButton();
        character_button.gameObject.GetComponentInChildren<Button>().navigation = nav;
    } // ButtonSetLeftAndDown()

    public void LevelUp() {
        ++level;
        expMax += 4 * level;
        exp = 0;
    } // LevelUp()

    public void SetEXP(int expIn) {
        exp = expIn;
    }

    // GETTERS USED FOR SETING UP CHARACTER TAGS
    public string GetName() { return character.char_name; }
    public int GetMaxHP() { return character.hp_stat; }
    public int GetMaxSP() { return character.sp_stat; }


    // Getters for being attacked (checked by EnemyRuntime)
    public bool isDefending() {
        return defending;
    }



    // STAT GETTERS
    public int GetSpeed() { return speed; }
    public int GetAttack() { return attack; }
    public int GetLevel() { return level; }

    public int EXP() { return exp; }
    public int EXPNeeded() { return expMax; }
    public bool InAnimation() { return inAnimation; }
    
    // Returns Total Attack stat
    public int GetMight() { return attack; }
    public int GetDefense() { return defense; }
    public int GetMagic() { return magic; }
    public int GetMagicDefense() { return magicDefense; }
    public int GetDexterity() { return dexterity; }
    public int GetEvasion() { return evasion; }
    public int GetCrit() { return crit; }
    public int GetCon() { return con; }
    public int GetTotalSpeed() {
        return speed + (con - weapon.weight);
    }

    /* When the player is scrolling through equiptables, they should be able to view stat changes
     * However, we don't want to actually change the stats of the current character in doing so
     * This function clones the character's stats into a special class created to view stat changes
     */
    public StatsViewer ViewStats() {
        StatsViewer viewer = new StatsViewer();
        viewer.attack = character.attack_stat;
        viewer.defense = character.defense_stat;
        viewer.magic = character.magic_stat;
        viewer.magicDefense = character.magic_defense_stat;
        viewer.dexterity = character.dexterity_stat;
        viewer.speed = character.speed_stat;
        viewer.con = character.con_stat;
        viewer.crit = character.crit_stat;
        viewer.evasion = character.evasion_stat;

        viewer.weapon = weapon;
        viewer.headArmor = headArmor;
        viewer.bodyArmor = bodyArmor;
        viewer.armArmor = armArmor;
        viewer.accessory = accessory;
        return viewer;
    } // ViewStats()

    // Update stats whenever equiping a new toy
    void UpdateStats() {
        attack = character.attack_stat + weapon.strength;
        defense = character.defense_stat + headArmor.defenseUp + bodyArmor.defenseUp + armArmor.defenseUp + accessory.defenseUp; 
        speed = character.speed_stat + headArmor.speedUp + bodyArmor.speedUp + armArmor.speedUp + accessory.speedUp;
        magic = character.magic_stat + weapon.magicStrength;
        magicDefense = character.magic_defense_stat + headArmor.magicDefenseUp + bodyArmor.magicDefenseUp + armArmor.magicDefenseUp + accessory.magicDefenseUp;
        dexterity = character.dexterity_stat + headArmor.dexterityUp + bodyArmor.dexterityUp + armArmor.dexterityUp + accessory.dexterityUp;
        evasion = character.evasion_stat + headArmor.evasionUp + bodyArmor.evasionUp + armArmor.evasionUp + accessory.evasionUp;
        crit = character.crit_stat + headArmor.critUp + bodyArmor.critUp + armArmor.critUp + accessory.critUp;
        con = character.con_stat;
    } // UpdateStats()
}
