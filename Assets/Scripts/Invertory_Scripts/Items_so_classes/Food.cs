using UnityEngine;

[CreateAssetMenu(fileName = "Food", menuName = "ScriptableObjects/Invertory/Food")]
public class Food : Item_so {
    [Header("Bites")]
    public int initialBites;
    public int bites;
    [Header("Stats")]
    public float calories;
    public float hydration_points;
    public float alco_points;

    public AudioClip useSound;

    public bool eaten = false;
    
    public void eat(){
        PlayerCondition playerCondition = GameObject.FindGameObjectWithTag("PlayerScripts").GetComponent<PlayerCondition>();
        if (playerCondition != null)
        {
            playerCondition.playerHunger += calories;
            playerCondition.playerHydration += hydration_points;
            playerCondition.playerTemperature += alco_points;

            if (bites <= 1)
            {
                eaten = true;
            }
            else
            {
                bites--;
            }
        }
    }
}


