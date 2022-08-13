using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Class controlling the exp and level of characters after completing a battle
// Works with the victory tag prefab in the editor
// When a victory screen is instatiated, it spawns a victory tag corresponding to each character
public class VictoryTag : MonoBehaviour {
    const float X_POS = 216.25f;
    const float START_Y_POS = 176.5f;
    const float Y_POS_OFFSET = 136.5f;
    
    // Spawns tag in necessary location given index in the party
    public static VictoryTag Spawn(int partyIndex, int expGain, GameObject parent) {
        float Y_POS = START_Y_POS - (Y_POS_OFFSET) * partyIndex;
        Transform tagTransform = Instantiate(GameAsset.Instance.victoryTag, new Vector3(X_POS, Y_POS, 0), Quaternion.identity);
        tagTransform.SetParent(parent.transform, false);
        
        VictoryTag tag = tagTransform.gameObject.GetComponent<VictoryTag>();

        tag.Setup(partyIndex, expGain);

        return tag;
    } // Spawn()

    CharacterRuntime characterRef;
    // GameObject parentRef;

    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI maxEXPText;
    public TextMeshProUGUI levelText;

    private int level;
    private float exp;
    private int maxEXP;

    private float expGained;

    public Image fill;
    public Image border;

    bool finished;

    void Update() { }
    
    // Sets up tag information with character info from party[partyIndex]
    // Input: partyIndex -> index of character this tag belongs to
    // Input: expGain -> amount of exp being gained for the tag to update
    public void Setup(int partyIndex, int expGain) {
        CharacterRuntime character = Party.Instance.current_party[partyIndex].GetComponent<CharacterRuntime>();
        characterRef = character;

        characterNameText.SetText(character.name);

        level = character.GetLevel();
        exp = (float)character.EXP();
        maxEXP = character.EXPNeeded();

        Debug.Log(level);

        levelText.SetText("LV " + level.ToString());
        expText.SetText(character.EXP().ToString());
        maxEXPText.SetText("/" + character.EXPNeeded().ToString());

        float expRatio = exp / (float)maxEXP;
        fill.transform.localScale = new Vector3(expRatio, 1, 1);

        expGained = (float)expGain;
        finished = false;
    } // Setup()

    public void StartEXPGain() { StartCoroutine(GainEXP()); }
    
    // They can manually skip the EXP update by clicking the trigger button
    // This function considers the current exp left to gain and updates character exp and level accordingly
    public void SkipEXPGain() {
        StopAllCoroutines();
        finished = true;
        
        while(exp + expGained >= maxEXP) {
            characterRef.LevelUp();
            expGained -= maxEXP - exp;
            
            exp = 0f;
            maxEXP = characterRef.EXPNeeded();
        }

        exp += expGained;
        expGained = 0;

        expText.SetText(Mathf.RoundToInt(exp).ToString());
        maxEXPText.SetText("/" + characterRef.EXPNeeded().ToString());

        level = characterRef.GetLevel(); 
        levelText.SetText("LV " + level.ToString());

        float expRatio = exp / (float)maxEXP;
        fill.transform.localScale = new Vector3(expRatio, 1, 1);

        characterRef.SetEXP((int)exp);
    } // SkipEXPGain()

    // Visually update EXP and level by scaling a bar and updating values accordingly
    IEnumerator GainEXP() {
        while(expGained > 0) {
            exp += 0.02f;
            expGained -= 0.02f; 

            if(Mathf.Floor(exp) >= maxEXP) {
                characterRef.LevelUp();
                exp = 0f;
                maxEXP = characterRef.EXPNeeded();
                level = characterRef.GetLevel(); 
                levelText.SetText("LV " + level.ToString());
                Debug.Log(level);
                maxEXPText.SetText("/" + characterRef.EXPNeeded().ToString());
            }

            expText.SetText(Mathf.Floor(exp).ToString());

            float expRatio = exp / (float)maxEXP;
            fill.transform.localScale = new Vector3(expRatio, 1, 1);
            yield return new WaitForSeconds(0.001f);
        }
        
        finished = true;
        characterRef.SetEXP((int)exp);
    } // GainEXP()

    public bool Finished() { return finished; }
}
