using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Medicne", menuName = "ScriptableObjects/Invertory/Medicine")]
public class Medicine : Item_so
{
    [Header("Bites")]
    public int initialBites;
    public int bites;
    [Header("Stats")]
    public float healthPoints;
    public bool eaten = false;
    public AudioClip useSound;

    public void eat()
    {
        PlayerCondition playerCondition = GameObject.FindGameObjectWithTag("PlayerScripts").GetComponent<PlayerCondition>();
        if (playerCondition != null)
        {
            playerCondition.playerHealth += healthPoints;
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
