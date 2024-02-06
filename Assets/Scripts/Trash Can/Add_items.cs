using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Add_items : MonoBehaviour
{
    [SerializeField] Invertory_logic _invertoryLogic;
    [SerializeField] Invertory_UI _invertoryUI;
    public List<Button> AddButtons = new List<Button>();
    public List<Item_so> itemsList = new List<Item_so>();
    private void Start()
    {
        for (int i = 0; i < AddButtons.Count; i++)
        {
            int index = i;
            AddButtons[i].onClick.AddListener(() => add(index));
        }
    }

    private void add(int index)
    {
        if (_invertoryUI.Slots.Count > _invertoryLogic._invertoryList.Count)
        {           
            _invertoryLogic.addItem(itemsList[index]);
            _invertoryUI.redraw();
        }
        else
        {
            _invertoryLogic.stackItem(itemsList[index]);
            _invertoryUI.redraw();
        }
    }
}
