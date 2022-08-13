using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// Manager class for general battles (random encounters, staged fights)
// For specific changes due to boss battles, see BossBattle.cs 
public class BattleManager : MonoBehaviour {
    // Controls State of Battle
    protected enum Battle_States {
        WAIT,
        SELECT_COMMAND,
        INITIATE_BREAK,
        BREAK,
        CHAIN_BREAK,
        BREAK_REMAIN,
        VICTORY,
        DIALOGUE
    }

    protected Battle_States battleState;
    protected int chain;
    protected int expGain;

    public List<GameObject> party = new List<GameObject> { };
    public Enemy[] enemiesInit;
    public List<int> levelsInit = new List<int>();
    public List<GameObject> enemies = new List<GameObject> { };

    // List of available characters that are ready to attack
    public List<GameObject> attack_ready = new List<GameObject> { };

    // bools that dictate if a character or enemy is attacking
    protected bool char_attacking;
    protected bool enemy_attacking;

    // Controls the stage in input commands
    protected int input_stage = 1;

    // Index of Enemy being attacked
    protected int enemy_index = 0;

    public bool ableToFlee = true;

    // Lists dictating order of move when two Game Objects are moving at the same time
    protected List<GameObject> moving_objects = new List<GameObject> { };
    protected List<GameObject> targets = new List<GameObject> { };

    // Gameobject for controlling the character who gets a break
    GameObject lastCharacterMoved;

    // Bool to make sure only one object moves at one time
    protected bool someone_moving = false;

    // Reference controls Menu
    public BattleMenuManager menuManager;

    public GameObject input_box;
    public GameObject eventSystem;

    public GameObject canvas;

    public Party gameParty;

    // public const int MAX_MOVE_GAUGE = 155366;
    public int MAX_MOVE_GAUGE = 155366 / 120;

    // Determins which type of action a character is using
    protected enum Action_Types {
        ATTACK,
        SKILL,
        COMBO,
        DEFEND,
        ITEM,
        FLEE
    }

    protected Action_Types action;

    protected int action_index;

    private float fixedDeltaTime;

    // Declare this object as a Singleton
    protected static BattleManager instance;

    public static BattleManager Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<BattleManager>();
            }
            return instance;
        }
    }

    // Destroy any other instances of BattleManager
    protected void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
        fixedDeltaTime = Time.fixedDeltaTime;
    } // Awake()

 
    // Start is called before the first frame update
    // Initializes variables at beginning of scene
    protected virtual void Start() {
        MainVCam.Instance.EnterBattle();
        CameraController.Instance.EnterBattle();

        chain = 0;
        expGain = 0;
        input_box.SetActive(false);
        eventSystem.SetActive(false);
        battleState = Battle_States.WAIT;

        // Set up party buttons, tags, move guages
        for(int i = 0; i < Party.Instance.current_party.Count; ++i) {
            party.Add(Party.Instance.current_party[i]);
            
            party[i].transform.position = new Vector2(3f + (i * 0.8f), 1f - (1.4f * i));
            party[i].GetComponent<CharacterRuntime>().ResetMoveGauge();
            
            Vector3 tag_spawn_pos = new Vector3(-100, 350 - (125 * i));
            party[i].GetComponent<CharacterRuntime>().SetCharTag(TagUI.Spawn(tag_spawn_pos, party[i], canvas));
            party[i].GetComponent<CharacterRuntime>().SetButton(ButtonWrapper.Spawn(new Vector3(0, 0, 0), party[i]));

            if(i > 0) {
                party[i].GetComponent<CharacterRuntime>().ButtonSetRightAndUp(party[i - 1].GetComponent<CharacterRuntime>());
                party[i - 1].GetComponent<CharacterRuntime>().ButtonSetLeftAndDown(party[i].GetComponent<CharacterRuntime>());
            }
        }

        enemiesInit = BattleSceneManager.Instance.GetEnemiesInit();
        levelsInit = BattleSceneManager.Instance.GetLevelsInit();
        ableToFlee = BattleSceneManager.Instance.ableToFlee;

        // Spawn enemies
        if (BattleSceneManager.Instance.GetEnemiesInit().Length == 1) {
            enemies.Add(EnemyRuntime.Spawn(new Vector3(-3.5f, 0.5f), enemiesInit[0], levelsInit[0]).gameObject);
        } else if (BattleSceneManager.Instance.GetEnemiesInit().Length == 2) {
            enemies.Add(EnemyRuntime.Spawn(new Vector3(-2.5f, 1.75f), enemiesInit[0], levelsInit[0]).gameObject);
            enemies.Add(EnemyRuntime.Spawn(new Vector3(-5f, -0.25f), enemiesInit[1], levelsInit[1]).gameObject);
        } else if (BattleSceneManager.Instance.GetEnemiesInit().Length == 3) {
            enemies.Add(EnemyRuntime.Spawn(new Vector3(-0.9f, 1.75f), enemiesInit[0], levelsInit[0]).gameObject);
            enemies.Add(EnemyRuntime.Spawn(new Vector3(-2.9f, -0.25f), enemiesInit[1], levelsInit[1]).gameObject);
            enemies.Add(EnemyRuntime.Spawn(new Vector3(-4.75f, 1f), enemiesInit[2], levelsInit[2]).gameObject);
        }

        // Set up enemy buttons, tags, move guages
        for (int i = 0; i < enemies.Count; ++i) {
            enemies[i].GetComponent<EnemyRuntime>().ResetMoveGauge();
            EnemyTag eTag = EnemyTag.Spawn(new Vector3(0, 0, 0), enemies[i]);
            Debug.Log(eTag);
            enemies[i].GetComponent<EnemyRuntime>().SetEnemyTag(eTag);
            enemies[i].GetComponent<EnemyRuntime>().SetButton(ButtonWrapper.Spawn(new Vector3(0, 0, 0), enemies[i]));
            if (i > 0) {
                enemies[i].GetComponent<EnemyRuntime>().ButtonSetRightAndUp(enemies[i - 1].GetComponent<EnemyRuntime>());
                enemies[i - 1].GetComponent<EnemyRuntime>().ButtonSetLeftAndDown(enemies[i].GetComponent<EnemyRuntime>());
            }
        }
    } // Start()


    // Update is called once per frame
    // This function is terribly written -- Needs to be edited
    // ONE FUNCTION SHOULD NOT BE ~250 LINES
    protected void Update() { // overridable
        // For the most part, Time.timeScale is set to 1
        // However when breaks are initiated, there is a slow down effect and Time.timeScale < 1
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        
        if(battleState == Battle_States.VICTORY) {
            // Continue updating move guages during victory
            for (int i = 0; i < party.Count; ++i) {
                party[i].GetComponent<CharacterRuntime>().AdjustMoveGauge();
            }
        } else if (battleState == Battle_States.INITIATE_BREAK) { // WAITING_FOR_BREAK?
            // Block off input while break is being initiated
            input_box.SetActive(false);
            eventSystem.SetActive(false);
        } else if(battleState == Battle_States.BREAK_REMAIN) {
            // One frame state to start break -- this should really be called INITIATE_BREAK
            if(someone_moving == false && !attack_ready[0].GetComponent<CharacterRuntime>().InAnimation()) {
                battleState = Battle_States.BREAK;
                menuManager.ReturnToMain();
            }
        } else if (battleState == Battle_States.BREAK) {
            // Because of how the animator is set up:
            // In some cases, the selecting a move while the same character is still moving
            // can bring the battle to a complete stop -- keeping the animator stuck in idle
            // This has (hopefully) been fixed, but as a precaution: wait for the previous animation to end
            if (someone_moving == true) {
                return;
            }
           
            // Go into BREAK when the battleState is BREAK
            menuManager.SetCharacterReference(attack_ready[0].GetComponent<CharacterRuntime>());
            
            // Force input ability because the previous turn can close it
            input_box.SetActive(true);
            eventSystem.SetActive(true);

            // Enemy death check -- move to separate function
            for (int i = 0; i < enemies.Count; ++i) {
                if (!someone_moving && enemies[i].GetComponent<EnemyRuntime>().hp <= 0) {
                    GameObject dyingEnemy = enemies[i];
                    enemies.RemoveAt(i);
                    expGain += dyingEnemy.GetComponent<EnemyRuntime>().GetExp();
                    if (enemies.Count == 0)
                    {
                        battleState = Battle_States.VICTORY;
                        // BattleSceneManager.Instance.EndBattle();
                        targets.Clear();
                        moving_objects.Clear();
                        VictoryScreen.Spawn(expGain, 0, canvas);
                    }
                    else
                    {
                        for (int j = 0; j < targets.Count; ++j)
                        {
                            if (targets[j] == dyingEnemy) {
                                targets[j] = enemies[0];
                            }
                        }
                        for (int j = 0; j < moving_objects.Count; ++j)
                        {
                            if (moving_objects[j] == dyingEnemy)
                            {
                                moving_objects.RemoveAt(j);
                                targets.RemoveAt(j);
                            }
                        }
                    }
                    menuManager.RemoveTarget(dyingEnemy);
                    Destroy(dyingEnemy.gameObject);
                    for (int j = 1; j < enemies.Count; ++j) {
                        enemies[j].GetComponent<EnemyRuntime>().ButtonSetRightAndUp(enemies[j - 1].GetComponent<EnemyRuntime>());
                        enemies[j - 1].GetComponent<EnemyRuntime>().ButtonSetLeftAndDown(enemies[j].GetComponent<EnemyRuntime>());
                    }
                    break;
                }
            }
        } else { // WAIT, SELECT_COMMAND -- move this to separate function
            
            // Updates wait time of characters
            // Changes battleState to SELECT_COMMAND when a character's wait time is below or equal to 0
            for (int i = 0; i < party.Count; ++i) {
                party[i].GetComponent<CharacterRuntime>().AdjustMoveGauge();
                if (party[i].GetComponent<CharacterRuntime>().GetMoveGauge() >= MAX_MOVE_GAUGE && party[i].GetComponent<CharacterRuntime>().isAttackReady() == false) {
                    party[i].GetComponent<CharacterRuntime>().AttackReady();
                    battleState = Battle_States.SELECT_COMMAND;
                    attack_ready.Add(party[i]);
                }
            }

            // Sanity Check (?) - still don't understand why this is here (05/03/22)
            // Similarly, if attack_ready has a size greater than 0, sets battleState to SELECT_COMMAND
            if(attack_ready.Count > 0) {
                battleState = Battle_States.SELECT_COMMAND;
            }

            // Loops through enemies and adjusts their information
            for (int i = 0; i < enemies.Count; ++i) {
                enemies[i].GetComponent<EnemyRuntime>().AdjustMoveGauge();
                
                // Initiates break if enemy is still alive and has no shields
                if (enemies[i].GetComponent<EnemyRuntime>().GetShields() <= 0 && enemies[i].GetComponent<EnemyRuntime>().hp > 0) {
                    battleState = Battle_States.INITIATE_BREAK;
                }
                
                // Makes enemy move when its wait time is below or equal to 0
                if (enemies[i].GetComponent<EnemyRuntime>().GetMoveGauge() >= MAX_MOVE_GAUGE && enemies[i].GetComponent<EnemyRuntime>().isAttackReady() == false) {
                    enemies[i].GetComponent<EnemyRuntime>().AttackReady();
                    Debug.Log("READY TO ATTACK: " + enemies[i]);
                    moving_objects.Add(enemies[i]);
                    int rand = UnityEngine.Random.Range(0, party.Count);
                    targets.Add(party[rand]);
                }
                
                // Enemy death check -- move to separate function
                // This could also be called somewhere else??
                if (!someone_moving && enemies[i].GetComponent<EnemyRuntime>().hp <= 0) {
                    GameObject dyingEnemy = enemies[i];
                    expGain += dyingEnemy.GetComponent<EnemyRuntime>().GetExp();
                    enemies.RemoveAt(i);
                    
                    if (enemies.Count == 0) {
                        battleState = Battle_States.VICTORY;
                        VictoryScreen.Spawn(expGain, 0, canvas);
                        targets.Clear();
                        moving_objects.Clear();
                    } else {
                        for (int j = 0; j < targets.Count; ++j) {
                            if (targets[j] == dyingEnemy) {
                                targets[j] = enemies[0];
                            }
                        }
                        
                        for (int j = 0; j < moving_objects.Count; ++j) {
                            if (moving_objects[j] == dyingEnemy) {
                                moving_objects.RemoveAt(j);
                                targets.RemoveAt(j);
                            }
                        }
                    }
                    
                    menuManager.RemoveTarget(dyingEnemy);
                    Destroy(dyingEnemy.gameObject);
                    for (int j = 1; j < enemies.Count; ++j) {
                        enemies[j].GetComponent<EnemyRuntime>().ButtonSetRightAndUp(enemies[j - 1].GetComponent<EnemyRuntime>());
                        enemies[j - 1].GetComponent<EnemyRuntime>().ButtonSetLeftAndDown(enemies[j].GetComponent<EnemyRuntime>());
                    }
                    Debug.Log("Enemies Left " + enemies.Count);
                    break;
                }
            }
            
            // Displays Select Menu when battleState is SELECT_COMMAND
            if (battleState == Battle_States.SELECT_COMMAND && battleState != Battle_States.VICTORY) {
                menuManager.SetCharacterReference(attack_ready[0].GetComponent<CharacterRuntime>());
                
                input_box.SetActive(true);
                eventSystem.SetActive(true);
            }

            // When there is one or more moving objects and no one is moving, the first object in the list will take their turn
            if (moving_objects.Count > 0 && !someone_moving && battleState != Battle_States.VICTORY) {
                // Character Move - move this to a separate function
                if (moving_objects[0].GetComponent<CharacterRuntime>() != null) {
                    // Store this character in case of break
                    lastCharacterMoved = moving_objects[0];
                    moving_objects[0].GetComponent<CharacterRuntime>().StartTurn();
                    
                    // Sanity check: if the enemy target has died, it can no longer be accessed
                    // Default to enemies[0]
                    if(targets[0] == null) {
                        targets[0] = enemies[0];
                    }
                    
                    if (action == Action_Types.ATTACK) {
                        someone_moving = true;
                        moving_objects[0].GetComponent<CharacterRuntime>().Attack(targets[0]);
                    } else if (action == Action_Types.DEFEND) {
                        someone_moving = true;
                        moving_objects[0].GetComponent<CharacterRuntime>().Defend();
                    } else if (action == Action_Types.SKILL) {
                        someone_moving = true;
                        moving_objects[0].GetComponent<CharacterRuntime>().UseSkill(action_index);

                    } else if (action == Action_Types.ITEM) {
                        Debug.Log("ACTION INDEX: " + action_index);
                        Party.Instance.inventory.UseItem(action_index);

                        // CHANGE THIS LATER - wait, why?? (05/03/22)
                        moving_objects[0].GetComponent<CharacterRuntime>().ResetMoveGauge();
                        moving_objects[0].GetComponent<CharacterRuntime>().FinishAttack();
                        // CHANGE THIS LATER
                    } else if (action == Action_Types.FLEE) {
                        // Flee calculation - move this to a separate function
                        int averageSpeed = 0;
                        int averageLevel = 0;
                        foreach (GameObject characterObject in party) {
                            averageSpeed += characterObject.GetComponent<CharacterRuntime>().GetSpeed();
                            averageLevel += characterObject.GetComponent<CharacterRuntime>().GetLevel();
                        }
                        averageSpeed /= party.Count;
                        averageLevel /= party.Count;

                        int averageSpeedEnemies = 0;
                        int averageLevelEnemies = 0;
                        foreach (GameObject enemyObject in enemies) {
                            averageSpeedEnemies += enemyObject.GetComponent<EnemyRuntime>().GetSpeed();
                            averageLevelEnemies += enemyObject.GetComponent<EnemyRuntime>().GetLevel();
                        }
                        averageSpeedEnemies /= enemies.Count;
                        averageLevelEnemies /= enemies.Count;

                        float odds = (40) * (averageSpeed/averageSpeedEnemies) * (averageLevel/averageLevelEnemies);

                        float random = UnityEngine.Random.Range(0, 100);
                        Debug.Log(odds);
                        Debug.Log(random);
                        moving_objects[0].GetComponent<CharacterRuntime>().ResetMoveGauge();
                        moving_objects[0].GetComponent<CharacterRuntime>().FinishAttack();
                        
                        if(random <= odds) {
                            UnityEvent endEvent = new UnityEvent();
                            endEvent.AddListener(Flee);
                            BattleAlert.Spawn("The Party Fled", canvas, endEvent);
                            battleState = Battle_States.VICTORY;
                            /* moving_objects.Clear();
                            targets.Clear();
                            return; */
                        } else {
                            BattleAlert.Spawn("Couldn't Escape", canvas);
                        }
                    }

                    // Begins initiating BREAK when enemy shields are at 0
                    if(targets.Count > 0 && targets[0].GetComponent<EnemyRuntime>() != null) {
                        if (targets[0].GetComponent<EnemyRuntime>().GetShields() <= 0 && targets[0].GetComponent<EnemyRuntime>().hp > 0) {
                            battleState = Battle_States.INITIATE_BREAK;
                        }
                    }
                } else if (moving_objects[0].GetComponent<EnemyRuntime>() != null) {
                    // Enemy move - move this to separate function
                    someone_moving = true;
                    moving_objects[0].GetComponent<EnemyRuntime>().Attack(targets[0]);
                    moving_objects[0].GetComponent<EnemyRuntime>().SetTarget(targets[0]);
                    Debug.Log(targets[0].GetComponent<CharacterRuntime>().name + targets[0].GetComponent<CharacterRuntime>().hp);
                    moving_objects[0].GetComponent<EnemyRuntime>().ResetMoveGauge();
                    moving_objects[0].GetComponent<EnemyRuntime>().FinishAttack();
                    
                }

                if(moving_objects.Count > 0) {
                    moving_objects.RemoveAt(0);
                    targets.RemoveAt(0);
                }
            }
        }
    } // Update()

    // Sets someone_moving to false, allowing another object to move
    // Should be invoked in EndAnimation function in EnemyRuntime and CharacterRuntime scripts
    public void allowMovement() {
        someone_moving = false;
    }

    public bool IsSomeoneMoving() { return someone_moving; }

    // Set up a character move
    public void SetMoving() {
        if(battleState == Battle_States.BREAK) {
            if(action == Action_Types.ATTACK) {
                // bool alreadyBroken = enemies[enemy_index].GetComponent<EnemyRuntime>().Broke();
                attack_ready[0].GetComponent<CharacterRuntime>().Attack(enemies[enemy_index]);
                
                /* CHAIN BREAK IS NOT IMPLEMENTED YET
                if (!alreadyBroken && enemies[enemy_index].GetComponent<EnemyRuntime>().GetShields() == 0)
                {
                    battleState = Battle_States.CHAIN_BREAK;
                    Debug.Log("HEWUIQIHFLJK");
                }*/

                someone_moving = true;
            } else if (action == Action_Types.DEFEND) {
                someone_moving = true;
                attack_ready[0].GetComponent<CharacterRuntime>().Defend();
                EndBreak();
            }
            //attack_ready[0].GetComponent<CharacterRuntime>().ResetMoveGauge();
            //attack_ready[0].GetComponent<CharacterRuntime>().FinishAttack();
            
        } else {
            // Debug.Log("Size of AttackReady: " + attack_ready.Count);
            moving_objects.Add(attack_ready[0]);
            Debug.Log("Adding enemy index: " + enemy_index);
            if(enemies.Count > 0) {
                targets.Add(enemies[enemy_index]);
            }
            Debug.Log("First target: " + targets[0]);
            //Debug.Log("First moving object: " + moving_objects[0]);
            Debug.Log("Recently added target: " + targets[targets.Count -1]);
            attack_ready.RemoveAt(0);
            battleState = Battle_States.WAIT;
        }

        input_box.SetActive(false);
        eventSystem.SetActive(false);
        input_stage = 1;
    } // SetMoving()

    // SetMoving for skills and items
    public void SetMoving(int index) {
        // this is a bad system btw, what happens if two skill moves are in the back log?
        // maybe store these in a list(?)
        action_index = index;
        Debug.Log("Size of AttackReady: " + attack_ready.Count);
        moving_objects.Add(attack_ready[0]);
        Debug.Log("Dance " + attack_ready[0] + "! But only after moving objects has you");
        //someone_moving = true;
        //targets.Add(enemies[enemy_index]);
        attack_ready.RemoveAt(0);
        battleState = Battle_States.WAIT;
        
        // Maybe have these two in a function - SetInput(bool state)
        input_box.SetActive(false);
        eventSystem.SetActive(false);
        
        input_stage = 1; // reset input stage (maybe function for this?)
    }

    // SetMoving for skills and items in the case of a break
    // We will later use character tags for this type of check, making this function useless (in the future) -- this will be mandatory for the case "Both"
    void SetMoving(int index, Use_Type useType) {
        if (useType == Use_Type.Party || useType == Use_Type.Party_All) {
            if(action == Action_Types.ITEM) {
                Party.Instance.inventory.UseItem(index);
            }
            else if (action == Action_Types.SKILL) {
                attack_ready[0].GetComponent<CharacterRuntime>().UseSkill(index);
            }
            someone_moving = true;
            EndBreak();
        } else {
            if (action == Action_Types.ITEM) {
                Party.Instance.inventory.UseItem(action_index);
            } else if (action == Action_Types.SKILL) {
                Debug.Log("Skill Index:" + index);
                int skillIndex = index;
                attack_ready[0].GetComponent<CharacterRuntime>().UseSkill(skillIndex);
            }
            someone_moving = true;
        }

        input_box.SetActive(false);
        eventSystem.SetActive(false);
    }

    // SET COMMAND TYPE
    public void SetAttack(int index) {
        action = Action_Types.ATTACK;
        enemy_index = index;
        Debug.Log("Enemy index: " + index);
        SetMoving();
    }

    // Setup information for party member to use a skill
    public void SetSkill(int index, int target_index, Use_Type useType) {
        action = Action_Types.SKILL;
        Debug.Log(target_index);

        if(battleState == Battle_States.BREAK) {
            enemy_index = target_index;
            SetMoving(index, useType);
        } else {
            if (useType == Use_Type.Party) {
                targets.Add(party[target_index]);
            } else {
                targets.Add(enemies[target_index]);
            }
            SetMoving(index);
        }
    }

    public void SetCombo() {
        action = Action_Types.COMBO;
    }
    public void SetDefend() {
        action = Action_Types.DEFEND;
        SetMoving();
    }
    
    public void SetItem(int index, int targetIndex, Use_Type useType) {
        action = Action_Types.ITEM;
        Debug.Log(targetIndex);
        if (battleState == Battle_States.BREAK) {
            enemy_index = targetIndex;
            SetMoving(index, useType);
        } else {
            if (useType == Use_Type.Party) {
                targets.Add(party[targetIndex]);
            } else {
                targets.Add(enemies[targetIndex]);
            }
            SetMoving(index);
        }
    }

    public void SetFlee() {
        action = Action_Types.FLEE;
        SetMoving();
    }

    void Flee() {
        while(enemies.Count > 0) {
            GameObject enemyRemove = enemies[enemies.Count - 1];
            enemies.RemoveAt(enemies.Count - 1);
            Destroy(enemyRemove);
        }

        BattleSceneManager.Instance.EndBattle();
    }

    // Getters for setting up Skill and Item Function List Classes
    public List<GameObject> GetParty() { return party; }
    public List<GameObject> GetEnemies() { return enemies; }
    public GameObject GetTarget() { return targets[0]; }

    // Slows time for cool effect during break
    public void SlowTime() {
        Time.timeScale = 0.25f;
    }

    // Resets time to normal
    public void NormalTime() {
        Time.timeScale = 1f;
    }

    // Returns state of break
    public bool InBreak() { return battleState == Battle_States.BREAK; }
    public bool PreparingForBreak() { return battleState == Battle_States.BREAK_REMAIN; }

    // Sets the battle state to BREAK
    public void SetBreak() {
        ++chain; // chain break not fully implemented
        if (battleState != Battle_States.BREAK && battleState != Battle_States.BREAK_REMAIN) {
            //battleState = Battle_States.BREAK;
            //chain = 1;
            Debug.Log("IN BREAK, ATTACK READY: " + attack_ready.Count);
            
            attack_ready.Insert(0, lastCharacterMoved);
            battleState = Battle_States.BREAK_REMAIN;
        } else {
            battleState = Battle_States.BREAK_REMAIN;
        }
    }

    // Updates tags at the end of a BREAK
    public void EndBreak() {
        input_box.SetActive(false);
        eventSystem.SetActive(false);
        if (battleState != Battle_States.BREAK_REMAIN) {
            battleState = Battle_States.WAIT;
            // Again, separate function
            foreach (GameObject enemy in enemies) {
                if (enemy.GetComponent<EnemyRuntime>().Broke()) {
                    enemy.GetComponent<EnemyRuntime>().RestoreShields();
                    enemy.GetComponent<EnemyRuntime>().ResetMoveGauge();
                    enemy.GetComponent<EnemyRuntime>().UpdateTag(true);
                    attack_ready.Remove(enemy);
                    for (int j = 0; j < moving_objects.Count; ++j) {
                        if (moving_objects[j] == enemy) {
                            moving_objects.RemoveAt(j);
                            targets.RemoveAt(j);
                        }
                    }
                }
            }

            //enemies[enemy_index].GetComponent<EnemyRuntime>().RestoreShields();
            //enemies[enemy_index].GetComponent<EnemyRuntime>().calculateWaitTime();
            //attack_ready[0].GetComponent<CharacterRuntime>().ResetMoveGauge();
            //attack_ready[0].GetComponent<CharacterRuntime>().FinishAttack();

            Debug.Log(attack_ready[0]);
            attack_ready.RemoveAt(0);
            Debug.Log("ATTACK READY: " + attack_ready.Count);
            chain = 0;
            Debug.Log("Is someone moving?");
            Debug.Log(someone_moving);
            if (attack_ready.Count > 0) {
                Debug.Log(attack_ready[0]);
                battleState = Battle_States.SELECT_COMMAND;
            }
            allowMovement();
        }
    }

    public int GetTargetIndexDuringBreak() { return enemy_index; }
    public void SetEXPGain(int gain) {
        expGain = gain;
    }
}
