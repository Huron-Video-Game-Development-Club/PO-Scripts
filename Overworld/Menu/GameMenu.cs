using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public GameObject menuUI;
    private bool menuIsActive;
    private static GameMenu instance;

    public static GameMenu Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameMenu>();
            }
            return instance;
        }
    }

    // Declare this object as a Singleton
    private void Awake() {
        if (instance == null || instance == this) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private List<MenuCharacterTag> cTags;
    public Status statusBox;
    public ItemList itemBox;
    public Equip equipBox;
    public Button defaultButton;
    private int positionSwapIndex;
    public GameObject canvas;
    public List<Button> commands;
    public enum MenuSelectStage {
        WAIT,
        CHARACTER,
    };
    MenuSelectStage selectStage;

    public enum Command {
        Status = 0,
        Item = 1,
        Equip = 2,
        Position = 3,
        Swap = 4,
        InStatus = 5,
    };
    Command command;

    // Start is called before the first frame update
    void Start() {
        positionSwapIndex = -1;
        cTags = new List<MenuCharacterTag>();
        for(int i = 0; i < Party.Instance.current_party.Count; ++i) {
            Debug.Log("SPAWING");
            cTags.Add(MenuCharacterTag.Spawn(i, canvas));
            if(cTags.Count > 1) {
                Navigation previousNav = new Navigation();
                previousNav.mode = Navigation.Mode.Explicit;
                
                previousNav.selectOnUp = cTags[i - 1].GetComponent<Button>().navigation.selectOnUp;
                previousNav.selectOnDown = cTags[i].GetComponent<Button>();
                
                cTags[i - 1].gameObject.GetComponent<Button>().navigation = previousNav;

                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                
                newNav.selectOnUp = cTags[i - 1].GetComponent<Button>();
                cTags[i].gameObject.GetComponent<Button>().navigation = newNav;
            }
        }
        Deactivate();
    }

    // Update is called once per frame
    void Update() {
        switch(command) {
            case Command.InStatus:
                if(Input.GetKeyDown(KeyCode.LeftArrow)) statusBox.ShiftLeft();
                if(Input.GetKeyDown(KeyCode.RightArrow)) statusBox.ShiftRight();
                break;
            case Command.Item:
                break;
            case Command.Equip:
                break;
            case Command.Position:
                break;
            case Command.Swap:
                break;
        }
    }
    public void Config() { } // Config
    public void Save() { } // Save
    public void Status(int input) {
        /*foreach(Button button in commands) {
            button.gameObject.SetActive(false);
        }*/
        foreach(MenuCharacterTag tag in cTags) {
            tag.gameObject.SetActive(true);
        }
        statusBox.Display(input);
        command = Command.InStatus;
        selectStage = MenuSelectStage.WAIT;
    }

    // 
    public void Item() {
        command = Command.Item;
        selectStage = MenuSelectStage.WAIT;
        itemBox.Display();
    } // Item()

    public void UseItem(int partyIndex) {
        positionSwapIndex = partyIndex;
        itemBox.UseItem();
    } // UseItem()

    public void Equip(int input) {
        equipBox.Display(input);
        command = Command.Equip;
        selectStage = MenuSelectStage.WAIT;
    }

    // Allows player to select player to perform command on
    public void SelectCharacter(int input) {
        command = (Command)input;
        selectStage = MenuSelectStage.CHARACTER;
        
        // Sanity check: can't swap positions of party members if there is only one member
        if(cTags.Count == 1 && command == Command.Position) {
            return;
        }
        
        cTags[0].gameObject.GetComponent<Button>().Select();
    } // SelectCharacter()

    // Calls appropriate action with saved command and inputed party member
    // Input: partyIndex -> index of party member being accessed
    public void CallAction(int partyIndex) { 
        selectStage = MenuSelectStage.WAIT;
        switch(command) {
            case Command.Status:
                Status(partyIndex);
                break;
            case Command.Item:
                UseItem(partyIndex);
                break;
            case Command.Equip:
                Equip(partyIndex);
                break;
            case Command.Position:
                Position(partyIndex);
                break;
            case Command.Swap:
                SwapPositions(partyIndex);
                break;
        }
    } // CallAction()

    public void Position(int partyIndex) {
        positionSwapIndex = partyIndex;
        command = Command.Swap;

        for(int i = 0; i < cTags.Count; ++i) {
            Navigation emptyNav = new Navigation();
            emptyNav.mode = Navigation.Mode.Explicit;
    
            cTags[i].gameObject.GetComponent<Button>().navigation = emptyNav;
        }

        cTags[partyIndex].transform.position = new Vector2(cTags[partyIndex].transform.position.x - 20f, cTags[partyIndex].transform.position.y);

        SetNavigation(partyIndex);
        if(partyIndex != 0) {
            cTags[0].GetComponent<Button>().Select();
        } else {
            cTags[1].GetComponent<Button>().Select();
        }
        selectStage = MenuSelectStage.WAIT;
    }

    public void SwapPositions(int partyIndex) {
        Party.Instance.SwapMembers(positionSwapIndex, partyIndex);
        Party.Instance.Setup();

        Vector2 positionSwap = new Vector2(cTags[positionSwapIndex].transform.position.x + 20f, cTags[positionSwapIndex].transform.position.y);
        cTags[positionSwapIndex].transform.position = cTags[partyIndex].transform.position;
        cTags[partyIndex].transform.position = positionSwap;

        MenuCharacterTag temporary = cTags[positionSwapIndex];
        cTags[positionSwapIndex] = cTags[partyIndex];
        cTags[partyIndex] = temporary;

        for (int i = 0; i < cTags.Count; ++i) {
            MenuCharacterTag cTag = cTags[i];
            int pIndex = i;
            cTag.GetComponent<Button>().onClick.RemoveAllListeners();
            cTag.GetComponent<Button>().onClick.AddListener(delegate {
                GameMenu.Instance.CallAction(pIndex);
            });
        }
        
        positionSwapIndex = -1;

        SetNavigation();
        defaultButton.Select();
        selectStage = MenuSelectStage.WAIT;
        command = Command.Status;
    }

    private void SetNavigation(int ignoreIndex = -2) {
        for(int i = 0; i < cTags.Count; ++i) {
            int prevIndex = i - 1;
            if(prevIndex == ignoreIndex) {
                --prevIndex;
            }
            
            int nextIndex = i + 1;
            if(nextIndex == ignoreIndex) {
                ++nextIndex;
            }

            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;

            if(prevIndex >= 0) {
                nav.selectOnUp = cTags[prevIndex].GetComponent<Button>();
            }

            if(nextIndex < cTags.Count) {
                nav.selectOnDown = cTags[nextIndex].GetComponent<Button>();
            }

            cTags[i].gameObject.GetComponent<Button>().navigation = nav;
        }
    }

    // Toggles UI on and off
    public void ChangeStates() {
        if (menuIsActive) {
            Deactivate();
        } else {
            menuUI.SetActive(true);
            menuIsActive = true;
            defaultButton.Select();
        }
    } // ChangeStates

    // Deactivate UI
    public void Deactivate()
    {
        menuUI.SetActive(false);
        statusBox.Hide();
        itemBox.Hide();
        equipBox.Hide();
        /*foreach(Button button in commands) {
            button.gameObject.SetActive(true);
        }*/
        foreach(MenuCharacterTag tag in cTags) {
            tag.gameObject.SetActive(true);
        }
        menuIsActive = false;
    } // Deactivate

    public void GoBack() {
        if(selectStage == MenuSelectStage.CHARACTER) {
            selectStage = MenuSelectStage.WAIT;
            if(command == Command.Item) {
                itemBox.ReturnToItem();
                return;
            }
            defaultButton.Select();
            command = Command.Status;
            return;
        }
        switch(command) {
            case Command.Status:
                Deactivate();
                break;
            case Command.Item:
                if(itemBox.selecting) {
                    itemBox.GoBack();
                    return;
                } else {
                    itemBox.Hide();
                    foreach(MenuCharacterTag tag in cTags) {
                        tag.gameObject.SetActive(true);
                    }
                    selectStage = MenuSelectStage.WAIT;
                    command = Command.Status;
                }
                break;
            case Command.Equip:
                if(equipBox.selecting) {
                    equipBox.GoBack();
                    return;
                } else {
                    equipBox.Hide();
                    foreach(MenuCharacterTag tag in cTags) {
                        tag.gameObject.SetActive(true);
                    }
                    selectStage = MenuSelectStage.WAIT;
                    command = Command.Status;
                }
                break;
            case Command.Position:
                command = Command.Status;
                cTags[positionSwapIndex].transform.position = new Vector2(cTags[positionSwapIndex].transform.position.x + 20f, cTags[positionSwapIndex].transform.position.y);
                Debug.Log("GOING BACK");
                break;
            case Command.Swap:
                Debug.Log("GOING BACK");
                cTags[positionSwapIndex].transform.position = new Vector2(cTags[positionSwapIndex].transform.position.x + 20f, cTags[positionSwapIndex].transform.position.y);
                command = Command.Status;
                break;
            case Command.InStatus:
                statusBox.Hide();
                /*foreach(Button button in commands) {
                    button.gameObject.SetActive(true);
                }*/
                foreach(MenuCharacterTag tag in cTags) {
                    tag.gameObject.SetActive(true);
                }
                selectStage = MenuSelectStage.WAIT;
                command = Command.Status;
                break;
        }
        defaultButton.Select();
    }

    public void ReturnToDefault() {
        selectStage = MenuSelectStage.WAIT;
        command = Command.Status;
        defaultButton.Select();
    }

    public bool Opened() { return menuIsActive; } // Opened()
    public int GetTargetIndex() { return positionSwapIndex; } // GetTargetIndex()
}
