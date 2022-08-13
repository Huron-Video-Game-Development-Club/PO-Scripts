using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemList : MonoBehaviour
{
    // Controls when to update 
    public bool visible;
    public bool selecting;

    private int y;
    private int x;
    private int list_start;
    private int list_end;

    private Button lastButton;
    private ItemWrapper itemToUse;

    public List<GameObject> items;
    public Button use;
    public Button arrange;
    public Button key;
    // Start is called before the first frame update
    void Start() {
        list_start = 0;
        // list_end = Mathf.Min(list_start + items.Count, Party.Instance.inventory.GetNumItems());
        // GetComponent<GridLayoutGroup>().cellSize = new Vector2(GetComponent<RectTransform>().rect.width/2, GetComponent<RectTransform>().rect.height/11);
    }

    // Update is called once per frame
    void Update() {
        if(!visible) return;

        if(selecting) GetInput();
    }

    // Get input
    void GetInput() {
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Debug.Log(list_end);
            Debug.Log(list_start);
            ++y;
            if (y > (int)Mathf.Ceil(((float)list_end - (float)list_start)/2) - 1) {
                y = (int)Mathf.Ceil(((float)list_end - (float)list_start)/2) - 1;
                if (list_end != Party.Instance.inventory.GetNumItems()) {
                    list_start += 2;
                    list_end += 2;
                    if (list_end > Party.Instance.inventory.GetNumItems()) {
                        list_end = Party.Instance.inventory.GetNumItems();
                    }
                    Display();
                }
            }
            Debug.Log("Y: " + y + " and " + (list_end - list_start));
            items[x + (2 * y)].GetComponent<Button>().Select();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            --y;
            if(y < 0) {
                y = 0;
                if (list_start != 0) {
                    list_start -= 2;
                    list_end -= 2;
                    if(list_end % 2 == 1) {
                        list_end += 1;
                    }
                    Display();
                }
            }
            items[x + (2 * y)].GetComponent<Button>().Select();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ++x;
            if (x > 1) x = 1;
            items[x + (2 * y)].GetComponent<Button>().Select();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            --x;
            if (x < 0) x = 0;
            items[x + (2 * y)].GetComponent<Button>().Select();
        }
    }

    public void Display() {
        if(!visible) {
            list_start = 0;
            x = 0;
            y = 0;
            use.Select();
            selecting = false;
        }

        visible = true;
        gameObject.SetActive(true);

        for(int i = 0; i < items.Count; ++i) {
            if(list_start + i < Party.Instance.inventory.GetNumItems()) {
                items[i].SetActive(true);
                items[i].GetComponent<MenuItem>().itemName.SetText(Party.Instance.inventory.GetItem(list_start + i).GetName().ToString());
                items[i].GetComponent<MenuItem>().itemCount.SetText(Party.Instance.inventory.GetItem(list_start + i).GetQuantity().ToString());
            } else {
                items[i].SetActive(false);
                Debug.Log("HERE");
            }
        }

        list_end = Mathf.Min(list_start + items.Count, Party.Instance.inventory.GetNumItems());
        Debug.Log("Num Items: " + Party.Instance.inventory.GetNumItems());
        Debug.Log(list_end);
    }

    public void ReturnToItem() {
        visible = true;
        gameObject.SetActive(true);

        items[x + (2 * y)].GetComponent<Button>().Select();
    }

    public void Hide() {
        visible = false;
        gameObject.SetActive(false);
    }

    public void Use() {
        selecting = true;
        items[0].GetComponent<Button>().Select();
        lastButton = use;
        for(int i = 0; i < items.Count; ++i) {
            if(list_start + i < Party.Instance.inventory.GetNumItems()) {
                items[i].GetComponent<Button>().onClick.AddListener(delegate {
                    GameMenu.Instance.SelectCharacter(1);
                    Hide();
                });
            }
        }
    }

    // Invokes the selected item's use event
    public void UseItem() {
        Party.Instance.inventory.UseItem(list_start + x + (2 * y));
        GameMenu.Instance.ReturnToDefault();
    }

    public void GoBack() {
        Debug.Log("HERE");
        selecting = false;
        lastButton.Select();
        Debug.Log(lastButton);
    }
}
