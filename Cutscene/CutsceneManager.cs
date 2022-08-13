using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[Serializable]
public class DialogueChain {
    public Dialogue[] dialogueChain;
}

/* My favorite file! (jk)
 * Unity's cutscene support is confusing and sucks (or I'm just stupid),
 * Anyway, I made my own class to handle cutscenes!
 * It's a bit scuffed but it does it's job well.
 * 
 * How does it work?
 * Cutscene information is stored in a gameobject in the scene
 * This information includes:
 *   > Dialogue sequences in the cutscene
 *   > Any battles that happen during or as a result of the cutscene
 *   > Most importantly: a list of all events happening in the cutscene
 *      - These events can be moving characters, starting dialogue, etc.
 *
 * As the cutscene plays, it iterates through and calls each event. 
 * Dialogue and battle iterators are also stored to keep track of which dialogue chain and battle comes next
 * 
 * By adding a trigger collider, cutscenes are automatically started 
 * when the player enters the trigger collision zone. Cutscenes can also be 
 * manually started by calling the StartCutscene() method externally
 *
 * When a cutscene is started, that cutscene occupies the static CutsceneManager reference: "Instance"
 * This allows the ongoing cutscene to be accessed statically as a singleton like so:
 *   > CutsceneManager.Instance.PlayNextEvent() -- easily accessible
 * At the end of the cutscene, the Instance reference is reset to null and no longer references the cutscene.
 */
public class CutsceneManager : MonoBehaviour {
    private static CutsceneManager instance;

    public static CutsceneManager Instance {
        get {
            if (instance == null && Party.Instance != null && Party.Instance.InCutscene()) {
                instance = FindObjectOfType<CutsceneManager>();
            }
            return instance;
        }
    }

    public bool replayable;

    public List<CutsceneNPC> npcs; // Useless in current context
    public List<DialogueChain> dialogueChains;
    public List<Encounter> battles;
    
    private int dialogueIterator;
    private int battleIterator;
    public List<UnityEvent> events;
    private int eventIterator;
    private bool playEventOnFirstFrame;
    public int partyIndex;

    public enum CutsceneState
    {
        None = 0,
        Ongoing = 1,
        Finished = 2,
        StartCutsceneOnPlay = 3
    };

    public CutsceneState state;

    // Load cutscene data from serialized files before first frame in scene
    // Used to save state of cutscenes between scenes
    private void Start() {
        playEventOnFirstFrame = false;
        CutsceneData data = SceneSaver.Instance.LoadCutsceneData(gameObject.name);
        if(data is null) {
            Debug.Log("Got Nothing!");
            eventIterator = 0;
            dialogueIterator = 0;
            battleIterator = 0;
        } else {
            eventIterator = data.eventIterator;
            Debug.Log(eventIterator);
            dialogueIterator = data.dialogueIterator;
            battleIterator = data.battleIterator;
            state = data.state;
            if(state == CutsceneState.Ongoing) {
                playEventOnFirstFrame = true;
            }
        }
        if(state == CutsceneState.StartCutsceneOnPlay) {
            StartCutscene();
        }
        Debug.Log(playEventOnFirstFrame);
    } // Start()

    void Update() {
        if(!playEventOnFirstFrame) {
            return;
        }

        instance = this;
        
        if(Party.Instance != null) {
            Party.Instance.EnterCutscene(this);
            Party.Instance.current_party[0].GetComponent<PlayerController>().LockInput();
        }
        PlayNextEvent();
        playEventOnFirstFrame = false;
    } // Update()

    // Starts cutscene immediately when player enters trigger collision zone
    void OnTriggerEnter2D() {
        if (state == CutsceneState.None) {
            StartCutscene();
        }
    } // OnTriggerEnter2D()
    
    // Guys, I wonder what this does
    public void StartCutscene() {
        instance = this;

        // Cutscenes can be played without an active Party
        // However, if there is an active party, the party is told to enter the cutscene 
        // and input is momentarily taken away from the player.
        if(Party.Instance != null) {
            Party.Instance.EnterCutscene(this);
            Party.Instance.current_party[0].GetComponent<PlayerController>().LockInput();
        }
        
        state = CutsceneState.Ongoing;
        eventIterator = 0;
        dialogueIterator = 0;
        battleIterator = 0;
        events[eventIterator].Invoke();
    } // StartCutscene()

    // I'll never guess this one
    public void EndCutscene() {
        if(Party.Instance != null) {
            Party.Instance.ExitCutscene();
            Party.Instance.current_party[0].GetComponent<PlayerController>().UnlockInput();
        }

        if(replayable) {
            state = CutsceneState.None;
        } else {
            state = CutsceneState.Finished;
        }
        
        instance = null;
    } // EndCutscene()

    // Plays next event in cutscene and tells everyone it's doing so (by logging to the console lol)
    public void PlayNextEvent() {
        Debug.Log("EVENT ITERATOR: " + eventIterator);
        ++eventIterator;
        Debug.Log("----------Playing NEXT EVENT-----------: " + eventIterator);
        events[eventIterator].Invoke();
    } // PlayNextEvent()

    // Starts the next dialogue sequence
    public void StartDialogueSequence() {
        Debug.Log("...Starting Dialogue Sequence");
        DialogueManager.Instance.PrintDialogue(dialogueChains[dialogueIterator].dialogueChain);
        ++dialogueIterator;
        Debug.Log(dialogueIterator);
    } // StartDialogueSequence()

    // Starts the next dialogue sequence
    // Does not play next event
    public void StartDialogueSequenceNoNext() {
        DialogueManager.Instance.PrintDialogue(dialogueChains[dialogueIterator].dialogueChain, false);
        ++dialogueIterator;
    } // StartDialogueSequenceNoNext() without 

    public void PlaySound(string sName) {
        AudioManager.Instance.Play(sName);
    } // PlaySound()
    

    public void PlaySFX(string sName) {
        AudioManager.Instance.PlayNoNext(sName);
    } // PlaySound()

    public void StopSound(string sName) {
        AudioManager.Instance.ForceStop();
    } // PlaySound()

    // Has nothing to do with battle move gauge/wait time
    // Literally waits a certain amount of seconds before playing next event
    public void WaitTime(float waitTime) {
        StartCoroutine(Wait(waitTime));
    } // WaitTime()

    // IEnumerator corresponding with WaitTime()
    public IEnumerator Wait(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        PlayNextEvent();
    } // Wait()

    public void StartBattle(string battleMusic) {
        List<int> levels = new List<int>();
        for(int j = 0; j < battles[battleIterator].enemies.Length; ++j) {
            levels.Add((int)UnityEngine.Random.Range(battles[battleIterator].levelLow, battles[battleIterator].levelHigh));
        }
        BattleSceneManager.Spawn(battles[battleIterator].enemies, levels, SceneManager.GetActiveScene().name, battleMusic, battles[battleIterator].mandatory);
        SwitchToBattle.Instance.FadeToLevel();
    } // StartBattle()

    // Getters
    public int GetEventIterator() { return eventIterator; }
    public int GetDialogueIterator() { return dialogueIterator; }
    public int GetBattleIterator() { return battleIterator; }
    public CutsceneState GetState() { return state; }

    // Reverses the character leading the party
    public void ReversePartyLead() {
        Party.Instance.current_party[0].GetComponent<PlayerController>().Reverse();
    }
    
    // Tells camera to follow a different transform
    public void SetVCamFollow(Transform t) {
        MainVCam.Instance.gameObject.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = t;
    } // SetVCamFollow

    // Tells camera to follow party member correspoding with current partyIndex
    public void SetVCamFollowParty() {
        MainVCam.Instance.gameObject.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = Party.Instance.current_party[partyIndex].transform;
    }
    
    // Prioritizes different camera
    public void SwitchToVCam(Cinemachine.CinemachineVirtualCamera vcam) {
        MainVCam.Instance.SwitchToCam(vcam);
    } // SwitchToVCam

    // Prioritizes different camera
    public void SwitchFromVCam(Cinemachine.CinemachineVirtualCamera vcam) {
        MainVCam.Instance.SwitchFromCam(vcam);
    } // SwitchFromVCam

    public void StartAssassinationEffect(GameObject canvas) {
        AssassinationEffect.Spawn(canvas);
    }

    /* Unfortunately, because characters are saved in DontDestroyOnLoad 
     * and may come from a different scene than the ongoing cutscene,
     * It's impossible, to my knowledge, to call methods directly from a party member reference
     * 
     * Instead, to force party members to perform cutscene actions, a party index, 
     * which references characters through their index in the current party, must be set
     * After doing so, party member cutscene actions can be called indirectly through the ongoing cutscene
     *
     * partyIndex can be set in two different ways:
     *   1. Directly set index with an integer [O(1)] -- recommended if you want to call an action on a specifc
     *      position in the party (like the party lead)
     *   2. Set party index through character name [O(n)] -- recommended if you want a specific character to 
     *      perform a cutscene action.
     *
     * SetPartyIndex() should be called before any party member action
     *   > unless the party index is already correctly set from a previous call
     */
    // Input: index -> position in party to call actions on
    public void SetPartyIndex(int index) {
        partyIndex = index;
        PlayNextEvent();
    } // SetPartyIndex()

    // Set party index via party member name
    public void SetPartyIndex(string memberName) {
        for(int i = 0; i < Party.Instance.current_party.Count; ++i) {
            if(Party.Instance.current_party[i].GetComponent<CharacterRuntime>().char_name != memberName) {
                continue;
            }

            partyIndex = i;
        }
        PlayNextEvent();
    } // SetPartyIndex()
    
    // =========================
    // Party Member Action Calls
    // =========================
    public void RunX(float distance) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().RunToPointX(distance);
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().RunToPointX(distance);
        }
    } // RunX()

    public void RunY(float distance) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().RunToPointY(distance);
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().RunToPointY(distance);
        }
    } // RunY()

    public void RunYNoNext(float distance) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().RunToPointYNoNext(distance);
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().RunToPointYNoNext(distance);
        }
    } // RunYNoNext()

    public void ShiftY(float distance) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().ShiftToPointY(distance);
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().ShiftToPointY(distance);
        }
    } // ShiftY()

    public void ShiftYNoNext(float distance) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().ShiftToPointYNoNext(distance);
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().ShiftToPointYNoNext(distance);
        }
    } // ShiftYNoNext()

    public void RunXThenY(Transform t) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().RunToPointXThenY(t);
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().RunToPointXThenY(t);
        }
    } // RunXThenY()

    public void RunXThenYNoNext(Transform t) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().RunToPointXThenYNoNext(t);
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().RunToPointXThenYNoNext(t);
        }
    } // RunXThenYNoNext()

    public void RunYThenX(Transform t) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().RunToPointYThenX(t);
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().RunToPointYThenX(t);
        }
    } // RunYThenX()

    public void RunYThenXNoNext(Transform t) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().RunToPointYThenXNoNext(t);
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().RunToPointYThenXNoNext(t);
        }
    } // RunYThenXNoNext()

    public void LookDown(bool playNext) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().LookDown(playNext);
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().LookDown(playNext);
        }
    } // LookDown()

    public void LookUp(bool playNext) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().LookUp(playNext);
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().LookUp(playNext);
        }
    } // LookUp()

    public void LookRight(bool playNext) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().LookRight(playNext);
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().LookRight(playNext);
        }
    } // LookRight()

    public void LookLeft(bool playNext) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().LookLeft(playNext);
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().LookLeft(playNext);
        }
    } // LookLeft()

    public void SetPos(Transform t) {
        if(partyIndex == 0) {
            Party.Instance.current_party[partyIndex].GetComponent<PlayerController>().StopAllCoroutines();
        } else {
            Party.Instance.current_party[partyIndex].GetComponent<FollowMain>().StopAllCoroutines();
        }
        Party.Instance.current_party[partyIndex].transform.position = t.position;
    } // SetPos()

    public void PlayPartyMemberAnimation(string transitionBool) {
        Party.Instance.current_party[partyIndex].GetComponent<CharacterRuntime>().StartAnimationStillAttacking(transitionBool);
    }
}
