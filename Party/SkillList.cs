using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SkillList", menuName = "SkillList")]
public class SkillList : ScriptableObject
{
    //private BattleManager battleManager;

    private int breakPower;
    private int repairPower;
    Affinity affinity;

    private List<GameObject> randomTargetOrder = new List<GameObject>();
    public CharacterRuntime character_ref;
    private delegate void SkillUse();
    private SkillUse skillUse;

    //Sets up skill List for use at Runtime
    /*public void Setup(BattleManager battleManagerInput, CharacterRuntime character)
    {
        //battleManager = battleManagerInput;
        character_ref = character;
    }*/
    public void Setup(CharacterRuntime character)
    {
        //battleManager = battleManagerInput;
        character_ref = character;
    }

    //Sets breakPower
    public void SetPower(int breakP, int repairP, Affinity affinityInput)
    {
        breakPower = breakP;
        repairPower = repairP;
        affinity = affinityInput;
    }

    //Sets repairPower
    public void SetRepairPower(int power)
    {
        repairPower = power;
    }

    public void CleanUp()
    {
        breakPower = 0;
        repairPower = 0;
        affinity = Affinity.Simple;
        randomTargetOrder.RemoveAll((GameObject) => { return true; });
    }

    //Continues a skill that needs several iterations
    public void ContinueSkill()
    {
        skillUse();
    }

    //Adds a rando target to randomTarget Order
    public void AddRandomTarget(GameObject randTarget)
    {
        randomTargetOrder.Add(randTarget);
    }

    //Gets the random targets
    public List<GameObject> GetRandTargets()
    {
        return randomTargetOrder;
    }


    // Nico
    // Starts Lightning Dance Skill
    public void LightningDanceStarter()
    {
        randomTargetOrder = new List<GameObject>();
        for(int i = 0; i < 3; ++i)
        {
            randomTargetOrder.Add(BattleManager.Instance.GetEnemies()[Random.Range(0, BattleManager.Instance.GetEnemies().Count)]);
        }
        Debug.Log("RAND TARGETS: " + randomTargetOrder.Count);
        int damage;
        if (randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().GetSpeed() < character_ref.GetSpeed())
        {
            damage = character_ref.GetAttack() * (character_ref.GetSpeed() - randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().GetSpeed());
        }
        else
        {
            damage = character_ref.GetAttack();
        }
        damage /= randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().defense / 2;
        damage = Mathf.RoundToInt(Random.Range(damage - 5, damage + 5));
        randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().hp -= damage;
        character_ref.damage_out = damage;
        if (randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().hasWeakness(affinity))
        {
            randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().LoseShields(breakPower);
        }
        //randomTargetOrder.RemoveAt(randomTargetOrder.Count);
        //INSERT HELPER -- goodnight
        //SlashEffectManager.Spawn(randomTargetOrder[0], damage);

        character_ref.animator.SetBool("LtgDance", true);
        skillUse = LightningDance;
    }

    // Nico
    // Continues Lightning Dance Skill
    public void LightningDance()
    {
        Debug.Log("We got this much: " + randomTargetOrder.Count);
        randomTargetOrder.RemoveAt(randomTargetOrder.Count - 1);
        if(randomTargetOrder.Count != 0) {
            Debug.Log("HP OF ENEMY BEING ATTACKED: " + randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().hp);
        }
        
        if(randomTargetOrder.Count != 0 && (randomTargetOrder[randomTargetOrder.Count - 1].gameObject == null || randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().hp <= 0)) {
            Debug.Log("It's NULL!");
            randomTargetOrder[randomTargetOrder.Count - 1] = BattleManager.Instance.GetEnemies()[Random.Range(0, BattleManager.Instance.GetEnemies().Count)];
        }
        Debug.Log("We got this much left: " + randomTargetOrder.Count);
        if (randomTargetOrder.Count != 0)
        {
            Debug.Log("O GOD O GOD O GOD");
            Debug.Log("Is someone moving?");
            Debug.Log(BattleManager.Instance.IsSomeoneMoving());
            Debug.Log("Are we in break?");
            Debug.Log(BattleManager.Instance.InBreak());
            int damage;
            if (randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().GetSpeed() < character_ref.GetSpeed()) {
                damage = character_ref.GetAttack() * (character_ref.GetSpeed() - randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().GetSpeed());
            } else {
                damage = character_ref.GetAttack();
            }
            
            /*if (randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().defense < character_ref.GetAttack()) {
                
            } else {
                damage /= 
            }*/

            damage /= randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().defense / 2;

            damage = Mathf.RoundToInt(Random.Range(damage - 5, damage + 5));
            randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().hp -= damage;
            if(randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().hasWeakness(affinity))
            {
                randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().LoseShields(breakPower);
            }

            //StartCoroutine(pause());
            //yield return new WaitForSecondsRealtime(0.2f);
            //randomTargetOrder.RemoveAt(randomTargetOrder.Count);
            //INSERT HELPER -- goodnight
            //SlashEffectManager.Spawn(randomTargetOrder[0], damage);
            if ((randomTargetOrder.Count == 1 && BattleManager.Instance.InBreak()) || (randomTargetOrder.Count == 1 && randomTargetOrder[randomTargetOrder.Count - 1].GetComponent<EnemyRuntime>().hp < 0)) // 
            {
                SlashEffectManager.Spawn(randomTargetOrder[randomTargetOrder.Count - 1], character_ref, damage, false);
                skillUse = null;
                //character_ref.set_no_one_attacking.Invoke();
                BattleManager.Instance.allowMovement();
                CleanUp();

                character_ref.StartAnimation("Appear");
                character_ref.EndAnimation("Vanished");
                //battleManager.RemoveTarget();
            }
            else
            {
                SlashEffectManager.Spawn(randomTargetOrder[randomTargetOrder.Count - 1], character_ref, damage, true);
            }
            Debug.Log("We still got this much left: " + randomTargetOrder.Count);

        } else if(randomTargetOrder.Count == 0)
        {
            Debug.Log("IN HERE");
            character_ref.FinishAttack();
            skillUse = null;
            BattleManager.Instance.allowMovement();
            //character_ref.set_no_one_attacking.Invoke();
            CleanUp();
            character_ref.StartAnimation("Appear");
            character_ref.EndAnimation("Vanished");
        }
    }

    // Ace's Skill
    // XSlash: A stronger attack that does double damage during break
    public void XSlash()
    {
        EnemyRuntime enemy;
        int damage;
        if (BattleManager.Instance.InBreak())
        {
            enemy = BattleManager.Instance.GetEnemies()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<EnemyRuntime>();
            int enemy_defense = enemy.defense;
            int damage_upper = Mathf.RoundToInt(character_ref.GetAttack() * (character_ref.GetAttack() + Mathf.Pow(character_ref.GetLevel(), 2) + 1) / enemy_defense);
            int damage_lower = Mathf.RoundToInt(character_ref.GetAttack() * (character_ref.GetAttack() + Mathf.Pow(character_ref.GetLevel(), 2) - 1) / enemy_defense);
            damage = Random.Range(damage_lower, damage_upper) * 2;
        }
        else
        {
            enemy = BattleManager.Instance.GetTarget().GetComponent<EnemyRuntime>();
            int enemy_defense = enemy.defense;
            int damage_upper = Mathf.RoundToInt(character_ref.GetAttack() * (character_ref.GetAttack() + Mathf.Pow(character_ref.GetLevel(), 2) + 1) / enemy_defense);
            int damage_lower = Mathf.RoundToInt(character_ref.GetAttack() * (character_ref.GetAttack() + Mathf.Pow(character_ref.GetLevel(), 2) - 1) / enemy_defense);
            damage = Random.Range(damage_lower, damage_upper);
        }
        enemy.hp -= damage;
        character_ref.SetTarget(enemy.gameObject);
        character_ref.damage_out = damage;
        enemy.LoseShields(breakPower);
        character_ref.animator.SetBool("isAttacking", true);
        Debug.Log("ENEMY SHIELDS: " + enemy.GetComponent<EnemyRuntime>().GetShields());
    }

    public void AttackChains()
    {
        foreach(GameObject enemyObject in BattleManager.Instance.GetEnemies()) {
            EnemyRuntime enemy = enemyObject.GetComponent<EnemyRuntime>();
            
            int enemy_defense = enemy.defense;
            int damage_upper = Mathf.RoundToInt(character_ref.GetAttack() * (character_ref.GetAttack() + character_ref.GetLevel() + 1) / enemy_defense);
            int damage_lower = Mathf.RoundToInt(character_ref.GetAttack() * (character_ref.GetAttack() + character_ref.GetLevel() - 1) / enemy_defense);
            int damage = UnityEngine.Random.Range(damage_lower, damage_upper);
            
            enemy.hp -= damage;
            enemy.LoseShields(breakPower);
            character_ref.damageOut.Enqueue(damage);
            
            character_ref.animator.SetBool("AtkChains", true);
            Debug.Log("ENEMY SHIELDS: " + enemy.GetComponent<EnemyRuntime>().GetShields());
            Buff lowerAttack = new Buff(0.8f, 3, Buff.Buff_Type.ATTACK);
            enemy.ApplyBuff(lowerAttack);
        }
        /*character_ref.SetTarget(enemy.gameObject);
        character_ref.damage_out = damage;
        enemy.LoseShields(breakPower);
        character_ref.animator.SetBool("isAttacking", true);
        Debug.Log("ENEMY SHIELDS: " + enemy.GetComponent<EnemyRuntime>().GetShields());*/
    }
}
