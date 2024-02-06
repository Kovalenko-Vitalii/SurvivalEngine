using UnityEngine;

 public abstract class Item_so : ScriptableObject{
    [SerializeField] public GameObject prefab;
    [SerializeField] public GameObject FPSprefab;
    public Sprite sprite;
    public string name;
    public string prefabName;
    public int quantity;
    public string description;
    public bool selected = false;
    public int maxAmount;
    public float weight;
    public int id;
   
    public Item_so Copy()
    {
        Item_so newItem = Instantiate(this);
        newItem.quantity = this.quantity;
        newItem.name = this.name;
        return newItem;
    }
 }

abstract public class Tool : Item_so{

}


