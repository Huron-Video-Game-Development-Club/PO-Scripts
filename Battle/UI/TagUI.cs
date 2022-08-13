using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TagUI : MonoBehaviour
{
    public static TagUI Spawn(Vector3 position, GameObject character_ref, GameObject canvas)
    {
        Transform tag_transform = Instantiate(GameAsset.Instance.tagUI, position, Quaternion.identity);
        tag_transform.SetParent(canvas.transform, false);
        TagUI tag = tag_transform.gameObject.GetComponent<TagUI>();

        tag.Setup(character_ref.GetComponent<CharacterRuntime>(), canvas);

        return tag;
    }

    private CharacterRuntime character_reference;
    private GameObject canvas_reference;

    public GameObject char_name_object;
    public GameObject current_hp_object;
    public GameObject max_hp_object;
    public GameObject current_sp_object;
    public GameObject max_sp_object;

    private TextMeshProUGUI char_name_text;
    private TextMeshProUGUI current_hp_text;
    private TextMeshProUGUI max_hp_text;
    private TextMeshProUGUI current_sp_text;
    private TextMeshProUGUI max_sp_text;

    private int current_hp;
    private int current_sp;
    private int max_hp;
    private int max_sp;
    // private const int MAX_MOVE_GAUGE = 155366;
    private const int MAX_MOVE_GAUGE = 155366 / 120;

    public Image hp_fill;
    public Image sp_fill;
    public Image atbFill;

    private Vector2 hp_bar_pos;
    private Vector2 sp_bar_pos;
    private Vector2 atbBarPos;

    private void Awake()
    {
        char_name_text = char_name_object.GetComponent<TextMeshProUGUI>();
        current_hp_text = current_hp_object.GetComponent<TextMeshProUGUI>();
        max_hp_text = max_hp_object.GetComponent<TextMeshProUGUI>();
        current_sp_text = current_sp_object.GetComponent<TextMeshProUGUI>();
        max_sp_text = max_sp_object.GetComponent<TextMeshProUGUI>();
    }

    void Setup(CharacterRuntime character_ref, GameObject canvas)
    {
        char_name_text.SetText(character_ref.name);

        current_hp = character_ref.hp;
        max_hp = character_ref.GetMaxHP();
        current_sp = character_ref.sp;
        max_sp = character_ref.GetMaxSP();

        current_hp_text.SetText(current_hp.ToString());
        max_hp_text.SetText("/" + max_hp.ToString());
        current_sp_text.SetText(current_sp.ToString());
        max_sp_text.SetText("/" + max_sp.ToString());

        hp_bar_pos = new Vector2(hp_fill.transform.position.x, hp_fill.transform.position.y);
        sp_bar_pos = new Vector2(sp_fill.transform.position.x, sp_fill.transform.position.y);
        atbBarPos = new Vector2(atbFill.transform.position.x, atbFill.transform.position.y);

        character_reference = character_ref;
        canvas_reference = canvas;
        UpdateHP();
        UpdateSP();
    }

    public void UpdateHP()
    {
        current_hp = character_reference.hp;
        current_hp_text.SetText(current_hp.ToString());

        float hp_ratio = (float)current_hp / (float)max_hp;

        hp_fill.fillAmount = hp_ratio;
        // RectTransform rTransform = hp_fill.GetComponent<RectTransform>();
        // rTransform.position = new Vector2(rTransform.anchoredPosition.x  - hp_shift_dist, rTransform.anchoredPosition.y);
        
    }

    public void UpdateSP()
    {
        current_sp = character_reference.sp;
        current_sp_text.SetText(current_sp.ToString());

        float sp_ratio = current_sp / (float)max_sp;
        sp_fill.fillAmount = sp_ratio;
        // sp_fill.transform.position = new Vector2(sp_bar_pos.x - sp_shift_dist, sp_bar_pos.y);
    }

    public void UpdateATB(){
      //int atbDiff = MAX_MOVE_GAUGE - character_reference.GetMoveGauge();
      float atbRatio = character_reference.GetMoveGauge() / (float)MAX_MOVE_GAUGE;

      //float atbWidthScale = atbRatio * atbFill.rectTransform.rect.width  * canvas_reference.transform.localScale.x;
      //Debug.Log(atbWidthScale);
      atbFill.transform.localScale = new Vector3(atbRatio, 1, 1);
    }
}
