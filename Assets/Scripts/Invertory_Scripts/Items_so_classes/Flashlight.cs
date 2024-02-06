using UnityEngine;

[CreateAssetMenu(fileName = "Flashlight", menuName = "ScriptableObjects/Invertory/Flashlight")]
public class Flashlight : Tool{
    public Battery battery;

    public string getDurability(){
        if(battery != null){
            return battery.charge.ToString();
        }
        else{
            return "No Battery";
        }
    }
}
