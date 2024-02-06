using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Invertory/Gun")]
public class Gun : Tool{
    public float damage;
    public float cooldown;
    public Item_so magazine;
}
