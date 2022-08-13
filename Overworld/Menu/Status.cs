using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Displays and configures the status function of the menu
// Works with the status gameobject within the menu
public class Status : MonoBehaviour
{
    public Image box;

    // Character Tag Information
    public Image profile;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI charaterLevel;
    public TextMeshProUGUI currHP;
    public TextMeshProUGUI currSP;
    public TextMeshProUGUI maxHP;
    public TextMeshProUGUI maxSP;
    public TextMeshProUGUI currEXP;
    public TextMeshProUGUI maxEXP;

    // Character Equipment Information
    public TextMeshProUGUI weapon;
    public TextMeshProUGUI helm;
    public TextMeshProUGUI body;
    public TextMeshProUGUI arm;
    public TextMeshProUGUI accessory;

    // Character Stats
    public TextMeshProUGUI maxHPStat;
    public TextMeshProUGUI maxSPStat;
    public TextMeshProUGUI attack;
    public TextMeshProUGUI defense;
    public TextMeshProUGUI dexterity;
    public TextMeshProUGUI crit;
    public TextMeshProUGUI con;
    public TextMeshProUGUI magAttack;
    public TextMeshProUGUI magDefense;
    public TextMeshProUGUI evasion;
    public TextMeshProUGUI speed;

    public Image hpFill;
    public Image spFill;
    public Image expFill;

    private Vector2 hpBarPos;
    private Vector2 spBarPos;
    
    // Set hpBarPos, spBarPos to ORIGINAL positions of hp and sp fills
    private void Awake() {
        hpBarPos = new Vector2(hpFill.transform.position.x, hpFill.transform.position.y);
        spBarPos = new Vector2(spFill.transform.position.x, spFill.transform.position.y);
    }
    
    int index = 0; // characterRef index

    // Displays information about character on status gameobject
    // Called when the status button in the menu is clicked
    // Input: indexIn -> index of corresponding character
    public void Display(int indexIn) {
        gameObject.SetActive(true);

        index = indexIn;
        Character character = Party.Instance.current_party[index].GetComponent<CharacterRuntime>().character;

        UpdateHP(Party.Instance.current_party[index].GetComponent<CharacterRuntime>());
        UpdateSP(Party.Instance.current_party[index].GetComponent<CharacterRuntime>());
        UpdateEXP(Party.Instance.current_party[index].GetComponent<CharacterRuntime>());

        // Set Tag Information
        profile.sprite = character.profile;
        characterName.SetText(character.char_name);
        charaterLevel.SetText("Lv " + Party.Instance.current_party[index].GetComponent<CharacterRuntime>().GetLevel().ToString());
        currHP.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().hp.ToString());
        currSP.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().sp.ToString());
        maxHP.SetText("/" + character.hp_stat.ToString());
        maxSP.SetText("/" + character.sp_stat.ToString());

        // Set Equipment Information
        try {
            weapon.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().weapon.weaponName);
            helm.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().headArmor.armorName);
            body.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().bodyArmor.armorName);
            arm.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().armArmor.armorName);
            accessory.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().accessory.armorName);
        } catch {
            Debug.Log("ERROR");
        }
        

        // Set Stats
        maxHPStat.SetText(character.hp_stat.ToString());
        maxSPStat.SetText(character.sp_stat.ToString());
        attack.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().GetAttack().ToString());
        magAttack.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().GetMagic().ToString());
        defense.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().GetDefense().ToString());
        magDefense.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().GetMagicDefense().ToString());
        dexterity.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().GetDexterity().ToString());
        evasion.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().GetEvasion().ToString());
        crit.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().GetCrit().ToString());
        speed.SetText(Party.Instance.current_party[index].GetComponent<CharacterRuntime>().GetSpeed().ToString());
        con.SetText(character.con_stat.ToString());
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    // Shifts status to next character in party
    public void ShiftRight() {
        Display((index + 1) % Party.Instance.current_party.Count);
    } // ShiftRight()

    // Shifts status to previous character in party
    public void ShiftLeft() {
        Display(((index - 1) + Party.Instance.current_party.Count) % Party.Instance.current_party.Count);
    } // ShiftLeft()

    // Updates HP bar to correct placement by shifting fill bar a certain ratio
    // Called on display
    public void UpdateHP(CharacterRuntime characterRef) {
        int hp = characterRef.hp;
        currHP.SetText(hp.ToString());

       //  int hpDiff = characterRef.GetMaxHP() - hp;

        float hpRatio = (float)hp / (float)characterRef.GetMaxHP();
        hpFill.fillAmount = hpRatio;
        // float hpShiftDist = hpRatio * hpFill.rectTransform.rect.width * GameMenu.Instance.canvas.transform.localScale.x;

        // hpFill.transform.position = new Vector2(hpBarPos.x - hpShiftDist, hpBarPos.y);
    } // UpdateHP()

    // Updates SP bar to correct placement by shifting fill bar a certain ratio
    // Called on display
    public void UpdateSP(CharacterRuntime characterRef) {
        int sp = characterRef.sp;
        currSP.SetText(sp.ToString());

        // int spDiff = characterRef.GetMaxSP() - sp;

        float spRatio = (float)sp / (float)characterRef.GetMaxSP();
        spFill.fillAmount = spRatio;
        // float spShiftDist = spRatio * spFill.rectTransform.rect.width * GameMenu.Instance.canvas.transform.localScale.x;

        // spFill.transform.position = new Vector2(spBarPos.x - spShiftDist, spBarPos.y);
    } // UpdateSP()

    // Updates EXP bar to correct size by scaling fill bar by certain ratio
    // Called on display
    public void UpdateEXP(CharacterRuntime characterRef) {
        int exp = characterRef.EXP();
        int maxEXPNum = characterRef.EXPNeeded();

        currEXP.SetText(characterRef.EXP().ToString());
        maxEXP.SetText("/" + characterRef.EXPNeeded().ToString());

        float expRatio = (float)exp / (float)maxEXPNum;
        expFill.transform.localScale = new Vector3(expRatio, 1, 1);
    } // UpdateEXP()
}
