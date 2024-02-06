using System.Collections.Generic;
using UnityEngine;

public class Invertory_logic : MonoBehaviour{
    public List<Item_so> _invertoryList = new List<Item_so>();

    public Item_so[] _clothList = new Item_so[] { null, null, null, null };
    public Item_so[] _combinationList = new Item_so[] { null, null, null };
    public Item_so[] _toolList = new Item_so[] { null, null };

    public Transform dropItemPoint;
    [SerializeField] Invertory_UI _invertoryUI;
    [SerializeField] PlayerCondition playerCondition;
    [SerializeField] PlayerHands playerHands;

    public Item_so equipedItem_so;

    private void Update(){
        selectItem_so();
    }

    //================================ Clear Inventory(only for update) ==============================================

    public void clearInventory()
    {
        _invertoryList.Clear();

        _clothList[0] = null;
        _clothList[1] = null;
        _clothList[2] = null;
        _clothList[3] = null;

        _combinationList[0] = null;
        _combinationList[1] = null;

        playerCondition.clearWeight();
        calculateWeight();
    }

    //================================ Drop Item =====================================================================

    public void dropItem(Item_so _item)
    {
        Vector3 pos = dropItemPoint.position;
        Vector3 desiredmove = dropItemPoint.transform.forward;
        GameObject droppedItem = Instantiate(_item.prefab, pos, Quaternion.identity);
        droppedItem.GetComponent<Ite_so_Holder>().itemSO = _item;
        droppedItem.GetComponent<Rigidbody>().AddForce(desiredmove * 40f);
        _invertoryList.Remove(_item);
        calculateWeight();
    }

    //================================ Add item ======================================================================

    public void pickUpItem(Item_so item)
    {
        if (_invertoryUI.Slots.Count > _invertoryList.Count)
        {
            addItem(item);
            _invertoryUI.redraw();
            calculateWeight();
        }
        else
        {
            stackItem(item);
            _invertoryUI.redraw();
            calculateWeight();
        }
    }

    //==================================== Adding and Stacking Items =================================================
    public void addItem(Item_so _item)
    {
        if (_invertoryList.Count != 0)
        {
            bool itemStacked = false;
            for (int i = 0; i < _invertoryList.Count; i++)
            {
                if (_invertoryList[i].id == _item.id)
                {
                    Item_so existingItem = _invertoryList[i];

                    if (existingItem.maxAmount > existingItem.quantity)
                    {
                        int remainingSpace = existingItem.maxAmount - existingItem.quantity;

                        if (_item.quantity <= remainingSpace)
                        {
                            existingItem.quantity += _item.quantity;
                            itemStacked = true;
                            calculateWeight();
                            break;
                        }
                        else
                        {
                            Item_so item = _item.Copy();
                            int quantityToAdd = remainingSpace;
                            existingItem.quantity += quantityToAdd;
                            item.quantity -= quantityToAdd;
                            _invertoryList.Add(item);
                            itemStacked = true;
                            calculateWeight();
                            break;
                        }
                    }
                }
            }
            if (!itemStacked)
            {
                Item_so newItem = _item.Copy();
                _invertoryList.Add(newItem);
                calculateWeight();
            }
        }
        else
        {
            Item_so newItem = _item.Copy();
            _invertoryList.Add(newItem);
            calculateWeight();
        }
    }

    public void stackItem(Item_so _item)
    {
        for (int i = 0; i < _invertoryList.Count; i++)
        {
            if (_invertoryList[i].id == _item.id)
            {
                Item_so existingItem = _invertoryList[i];

                if (existingItem.maxAmount > existingItem.quantity)
                {
                    int remainingSpace = existingItem.maxAmount - existingItem.quantity;

                    if (_item.quantity <= remainingSpace)
                    {
                        existingItem.quantity += _item.quantity;
                        calculateWeight();
                        break;
                    }
                    else
                    {
                        Item_so item = _item.Copy();
                        int quantityToAdd = remainingSpace;
                        existingItem.quantity += quantityToAdd;
                        item.quantity -= quantityToAdd;
                        dropItem(item);
                        calculateWeight();
                        break;
                    }
                }
            }
        }
    }

    //========================================== Equip method ===============================================

    public void equip(Item_so itemToEquip)
    {
        if (itemToEquip is Cloth cloth && cloth.slotNumber - 1 <= 3 && _clothList[cloth.slotNumber - 1] == null)
        {
            _clothList[cloth.slotNumber - 1] = cloth;
            _invertoryList.Remove(itemToEquip);
            calculateWeight();
        }
        else if (itemToEquip.GetType().IsSubclassOf(typeof(Tool)))
        {
            if(_toolList[0] == null)
            {
                _toolList[0] = itemToEquip;
                _invertoryList.Remove(itemToEquip);
                calculateWeight();
            }
            else if (_toolList[1] == null)
            {
                _toolList[1] = itemToEquip;
                _invertoryList.Remove(itemToEquip);
                calculateWeight();
            }
        }
    }

    //========================================== Cloth methods ===============================================

    public void unequipCloth(int index)
    {
        if (_invertoryList.Count < 12 && _clothList[index] != null)
        {
            _invertoryList.Add(_clothList[index]);
            _clothList[index] = null;
            calculateWeight();
        }
        /*
        else
        {
            dropFromClothList(index);
            calculateWeight();
        }
        */
    }

    public void dropFromClothList(int index)
    {
        _clothList[index] = null;
        calculateWeight();
    }

    //=================================== Tools Methods ====================================================

    public void unequipTool(int index)
    {
        if (_invertoryList.Count < 12 && _toolList[index] != null)
        {
            _invertoryList.Add(_toolList[index]);
            _toolList[index] = null;
            calculateWeight();
        }
        else
        {
            _toolList[index] = null;
            calculateWeight();
        }

        equipedItem_so = null; // Reset the equipped item variable
    }

    //=================================== Combination methods ==============================================

    public void toCombineList(int index)
    {
        
        if (_combinationList[0] == null)
        {       
            _combinationList[0] = _invertoryList[index];
            _invertoryList.Remove(_invertoryList[index]);
            calculateWeight();
        }
        else if (_combinationList[0] != null)
        {
            _combinationList[1] = _invertoryList[index];
            _invertoryList.Remove(_invertoryList[index]);
            calculateWeight();
        }
        else
        {
            Debug.Log("Combination slot is full");
            calculateWeight();
        }
    }

    public void clearCombination()
    {
        if(_combinationList[0] != null)
        {
            if (_invertoryUI.Slots.Count > _invertoryList.Count)
            {
                addItem(_combinationList[0]);
                _invertoryUI.redraw();
            }
            else
            {
                stackItem(_combinationList[0]);
                _invertoryUI.redraw();
            }
        }

        if (_combinationList[1] != null)
        {
            if (_invertoryUI.Slots.Count > _invertoryList.Count)
            {
                addItem(_combinationList[1]);
                _invertoryUI.redraw();
            }
            else
            {
                stackItem(_combinationList[1]);
                _invertoryUI.redraw();
            }
        }

        _combinationList[0] = null;
        _combinationList[1] = null;
        calculateWeight();
    }   

    public void combine()
    {
        if(!loadMagazine(0, 1))
        {
            if(!loadMagazineReversed(0, 1))
            {
                Debug.Log("CHTO-TO NE TO");
            }
        }
        calculateWeight();
    }

    //=================================== Selecting inHands methods ========================================

    void selectItem_so(){
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("111111");
            equipedItem_so = _toolList[0];
            playerHands.changeItemInHand(equipedItem_so);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("222222");
            equipedItem_so = _toolList[1];
            playerHands.changeItemInHand(equipedItem_so);
        }
    }

    //================================ Combining/Carfting methods ==========================================

    public bool loadMagazine(int ammoIndex, int magazIndex)
    {
        if (_combinationList[ammoIndex] is Ammo ammo && _combinationList[magazIndex] is Magazine magazine)
        {
            if (magazine.caliber == ammo.caliber && magazine.maxCapacity > magazine.roundsIn)
            {
                int remainingSpace = magazine.maxCapacity - magazine.roundsIn;
                if (remainingSpace >= ammo.quantity)
                {
                    magazine.roundsIn += ammo.quantity;
                    for (int i = 0; i < _combinationList.Length; i++)
                    {
                        if (_combinationList[i] is Ammo)
                        {
                            _combinationList[i] = null;
                        }
                    }
                    clearCombination();
                    calculateWeight();
                    return true;
                }
                else
                {
                    int quantityToAdd = remainingSpace;
                    magazine.roundsIn += quantityToAdd;
                    ammo.quantity -= quantityToAdd;
                    clearCombination();
                    calculateWeight();
                    return true;
                }
            }
            calculateWeight();
            return false;
        }
        calculateWeight();
        return false;
    }

    public bool loadMagazineReversed(int magazIndex, int ammoIndex)
    {
        if (_combinationList[magazIndex] is Magazine magazine && _combinationList[ammoIndex] is Ammo ammo)
        {
            if (magazine.caliber == ammo.caliber && magazine.maxCapacity > magazine.roundsIn)
            {
                int remainingSpace = magazine.maxCapacity - magazine.roundsIn;
                if (remainingSpace >= ammo.quantity)
                {
                    magazine.roundsIn += ammo.quantity;
                    for (int i = 0; i < _combinationList.Length; i++)
                    {
                        if (_combinationList[i] is Ammo)
                        {
                            _combinationList[i] = null;
                        }
                    }
                    clearCombination();
                    calculateWeight();
                    return true;
                }
                else
                {
                    int quantityToAdd = remainingSpace;
                    magazine.roundsIn += quantityToAdd;
                    ammo.quantity -= quantityToAdd;
                    clearCombination();
                    calculateWeight();
                    return true;
                }
            }
            calculateWeight();
            return false;
        }
        calculateWeight();
        return false;
    }

    public void calculateWeight()
    {
        float weight = 0;

        for (int i = 0; i < _invertoryList.Count; i++)
        {
            weight += _invertoryList[i].weight * _invertoryList[i].quantity;
        }
        for (int i = 0; i < _clothList.Length; i++)
        {
            if(_clothList[i] != null)
                weight += _clothList[i].weight * _clothList[i].quantity;
        }
        for (int i = 0; i < _combinationList.Length; i++)
        {
            if(_combinationList[i] != null)
                weight += _combinationList[i].weight * _combinationList[i].quantity;
        }
        for (int i = 0; i < _toolList.Length; i++)
        {
            if (_toolList[i] != null)
                weight += _toolList[i].weight * _toolList[i].quantity;
        }

        playerCondition.setPlayerCarryingWeight(weight);
    }
}

