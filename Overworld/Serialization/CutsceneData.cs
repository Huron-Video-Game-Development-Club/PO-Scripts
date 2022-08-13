using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Storage class for serializing cutscene date
[System.Serializable]
public class CutsceneData {
    public int eventIterator;
    public int dialogueIterator;
    public int battleIterator;
    public CutsceneManager.CutsceneState state;

    public CutsceneData(CutsceneManager cutscene) {
        eventIterator = cutscene.GetEventIterator();
        dialogueIterator = cutscene.GetDialogueIterator();
        battleIterator = cutscene.GetBattleIterator();

        state = cutscene.state;
    } // CutsceneData()
}
