using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SavesManager : MonoBehaviour
{
    public string savePath = "C:\\TLD_saves\\";

    public class PlayerData
    {
        public Vector3 position;
        public List<Item_so> inventoryList = new List<Item_so>();
        public Item_so[] clothArray = new Item_so[] { null, null, null, null };
        public Item_so[] combinationArray = new Item_so[] { null, null, null };
    }
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

        File.WriteAllText(
            savePath + "jsonWorld.json",
            JsonConvert.SerializeObject(playerData, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            })
        );
    }

    public void Load()
    {
        PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(savePath + "/jsonWorld.json"));

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
