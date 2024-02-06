using UnityEngine;

[CreateAssetMenu(fileName = "Cloth", menuName = "ScriptableObjects/Invertory/Cloth")]
public class Cloth : Item_so{
    public int armor;
    public int wormth;
    public int slotNumber;
    public void wear(){
        Debug.Log("Mister fancy " + name + ". Damn, it looks cool!");
    }
}
