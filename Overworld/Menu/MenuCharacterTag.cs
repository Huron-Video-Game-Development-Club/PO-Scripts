using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuCharacterTag : MonoBehaviour
{
    const float X_POS = 113f;
    const float START_Y_POS = 181f;
    const float Y_POS_OFFSET = 122f;
    
    // Spawns tag in necessary location given index in the party
    public static MenuCharacterTag Spawn(int partyIndex, GameObject characterTags)
    {
        float Y_POS = START_Y_POS - (Y_POS_OFFSET) * partyIndex;
        Transform tag_transform = Instantiate(GameAsset.Instance.menuCharacterTag, new Vector3(X_POS, Y_POS, 0), Quaternion.identity);
        tag_transform.SetParent(characterTags.transform, false);
        
        MenuCharacterTag tag = tag_transform.gameObject.GetComponent<MenuCharacterTag>();

        tag.Setup(partyIndex, characterTags);

        return tag;
    } // Spawn()

    /* Maybe not necessary
    public GameObject characterNameObject;
    public GameObject hpObject;
    public GameObject maxHPObject;
    public GameObject spObject;
    public GameObject maxSPObject;
    */
    CharacterRuntime characterRef;
    GameObject parentRef;

    public Image profile;

    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI charaterLevel;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI maxHPText;
    public TextMeshProUGUI spText;
    public TextMeshProUGUI maxSPText;

    private int hp;
    private int sp;
    private int maxHP;
    private int maxSP;

    public Image hpFill;
    public Image spFill;

    private Vector2 hpBarPos;
    private Vector2 spBarPos;

    private Vector2 originalPos;

    void Update() {
        if(characterRef != null) {
            hp = characterRef.hp;
            UpdateHP();

            sp = characterRef.sp;
            UpdateSP();
            charaterLevel.SetText("Lv " + characterRef.GetLevel().ToString());
        }  
    }
    
    // Sets up tag information with character info from party[partyIndex]
    public void Setup(int partyIndex, GameObject parent) {
        CharacterRuntime character = Party.Instance.current_party[partyIndex].GetComponent<CharacterRuntime>();
        
        profile.sprite = character.character.profile;
        characterNameText.SetText(character.name);
        charaterLevel.SetText("Lv " + character.GetLevel().ToString());

        hp = character.hp;
        Debug.Log("Name: " + character.char_name);
        Debug.Log("HP: " + character.hp);
        maxHP = character.GetMaxHP();
        sp = character.sp;
        maxSP = character.GetMaxSP();

        hpText.SetText(hp.ToString());
        maxHPText.SetText("/" + maxHP.ToString());
        spText.SetText(sp.ToString());
        maxSPText.SetText("/" + maxSP.ToString());

        hpBarPos = new Vector2(hpFill.transform.position.x, hpFill.transform.position.y);
        spBarPos = new Vector2(spFill.transform.position.x, spFill.transform.position.y);

        originalPos = transform.position;

        characterRef = character;
        parentRef = parent;

        UpdateHP();
        UpdateSP();

        gameObject.GetComponent<Button>().onClick.AddListener(delegate {
            GameMenu.Instance.CallAction(partyIndex);
        });
    } // Setup()

    public void UpdateHP() {
        hp = characterRef.hp;
        hpText.SetText(hp.ToString());
        /*

        int hpDiff = maxHP - hp;

        float hpRatio = (float)hpDiff / (float)maxHP;

        float hpShiftDist = hpRatio * hpFill.rectTransform.rect.width * parentRef.transform.localScale.x;
        hpFill.transform.position = new Vector2(hpBarPos.x - hpShiftDist, hpBarPos.y) + (Vector2)transform.position - originalPos;*/
        float hpRatio = (float)hp / (float)maxHP;
        hpFill.transform.localScale = new Vector3(hpRatio, 1, 1);
    } // UpdateHP()

    public void UpdateSP() {
        sp = characterRef.sp;
        spText.SetText(sp.ToString());

        /*int spDiff = maxSP - sp;

        float spRatio = (float)spDiff / (float)maxSP;

        float spShiftDist = spRatio * spFill.rectTransform.rect.width * GameMenu.Instance.canvas.transform.localScale.x;

        spFill.transform.position = new Vector2(spBarPos.x - spShiftDist, spBarPos.y) + (Vector2)transform.position - originalPos;*/
        float spRatio = (float)sp / (float)maxSP;
        spFill.transform.localScale = new Vector3(spRatio, 1, 1);
    } // UpdateSP()

    
}
