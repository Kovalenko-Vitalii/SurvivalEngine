using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class Invertory_UI : MonoBehaviour
{
    [SerializeField] Invertory_logic _invertoryLogic;
    [Header("List of invntory slots (BUTTONS)")]
    public List<Button> Slots = new List<Button>();

    [Header("Array of cloth slots (BUTTONS)")]
    public Button[] ClothSlots = new Button[4];

    [Header("Array of cloth unequip button (BUTTONS)")]
    public Button[] UequipClothBtn = new Button[4];

    [Header("Array of combination slots (BUTTONS)")]
    public Button[] CombinationSlots = new Button[2];

    [Header("Array of tools slots (BUTTONS)")]
    public Button[] ToolSlots = new Button[2];

    [Header("Array of unequip tools (BUTTONS)")]
    public Button[] UnequipToolBtn = new Button[2];

    [Header("Description images and labels")]
    [SerializeField] TMP_Text selectedName;
    [SerializeField] TMP_Text selectedDescription;
    [SerializeField] Image selectedSprite;

    [Header("Default icon sprites")]
    [SerializeField] Sprite defaultImage;
    [SerializeField] Sprite defaultIcon;

    int selectedIndex = -1;

    [Header("Sounds")]

    public AudioSource audioSource;
    public AudioClip defaultClick;
    public AudioClip negativeClick;
    public AudioClip dropSound;
    public AudioClip wearSound;

    public void Start()
    {
        //Assigning functions to buttons (unequip, select)
        for (int i = 0; i < Slots.Count; i++){
            int index = i; 
            Slots[i].onClick.AddListener(() => select(index));
        }

        for (int i = 0; i < UequipClothBtn.Length; i++){
            int index = i;
            UequipClothBtn[i].onClick.AddListener(() => unequipCloth(index));
        }

        for (int i = 0; i < UnequipToolBtn.Length; i++){
            int index = i;
            UnequipToolBtn[i].onClick.AddListener(() => unequipTool(index));
        }

        redraw();
    }

    public void redraw()
    {
        redrawInventory();
        redrawCloth();
        redrawCombination();
        redrawTool();
    }

    void redrawTool(){
        for (int i = 0; i < ToolSlots.Length; i++)
        {
            Image buttonImage = ToolSlots[i].transform.GetChild(0).GetComponent<Image>();
            TMP_Text name = ToolSlots[i].transform.GetChild(1).GetComponent<TMP_Text>();
            TMP_Text durability = ToolSlots[i].transform.GetChild(2).GetComponent<TMP_Text>();
            if (_invertoryLogic._toolList[i] != null)
            {
                buttonImage.sprite = _invertoryLogic._toolList[i].sprite;
                name.text = _invertoryLogic._toolList[i].name;
                switch (_invertoryLogic._toolList[i])
                {
                    case Flashlight flashlight:
                        durability.text = flashlight.getDurability().ToString();
                        break;
                }
            }
            else
            {
                buttonImage.sprite = defaultIcon;
                name.text = "";
                durability.text = "";
            }
        }
    }

    void redrawCombination(){
        for (int i = 0; i < CombinationSlots.Length; i++)
        {
            Image buttonImage = CombinationSlots[i].transform.GetChild(0).GetComponent<Image>();
            if (_invertoryLogic._combinationList[i] != null)
            {
                buttonImage.sprite = _invertoryLogic._combinationList[i].sprite;
            }
            else
            {
                buttonImage.sprite = defaultIcon;
            }
        }
    }

    void redrawCloth(){
        for (int i = 0; i < ClothSlots.Length; i++)
        {
            Image buttonImage = ClothSlots[i].transform.GetChild(0).GetComponent<Image>();
            if (_invertoryLogic._clothList[i] != null)
            {
                buttonImage.sprite = _invertoryLogic._clothList[i].sprite;
            }
            else
            {
                buttonImage.sprite = defaultIcon;
            }
        }
    }

    void redrawInventory(){
        for (int i = 0; i < Slots.Count; i++)
        {
            Image buttonImage = Slots[i].transform.GetChild(0).GetComponent<Image>();
            TMP_Text buttonQuantity = Slots[i].transform.GetChild(1).GetComponent<TMP_Text>();
            Image indicatorImage = Slots[i].transform.GetChild(3).GetComponent<Image>();
            Image backgroundImage = Slots[i].transform.GetChild(2).GetComponent<Image>();
            if (i < _invertoryLogic._invertoryList.Count)
            {
                buttonImage.sprite = _invertoryLogic._invertoryList[i].sprite;
                buttonQuantity.text = _invertoryLogic._invertoryList[i].quantity.ToString() + "/" + _invertoryLogic._invertoryList[i].maxAmount.ToString();
                if (_invertoryLogic._invertoryList[i] is Magazine magazine)
                {
                    indicatorImage.enabled = true;
                    backgroundImage.enabled = true;
                    indicatorImage.fillAmount = (float)magazine.roundsIn / magazine.maxCapacity;
                }
                else
                {
                    indicatorImage.enabled = false;
                    backgroundImage.enabled = false;

                    indicatorImage.fillAmount = 0;
                }
            }
            else
            {
                buttonImage.sprite = defaultIcon;
                buttonQuantity.text = "";
                indicatorImage.enabled = false;
                backgroundImage.enabled = false;
                indicatorImage.fillAmount = 0;
            }
        }
    }

    public Boolean GetItemAtIndex(int index)
    {
        try
        {
            return _invertoryLogic._invertoryList[index];
        }
        catch (System.ArgumentOutOfRangeException)
        {
            return false;
        }
    }

    void select(int index)
    {  
            if (GetItemAtIndex(index)){
                if (index == selectedIndex){
                    clearSelected(index);
                    audioSource.PlayOneShot(defaultClick);
                    
                }
                else{
                    updateSelected(index);
                    selectedIndex = index;
                    audioSource.PlayOneShot(defaultClick);
                }
            }
            else
            {
                audioSource.PlayOneShot(negativeClick);
            }
    }

    void updateSelected(int index){
        selectedName.text = _invertoryLogic._invertoryList[index].name;
        switch (_invertoryLogic._invertoryList[index])
        {
            case Food foodItem:
                selectedDescription.text = (float)Math.Round((float)foodItem.bites/(float)foodItem.initialBites * 100f, 0)+ "%" + "\n" + _invertoryLogic._invertoryList[index].description;
                break;
            case Medicine medicalItem:
                selectedDescription.text = (float)Math.Round((float)medicalItem.bites / (float)medicalItem.initialBites * 100f, 0) + "%" + "\n" + _invertoryLogic._invertoryList[index].description;
                break;
            default:
                selectedDescription.text = _invertoryLogic._invertoryList[index].description;
                break;
        }
        //selectedDescription.text = _invertoryLogic._invertoryList[index].description;
        selectedSprite.sprite = _invertoryLogic._invertoryList[index].sprite;
        _invertoryLogic._invertoryList[index].selected = true;
    }

    void clearSelected(int index){
        selectedName.text = "";
        selectedDescription.text = "";
        selectedSprite.sprite = defaultImage;
        selectedIndex = -1;
    }

    public void dropItem(){
        if (selectedIndex != -1) {
            Image buttonImage = Slots[selectedIndex].transform.GetChild(0).GetComponent<Image>();
            TMP_Text buttonQuantity = Slots[selectedIndex].transform.GetChild(1).GetComponent<TMP_Text>();
            buttonImage.sprite = defaultIcon;
            buttonQuantity.text = "";
            _invertoryLogic.dropItem(_invertoryLogic._invertoryList[selectedIndex]);
            clearSelected(selectedIndex);
            redraw();
            audioSource.PlayOneShot(dropSound);
        }
        else
        {
            audioSource.PlayOneShot(negativeClick);
        }
    }

    //useItem() ! XAPDKOD - CPO4HO UEPEDELAT!!!!!
    public void useItem(){
        if (selectedIndex != -1)
        {
            switch (_invertoryLogic._invertoryList[selectedIndex])
            {
                case Food:
                    Food foodItem = (Food)_invertoryLogic._invertoryList[selectedIndex];
                    foodItem.eat();
                    audioSource.PlayOneShot(foodItem.useSound);
                    if (foodItem.eaten)
                    {
                        Image buttonImage = Slots[selectedIndex].transform.GetChild(0).GetComponent<Image>();
                        TMP_Text buttonQuantity = Slots[selectedIndex].transform.GetChild(1).GetComponent<TMP_Text>();
                        buttonImage.sprite = defaultIcon;
                        buttonQuantity.text = "";
                        _invertoryLogic._invertoryList.Remove(_invertoryLogic._invertoryList[selectedIndex]);
                        clearSelected(selectedIndex);
                        redraw();
                    }
                    else
                    {
                        updateSelected(selectedIndex);
                        redraw();
                    }
                    break;
                    case Medicine:
                        Medicine medicalItem = (Medicine)_invertoryLogic._invertoryList[selectedIndex];
                        medicalItem.eat();
                        audioSource.PlayOneShot(medicalItem.useSound);
                        if (medicalItem.eaten)
                        {
                            Image buttonImage = Slots[selectedIndex].transform.GetChild(0).GetComponent<Image>();
                            TMP_Text buttonQuantity = Slots[selectedIndex].transform.GetChild(1).GetComponent<TMP_Text>();
                            buttonImage.sprite = defaultIcon;
                            buttonQuantity.text = "";
                            _invertoryLogic._invertoryList.Remove(_invertoryLogic._invertoryList[selectedIndex]);
                            clearSelected(selectedIndex);
                            redraw();
                        }
                        else
                        {
                            updateSelected(selectedIndex);
                            redraw();
                        }
                    break;
            }        
        }
        else
        {
            audioSource.PlayOneShot(negativeClick);
        }
    }

    public void equip(){
        if (selectedIndex != -1)
        {
            _invertoryLogic.equip(_invertoryLogic._invertoryList[selectedIndex]);
            redraw();
            clearSelected(selectedIndex);
            audioSource.PlayOneShot(wearSound);
        }
        else
        {
            audioSource.PlayOneShot(negativeClick);
        }

    }

    public void toCombine(){
        if(selectedIndex != -1)
        {
            _invertoryLogic.toCombineList(selectedIndex);
            redraw();
            clearSelected(selectedIndex);
        }
        else
        {
            audioSource.PlayOneShot(negativeClick);
        }
    }

    public void cancelCombination(){
        _invertoryLogic.clearCombination();
        redraw();
    }

    public void combine(){
        _invertoryLogic.combine();
        redraw();
    }

    public void unequipCloth(int index){
        _invertoryLogic.unequipCloth(index);
        redraw();
    }

    public void unequipTool(int index){
        _invertoryLogic.unequipTool(index);
        redraw();
    }
}

