using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Douglas : EnemyRuntime {
    int turnsUntilSwordDance = 1;
    int turnsUntilRespawn;
    bool respawning = false;
    int numHitsLeft;

    public List<GameObject> underlings;
    public Enemy soldier;

    protected override void Start() {
        attack = enemy.attack_stat;
        defense = enemy.defense_stat;
        hp = enemy.hp_stat;
        speed = enemy.speed_stat;
        shields = enemy.shields;
        level = enemy.level;
        attack_break = enemy.attack_break;
        exp = enemy.exp;
        enemy_name = enemy.enemyName;
        broke = false;
        battleManager = BattleManager.Instance;
        buffs = new List<Buff>();
        damageOut = new Queue<int>();
        BattleManager.Instance.SetEXPGain(30);
    }

    public override void Attack(GameObject character) {
        if(!respawning && underlings[0] == null && underlings[1] == null) {
            respawning = true;
            turnsUntilRespawn = 4;
        } else if(respawning && turnsUntilRespawn == 0 && turnsUntilSwordDance != 0) {
            respawning = false;
            BattleManager.Instance.menuManager.Pause();
            underlings[1] = EnemyRuntime.Spawn(new Vector3(-2.75f, -0.35f), soldier, 2).gameObject;
            underlings[0] = EnemyRuntime.Spawn(new Vector3(-0.9f, 1.9f), soldier, 2).gameObject;
            BattleManager.Instance.enemies.Insert(0, underlings[1]);
            BattleManager.Instance.enemies.Insert(0, underlings[0]);

            for (int i = 0; i < underlings.Count; ++i) {
                underlings[i].GetComponent<EnemyRuntime>().ResetMoveGauge();
                EnemyTag eTag = EnemyTag.Spawn(new Vector3(0, 0, 0), underlings[i]);
                underlings[i].GetComponent<EnemyRuntime>().SetEnemyTag(eTag);
                underlings[i].GetComponent<EnemyRuntime>().SetButton(ButtonWrapper.Spawn(new Vector3(0, 0, 0), underlings[i]));
                if (i > 0) {
                    underlings[i].GetComponent<EnemyRuntime>().ButtonSetRightAndUp(underlings[i - 1].GetComponent<EnemyRuntime>());
                    underlings[i - 1].GetComponent<EnemyRuntime>().ButtonSetLeftAndDown(underlings[i].GetComponent<EnemyRuntime>());
                }
            }

            underlings[1].GetComponent<EnemyRuntime>().ButtonSetLeftAndDown(this.GetComponent<EnemyRuntime>());
            ButtonSetRightAndUp(underlings[1].GetComponent<EnemyRuntime>());

            BattleManager.Instance.menuManager.ForceButtonUpdate();
            BattleAlert.Spawn("Douglas called for guards", BattleManager.Instance.canvas);
            FinishAttack();
            BattleManager.Instance.allowMovement();
            return;
        } else if(respawning && turnsUntilSwordDance != 0) {
            --turnsUntilRespawn;
        }

        if(hp < enemy.hp_stat / 2 && turnsUntilSwordDance == 1) {
            ReadyStrongAttack();
            --turnsUntilSwordDance;
            return;
        } else if (hp < enemy.hp_stat / 2 && turnsUntilSwordDance == 0) {
            SwordDanceStarter();
            return;
        } else if(hp < enemy.hp_stat / 2) {
            --turnsUntilSwordDance;
        }

        int attackIdentifier = Random.Range(1, 100);
        if(attackIdentifier >= 70) {
            Sweep();
        } else {
            StandardAttack(character);
        }
    }
    
    public void StandardAttack(GameObject character) {
        int enemy_defense = character.GetComponent<CharacterRuntime>().defense;
        
        int damage_upper = Mathf.RoundToInt(attack * (attack + level + 1) / enemy_defense);
        int damage_lower = Mathf.RoundToInt(attack * (attack + level - 1) / enemy_defense);
        int damage = Random.Range(damage_lower, damage_upper);
        
        if (character.GetComponent<CharacterRuntime>().isDefending()) {
            float damage_buffer = damage / 2f;
            damage = Mathf.RoundToInt(damage_buffer);
        }

        CharacterRuntime enemy_stats = character.GetComponent<CharacterRuntime>();
        enemy_stats.hp -= damage;
        
        damage_out = damage;
        target = character;

        animator.SetBool("isAttacking", true);
    }

    public void Sweep() {
        foreach(GameObject enemyObject in BattleManager.Instance.GetParty()) {
            CharacterRuntime enemy = enemyObject.GetComponent<CharacterRuntime>();
            
            int enemy_defense = enemy.defense;
            int damage_upper = Mathf.RoundToInt(attack * (attack + level + 1) / enemy_defense);
            int damage_lower = Mathf.RoundToInt(attack * (attack + level - 1) / enemy_defense);
            int damage = UnityEngine.Random.Range(damage_lower, damage_upper);
            
            enemy.hp -= damage;
            damageOut.Enqueue(damage);
        }

        BattleAlert.Spawn("Sweep", BattleManager.Instance.canvas);
        animator.SetBool("Sweep", true);
    }

    public void ReadyStrongAttack() {
        BattleAlert.Spawn("Douglas focuses for a strong attack", BattleManager.Instance.canvas);
        FinishAttack();
        BattleManager.Instance.allowMovement();
    }

    // Starts Sword Dance Attack
    public void SwordDanceStarter() {
        numHitsLeft = 3;

        GameObject currentTarget = BattleManager.Instance.GetParty()[Random.Range(0, BattleManager.Instance.GetParty().Count)];
        target = currentTarget;

        int damage_upper = Mathf.RoundToInt(attack * (attack + level * level + 1) / (currentTarget.GetComponent<CharacterRuntime>().GetDefense() / 2));
        int damage_lower = Mathf.RoundToInt(attack * (attack + level * level - 1) / (currentTarget.GetComponent<CharacterRuntime>().GetDefense() / 2));
        int damage = Random.Range(damage_lower, damage_upper);
        
        damage = Mathf.RoundToInt(Random.Range(damage - 5, damage + 5));
        
        currentTarget.GetComponent<CharacterRuntime>().hp -= damage;
        damage_out = damage;
        
        BattleAlert.Spawn("Sword Dance", BattleManager.Instance.canvas);

        animator.SetBool("SwordDance", true);
    }

    // Continues Sword Dance Attack
    public void SwordDance() {   
        --numHitsLeft;
        if (numHitsLeft != 0) {
            GameObject currentTarget = BattleManager.Instance.GetParty()[Random.Range(0, BattleManager.Instance.GetParty().Count)];

            int damage_upper = Mathf.RoundToInt(attack * (attack + level * level + 1) / (currentTarget.GetComponent<CharacterRuntime>().GetDefense() / 2));
            int damage_lower = Mathf.RoundToInt(attack * (attack + level * level - 1) / (currentTarget.GetComponent<CharacterRuntime>().GetDefense() / 2));
            int damage = Random.Range(damage_lower, damage_upper);
            
            damage = Random.Range(damage - 5, damage + 5);
            
            currentTarget.GetComponent<CharacterRuntime>().hp -= damage;
            SlashEffectManager.Spawn(currentTarget, this, damage, true);
        } else {
            FinishAttack();
            BattleManager.Instance.allowMovement();
            turnsUntilSwordDance = Random.Range(4, 6);
        }
    }

    public override void ContinueSkill() {
        SwordDance();
    }

    void StartSlashAnimMultiple() {
        SlashEffectManager.Spawn(target, this, damage_out, true);
    }

    void EndAnimationStillAttacking(string transitionBool) {
        animator.SetBool(transitionBool, false);
    }

    void StartSlashAnimAll() {
        foreach(GameObject enemy in BattleManager.Instance.GetParty()) {
            SlashEffectManager.Spawn(enemy, this, damageOut.Peek(), false);
            damageOut.Dequeue();
        }
    }

    public override void Break() {
        broke = true;
        if(hp < enemy.hp_stat / 2) {
            turnsUntilSwordDance = Random.Range(4, 6);
        }
    }
}
