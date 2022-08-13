using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleMenuManager : MonoBehaviour {
    public GameObject battle_manager_object;
    private BattleManager battle_manager;
    
    // Control button display
    enum Battle_Options {
        MAIN, // Regular options - Attack, Defend, ...
        SKILL, // List of skills
        ITEMS, // List of items
        COMBOS, //!List of available combos - NOT IMPLEMENTED
        ENEMY_SELECT // Next select stage
    }
    Battle_Options option;

    enum Selection_Stage {
        BASE,
        TARGET
    }
    Selection_Stage selectionStage;

    /* x and y of input grid
     * Go top to bottom, left to right
     * | (0, 0) | (0, 1) |
     * | (1, 0) | (1, 1) |
     * | (2, 0) | (2, 1) |
     */
    private int y;
    private int x;

    private int list_start;
    private int list_end;
    private CharacterRuntime character_ref;
    
    public List<Button> buttons;
    public List<Button> target_buttons;

    public Button return_button;
    public Party party;

    // dunno why these are stored separately, but we'll roll with it
    private int skill_index; 
    private int item_index;

    // Start is called before the first frame update
    void Start() {
        option = Battle_Options.MAIN;
        selectionStage = Selection_Stage.BASE;
        list_start = 0;
        list_end = buttons.Count;
        battle_manager = battle_manager_object.GetComponent<BattleManager>();
        SetButtons();
        return_button = buttons[0];
    } // Start()

    // Update is called once per frame
    void Update() {
        GetInput();

        if(option == Battle_Options.SKILL) {
            skill_index = list_start + x + (2 * y);
        }

        if (option == Battle_Options.ITEMS) {
            item_index = list_start + x + (2 * y);
        }
    } // Update()

    // > 100 line function -> consider splitting this up
    // Controls x and y position in grid based on user input
    // Updates grid to correctly 
    void GetInput() {
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            ++y;
            // why not exit early???
            if (y > 2) {
                y = 2;
                if (option == Battle_Options.SKILL && list_end != character_ref.GetSkills().Count) {
                    list_start += 2;
                    list_end += 2;
                    if (list_end > character_ref.GetSkills().Count) {
                        list_end = character_ref.GetSkills().Count;
                    }
                    SetButtons();
                }

                if (option == Battle_Options.ITEMS && list_end != character_ref.GetSkills().Count) {
                    list_start += 2;
                    list_end += 2;
                    if (list_end > Party.Instance.inventory.GetNumItems()) {
                        list_end = Party.Instance.inventory.GetNumItems();
                    }
                    SetButtons();
                }

                /* !COMBOS ARE NOT IMPLEMENTED YET
                if (option == Battle_Options.COMBO && list_end != combos.Count)
                {
                    list_start += 2;
                    list_end += 2;
                    if (list_end > combos.Count)
                    {
                        list_end = combos.Count;
                    }
                }
                */
            }
        }

        // don't see why this can't be "else if," but I'm too scared to change it
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            --y;
            // exit early???
            if(y < 0) {
                y = 0;
                if (option == Battle_Options.SKILL && list_start != 0) {
                    list_start -= 2;
                    list_end -= 2;
                    if(list_end % 2 == 1) {
                        list_end += 1;
                    }
                    SetButtons();
                } else if (option == Battle_Options.ITEMS && list_start != 0) {
                    list_start -= 2;
                    list_end -= 2;
                    if (list_end % 2 == 1) {
                        list_end += 1;
                    }
                    SetButtons();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ++x;
            if (x > 1) {
                x = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            --x;
            if (x < 0) {
                x = 0;
            }
        }

        if (Input.GetKeyDown("x")) {
            if(selectionStage == Selection_Stage.BASE) {
                switch (option) {
                    case Battle_Options.ITEMS:
                        return_button = buttons[3];
                        break;
                    case Battle_Options.SKILL:
                        return_button = buttons[2];
                        break;
                    case Battle_Options.COMBOS:
                        return_button = buttons[4];
                        break;
                }
                
                if (option != Battle_Options.MAIN) {
                    ReturnToMain();
                }
            } else if (selectionStage == Selection_Stage.TARGET) {
                if (option == Battle_Options.MAIN) {
                    ReturnToMain();
                } else if(option == Battle_Options.SKILL) {
                    ReturnToBase(Battle_Options.SKILL);
                } else if(option == Battle_Options.ITEMS) {
                    ReturnToBase(Battle_Options.ITEMS);
                }
            }
        }
    } // GetInput()


    // Initialize buttons -- so descriptive, me of the past
    // Yeah, I'm lazy; that's pretty much what it does ^ 
    void SetButtons() {
        
        buttons[0].onClick.RemoveAllListeners();
        if (option == Battle_Options.MAIN) {
            // SEPARATE!!!
            foreach(GameObject enemy in battle_manager.GetEnemies())
            {
                target_buttons.Add(enemy.GetComponent<EnemyRuntime>().GetButton());
            }

            buttons[0].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText("Attack");
            //buttons[0].GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize = 30;
            buttons[1].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText("Defend");
            //buttons[1].GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize = 30;
            buttons[2].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText("Skill");
            //buttons[2].GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize = 30;
            buttons[3].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText("Item");
            //buttons[3].GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize = 30;
            buttons[4].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText("Combo");
            //buttons[4].GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize = 30;
            buttons[5].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText("Flee");
            //buttons[5].GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize = 30;

            buttons[0].onClick.AddListener(SetEnemySelect);
            buttons[1].onClick.AddListener(PressDefend);
            buttons[2].onClick.AddListener(SetOptionSkill);
            buttons[3].onClick.AddListener(SetOptionItem);
            buttons[5].onClick.AddListener(PressFlee);
        } else if(option == Battle_Options.SKILL) {
            // SEPARATE!!!
            if (list_end > character_ref.GetSkills().Count) {
                list_end = character_ref.GetSkills().Count;
            }
            
            // Sets onclick events to buttons by index
            for(int i = 0; i < list_end - list_start; ++i) {
                //buttons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize = 26;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(character_ref.GetSkills()[list_start + i].short_name);
                int sum = list_start + i;
                if(character_ref.GetSkills()[sum].spCost <= character_ref.sp) {
                    buttons[i].onClick.AddListener(delegate {
                        SelectTarget(character_ref.GetSkills()[sum]);
                    });
                }
            }

            if (list_end - list_start < buttons.Count)
            {

                for (int i = list_end; i < buttons.Count; ++i)
                {
                    buttons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText("");
                    buttons[i].onClick.RemoveAllListeners();
                }
            }
        }
        else if (option == Battle_Options.ITEMS)
        {
            if (list_end > Party.Instance.inventory.GetNumItems())
            {
                //Debug.Log("In here");
                //Debug.Log("Inventory Storage: " + party.inventory.GetNumItems());
                list_end = Party.Instance.inventory.GetNumItems();
            }
            for (int i = 0; i < list_end - list_start; ++i)
            {
                //buttons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize = 26;
                buttons[i].onClick.RemoveAllListeners();
                //Debug.Log((list_start + i) + ": " + party.inventory.GetItem(list_start + i).GetName());
                buttons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(Party.Instance.inventory.GetItem(list_start + i).GetName());
                int sum = list_start + i;
                buttons[i].onClick.AddListener(delegate
                {
                    SelectTarget(Party.Instance.inventory.GetItem(sum));
                });

                /*buttons[i].onClick.AddListener(delegate
                {
                    Test(sum);
                });
                //int sum = list_start + i;*/
                //SelectTarget(party.inventory.GetItem(list_start + i));


            }
            if (list_end - list_start < buttons.Count)
            {
                for (int i = list_end; i < buttons.Count; ++i)
                {
                    buttons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText("");
                    buttons[i].onClick.RemoveAllListeners();
                }
            }
        }
    } // SetButtons()

    // Defend is Pressed
    void PressDefend()
    {
        battle_manager.SetDefend();
        ReturnToMain();
    } // PressDefend()

    // Calls for text of button to change to skill listing
    void SetOptionSkill() {
        option = Battle_Options.SKILL;
        y = 0;
        x = 0;
        buttons[0].Select();
        return_button = buttons[2];
        list_end = 6;
        
        SetButtons();
    } // SetOptionSkill()

    // Calls for text of button to change to items listing
    void SetOptionItem() {
        option = Battle_Options.ITEMS;
        y = 0;
        x = 0;
        buttons[0].Select();
        return_button = buttons[3];
        list_end = 6;
        SetButtons();
    } // SetOptionItem()

    // Flee is clicked - alerts battlemanager to perform flee calculation
    void PressFlee() {
        if(!BattleManager.Instance.ableToFlee) return;
        BattleManager.Instance.SetFlee();
        ReturnToMain();
    } // PressFlee()


    // Change input stage to select enemy
    // Initializes enemy buttons
    void SetEnemySelect() {
        selectionStage = Selection_Stage.TARGET;
        foreach (Button button in target_buttons) {
            button.onClick.AddListener(ReturnToMain);
        }

        if(option == Battle_Options.MAIN) {
            if (target_buttons.Count > 0) {
                target_buttons[0].Select();
            }

            // Adds onclick events to enemy buttons
            for(short i = 0; i < target_buttons.Count; ++i) {
                // C# seems to handle for loop iterators like references
                // To migrate this, we make deep copy of current index to use in button onclick event
                int current_index = i;
                target_buttons[i].onClick.AddListener(delegate {
                    battle_manager.SetAttack(current_index);
                });
            }

            return_button = buttons[0];
        } else if (option == Battle_Options.SKILL) {
            for (int i = 0; i < target_buttons.Count; ++i) {
                int index = skill_index;
                int currentIndex = i;
                target_buttons[i].onClick.AddListener(delegate {
                    battle_manager.SetSkill(index, currentIndex, character_ref.GetSkills()[index].use_type);
                });
            }

            return_button = buttons[x + (2 * y)];
        } else if (option == Battle_Options.ITEMS) {
            Debug.Log("IN HERE! " + target_buttons.Count);
            for (int i = 0; i < target_buttons.Count; ++i) {
                int index = item_index;
                int current_index = i;
                target_buttons[i].onClick.AddListener(delegate {
                    battle_manager.SetItem(index, current_index, Party.Instance.inventory.GetItem(index).GetUseType());
                });
            }

            return_button = buttons[x + (2 * y)];
        }
    } // SetEnemySelect()

    // Select the type of target button
    // Overload for skills
    void SelectTarget(Skill skill) {
        target_buttons.RemoveAll((Button button) => { return true; });
        switch (skill.use_type) {
            case Use_Type.Party:
                foreach(GameObject character in battle_manager.GetParty()) {
                    target_buttons.Add(character.GetComponent<CharacterRuntime>().GetButton());
                }
                character_ref.GetButton().Select();
                break;
            
            case Use_Type.Enemy:
                foreach (GameObject enemy in battle_manager.GetEnemies()) {
                    target_buttons.Add(enemy.GetComponent<EnemyRuntime>().GetButton());
                }
                battle_manager.GetEnemies()[0].GetComponent<EnemyRuntime>().GetButton().Select();
                break;
            
            case Use_Type.Enemy_All:
                foreach (GameObject enemy in battle_manager.GetEnemies()) {
                    target_buttons.Add(enemy.GetComponent<EnemyRuntime>().GetButton());
                }
                battle_manager.GetEnemies()[0].GetComponent<EnemyRuntime>().GetButton().Select();
                break;
        }
        SetEnemySelect();
    } // SelectTarget()

    // Select the type of target button
    // Overload for items
    // Input: item -> item being used ?
    void SelectTarget(ItemWrapper item) {
        target_buttons.RemoveAll((Button button) => { return true; });
        switch (item.GetUseType()) {
            case Use_Type.Party:
                foreach (GameObject character in battle_manager.GetParty()) {
                    target_buttons.Add(character.GetComponent<CharacterRuntime>().GetButton());
                }
                character_ref.GetButton().Select();
                break;

            case Use_Type.Enemy:
                foreach (GameObject enemy in battle_manager.GetEnemies()) {
                    target_buttons.Add(enemy.GetComponent<EnemyRuntime>().GetButton());
                }
                battle_manager.GetEnemies()[0].GetComponent<EnemyRuntime>().GetButton().Select();
                break;
        }
        SetEnemySelect();
    } // SelectTarget()

    /* Returns to Main Button Arrangement
     * -------------------
     * | ATTACK | DEFEND |
     * | SKILL  | ITEM   |
     * | COMBO  | FLEE   |
     * -------------------
     */
    public void ReturnToMain() {
        option = Battle_Options.MAIN;
        selectionStage = Selection_Stage.BASE;
        
        for(int i = 0; i < target_buttons.Count; ++i) {
            target_buttons[i].onClick.RemoveAllListeners();
        }
        target_buttons.RemoveAll((Button button) => { return true; });

        for (int i = 0; i < buttons.Count; ++i) {
            buttons[i].onClick.RemoveAllListeners();
        }

        return_button.Select();
        SetButtons();
    } // ReturnToMain()

    // Go from higher selection stage back to base grid
    // Input: optionInput -> Current battle option being selected
    void ReturnToBase(Battle_Options optionInput = Battle_Options.MAIN) {
        option = optionInput;
        selectionStage = Selection_Stage.BASE;
        
        for (int i = 0; i < target_buttons.Count; ++i) {
            target_buttons[i].onClick.RemoveAllListeners();
        }
        target_buttons.RemoveAll((Button button) => { return true; });

        for (int i = 0; i < buttons.Count; ++i) {
            buttons[i].onClick.RemoveAllListeners();
        }

        return_button.Select();
        SetButtons();
    } // ReturnToBase()

    // Set's current moving character to use as a reference when displaying buttons and inputting commands
    // Input: character -> current moving character (should be BattleManager.Instance.attack_ready[0])
    public void SetCharacterReference(CharacterRuntime character) {
        character_ref = character;
    }

    // Remove a possible target when they are defeated
    // Input: removableTarget -> target (enemy) to be removed
    public void RemoveTarget(GameObject removableTarget) {
        for(int i = 0; i < target_buttons.Count; ++i) {
            // exit early?
            if(target_buttons[i].GetComponentInParent<EnemyRuntime>() == removableTarget.GetComponent<EnemyRuntime>()) {
                target_buttons.RemoveAt(i);
                if(selectionStage == Selection_Stage.TARGET) {
                    foreach (Button button in target_buttons) {
                        button.onClick.RemoveAllListeners();
                    }

                    SetEnemySelect();
                }
            }
        }
    } // RemoveTarget()

    // Force target buttons to update
    /* Since we use indecies to track which enemy is currently being selected:
     * If a player is selecting a target when an enemy dies, the indecies may get screwed up
     * This will causes the played to attack a different enemy than they selected
     *
     * The current solution is to pause input the frame that an enemy dies in and force the buttons to update
     * Keep in mind, we only use indecies until the target is selected. 
     * That might've actually been a decent design choice on my part
     * 
     * This is only a problem in battles w/ > 2 enemies 
     * Thus, the solution is only implemented in the Douglas Boss Battle
     *
     *
     * Another possible solution to this is to ignore null buttons instead of removing them
     * While this may make some situations easier to handle, this would also require some big changes on the battle manager
     * To some that up, I'm lazy and stupid, 
     * I'll keep working with the bad design until it totally breaks or I want to make things more effiecent
     */
    public void ForceButtonUpdate() {
        Button current_priority = target_buttons[0];
        target_buttons.Clear();
        foreach(GameObject enemy in battle_manager.GetEnemies()) {
            target_buttons.Add(enemy.GetComponent<EnemyRuntime>().GetButton());
        }

        for(int i = 0; i < target_buttons.Count; ++i) {
            target_buttons[i].onClick.RemoveAllListeners();
        }

        if(selectionStage == Selection_Stage.TARGET) {
            SetEnemySelect();
            current_priority.Select();
        }
    } // ForceButtonUpdate()

    // Pause input during battle -- see explanation in ForceButtonUpdate()
    public void Pause() {
        if(selectionStage == Selection_Stage.TARGET)
            BattleManager.Instance.eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
    } // Pause()
}
