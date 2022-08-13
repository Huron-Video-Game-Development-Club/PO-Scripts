using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuItem : MonoBehaviour
{
    /*public static MenuItem Spawn(ItemWrapper item, bool left, Vector3 initialPos) {
        if(!left) initialPos = new Vector3(initialPos.x + 370, initialPos.y, 0);
        Transform itemObject = Instantiate(GameAsset.Instance.menuItem, initialPos, Quaternion.identity);
        MenuItem menuItem = itemObject.gameObject.GetComponent<MenuItem>();

        menuItem.Setup(item);

        return menuItem;
    }

    

    Image icon;
    TextMeshProUGUI itemName;
    TextMeshProUGUI itemCount;
    void Setup(ItemWrapper item) { 
        itemName.SetText(item.GetName());
        itemCount.SetText(item.GetQuantity().ToString());
        icon.sprite = item.GetIcon();
    }*/
    public Image icon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemCount;
}
