using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveToJson : MonoBehaviour
{
    [SerializeField] Invertory_logic inv_logic;
    public void Save()
    {
        PlayerData playerData = new PlayerData();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject playerScripts = player.transform.GetChild(3).gameObject;
        playerData.position = player.transform.position;

        for (int i = 0; i < playerScripts.GetComponent<Invertory_logic>()._invertoryList.Count; i++)
        {
            playerData.inventoryList.Add(playerScripts.GetComponent<Invertory_logic>()._invertoryList[i]);
        }

        for (int i = 0; i < playerScripts.GetComponent<Invertory_logic>()._clothList.Length; i++)
        {
            playerData.clothArray[i] = playerScripts.GetComponent<Invertory_logic>()._clothList[i];
        }

        for (int i = 0; i < playerScripts.GetComponent<Invertory_logic>()._combinationList.Length; i++)
        {
            playerData.combinationArray[i] = playerScripts.GetComponent<Invertory_logic>()._combinationList[i];
        }

        string data = JsonUtility.ToJson(playerData);
        string filePath = Application.persistentDataPath + "/TLDsave.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, data);
        Debug.Log("Saved");
    }

    public void Load()
    {
        inv_logic.clearInventory();
        string filePath = Application.persistentDataPath + "/TLDsave.json";
        string data = System.IO.File.ReadAllText(filePath);
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(data);

        if (playerData != null)
        {
            
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject playerScripts = player.transform.GetChild(3).gameObject;
            player.transform.position = playerData.position;

            for (int i = 0; i < playerData.inventoryList.Count; i++)
            {
                playerScripts.GetComponent<Invertory_logic>()._invertoryList.Add(playerData.inventoryList[i]);
            }

            for (int i = 0; i < playerData.clothArray.Length; i++)
            {
                playerScripts.GetComponent<Invertory_logic>()._clothList[i] = playerData.clothArray[i];
            }

            for (int i = 0; i < playerData.combinationArray.Length; i++)
            {
                playerScripts.GetComponent<Invertory_logic>()._combinationList[i] = playerData.combinationArray[i];
            }
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public Vector3 position;
    public List<Item_so> inventoryList = new List<Item_so>();
    public Item_so[] clothArray = new Item_so[] { null, null, null, null };
    public Item_so[] combinationArray = new Item_so[] { null, null };
}