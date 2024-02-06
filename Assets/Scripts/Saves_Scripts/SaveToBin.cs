using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveToBin : MonoBehaviour{
    [SerializeField] Invertory_logic inv_logic;

    private void Update(){
        if (Input.GetKeyDown(KeyCode.F9)){
            save();
        }

        if (Input.GetKeyDown(KeyCode.F10)){
            load();
        }
    }

    [System.Serializable]
    public class playerData {
        public float[] position = new float[3];//Player position

        public float xRotation;
        public float yRotation;

        public string[] itemNames;//InventoryList
        public int[] itemAmounts;//InventoryList
        public int[] itemMagazRoundsIn;//InventoryList magaz roundsIn

        public string[] clothNames;//ClothArray
        public int[] clothAmounts;//ClothArray

        public string[] combinationNames;//CombinationArray
        public int[] combinationAmounts;//CombinationArray
        public int[] combinationMagazRoundsIn;//CombinationArray magaz roundsIn
    }

    [System.Serializable]
    public class worldData{
        public float[,] itemsPos;
        public string[] itemsNames;
    }

    void clearItems(){
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject item in items){
            Destroy(item);
        }
    }

    public void save(){
        savePlayerData();
        saveWorldData();
    }

    public void load(){
        loadPlayerData();
        loadWorldData();
    }

    void saveWorldData(){
        //############################################################ SAVING WORLD DATA ###############################################

        worldData worldData = new worldData();

        GameObject[] itemsOnScene = new GameObject[GameObject.FindGameObjectsWithTag("Item").Length];
        itemsOnScene = GameObject.FindGameObjectsWithTag("Item");
        foreach (var item in itemsOnScene)
        {
            Debug.Log(item.GetComponent<Ite_so_Holder>().itemSO.prefabName);
        }
        worldData.itemsPos = new float[itemsOnScene.Length, 7];
        worldData.itemsNames = new string[itemsOnScene.Length];


        for (int i = 0; i < itemsOnScene.Length; i++)
        {
            if (itemsOnScene[i] != null)
            {
                worldData.itemsNames[i] = itemsOnScene[i].GetComponent<Ite_so_Holder>().itemSO.prefabName;

                worldData.itemsPos[i, 0] = itemsOnScene[i].transform.position.x;
                worldData.itemsPos[i, 1] = itemsOnScene[i].transform.position.y;
                worldData.itemsPos[i, 2] = itemsOnScene[i].transform.position.z;

                worldData.itemsPos[i, 3] = itemsOnScene[i].transform.rotation.x;
                worldData.itemsPos[i, 4] = itemsOnScene[i].transform.rotation.y;
                worldData.itemsPos[i, 5] = itemsOnScene[i].transform.rotation.z;
                worldData.itemsPos[i, 6] = itemsOnScene[i].transform.rotation.w;
            }
        }

        //Saving
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/TLDsaveBinWorld.b";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, worldData);
        stream.Close();
    }

    void savePlayerData(){
        //############################################### SAVING PLAYER DATA ###############################################
        playerData playerData = new playerData();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject cameraJoint = GameObject.FindGameObjectWithTag("Camera Joint");
        GameObject playerScripts = player.transform.GetChild(2).gameObject;

        playerData.position[0] = player.transform.position.x;
        playerData.position[1] = player.transform.position.y;
        playerData.position[2] = player.transform.position.z;

        playerData.xRotation = cameraJoint.GetComponent<PlayerCam>().xRotation;
        playerData.yRotation = cameraJoint.GetComponent<PlayerCam>().yRotation;

        playerData.itemNames = new string[playerScripts.GetComponent<Invertory_logic>()._invertoryList.Count];
        playerData.itemAmounts = new int[playerScripts.GetComponent<Invertory_logic>()._invertoryList.Count];
        playerData.itemMagazRoundsIn = new int[playerScripts.GetComponent<Invertory_logic>()._invertoryList.Count];

        playerData.clothNames = new string[playerScripts.GetComponent<Invertory_logic>()._clothList.Length];
        playerData.clothAmounts = new int[playerScripts.GetComponent<Invertory_logic>()._clothList.Length];

        playerData.combinationNames = new string[playerScripts.GetComponent<Invertory_logic>()._combinationList.Length];
        playerData.combinationAmounts = new int[playerScripts.GetComponent<Invertory_logic>()._combinationList.Length];
        playerData.combinationMagazRoundsIn = new int[playerScripts.GetComponent<Invertory_logic>()._combinationList.Length];


        //=============================== Inventory List ==========================================================

        for (int i = 0; i < playerScripts.GetComponent<Invertory_logic>()._invertoryList.Count; i++){
            if (playerScripts.GetComponent<Invertory_logic>()._invertoryList[i] != null)
            {
                playerData.itemNames[i] = playerScripts.GetComponent<Invertory_logic>()._invertoryList[i].name;
                playerData.itemAmounts[i] = playerScripts.GetComponent<Invertory_logic>()._invertoryList[i].quantity;

                if (playerScripts.GetComponent<Invertory_logic>()._invertoryList[i] is Magazine magaz)
                {
                    playerData.itemMagazRoundsIn[i] = magaz.roundsIn;
                }
                else
                {
                    playerData.itemMagazRoundsIn[i] = -1;
                }
            }
        }

        //================================ Cloth Array ============================================================

        for (int i = 0; i < playerScripts.GetComponent<Invertory_logic>()._clothList.Length; i++){
            if (playerScripts.GetComponent<Invertory_logic>()._clothList[i] != null)
            {
                playerData.clothNames[i] = playerScripts.GetComponent<Invertory_logic>()._clothList[i].name;
                playerData.clothAmounts[i] = playerScripts.GetComponent<Invertory_logic>()._clothList[i].quantity;
            }
        }

        //================================ Combination Array ======================================================

        for (int i = 0; i < playerScripts.GetComponent<Invertory_logic>()._combinationList.Length; i++){
            if (playerScripts.GetComponent<Invertory_logic>()._combinationList[i] != null)
            {
                playerData.combinationNames[i] = playerScripts.GetComponent<Invertory_logic>()._combinationList[i].name;
                playerData.combinationAmounts[i] = playerScripts.GetComponent<Invertory_logic>()._combinationList[i].quantity;

                if (playerScripts.GetComponent<Invertory_logic>()._combinationList[i] is Magazine magaz)
                {
                    playerData.combinationMagazRoundsIn[i] = magaz.roundsIn;
                }
                else
                {
                    playerData.combinationMagazRoundsIn[i] = -1;
                }
            }
        }

        //Saving
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/TLDsaveBinPlayer.b";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, playerData);
        stream.Close();
    }

    void loadPlayerData(){
        string path = Application.persistentDataPath + "/TLDsaveBinPlayer.b";

        playerData data;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject cameraJoint = GameObject.FindGameObjectWithTag("Camera Joint");

        if (File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            data = formatter.Deserialize(stream) as playerData;
            stream.Close();

            inv_logic.clearInventory();

            player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);

            cameraJoint.GetComponent<PlayerCam>().xRotation = data.xRotation;
            cameraJoint.GetComponent<PlayerCam>().yRotation = data.yRotation;

            //============================================ Assigning Inventory List =======================

            for (int i = 0; i < data.itemNames.Length; i++){
                if (data.itemNames[i] != null)
                {
                    Item_so item = Resources.Load<Item_so>($"Items/{data.itemNames[i]}");
                    Item_so copyItem = item.Copy();
                    copyItem.quantity = data.itemAmounts[i];
                    if (copyItem is Magazine magaz)
                    {
                        if (data.itemMagazRoundsIn[i] >= 0)
                        {
                            magaz.roundsIn = data.itemMagazRoundsIn[i];
                        }
                    }
                    inv_logic._invertoryList.Add(copyItem);
                }
            }

            //============================================ Assigning Cloth List ===========================

            for (int i = 0; i < data.clothNames.Length; i++){
                if (data.clothNames[i] != null){
                    Item_so item = Resources.Load<Item_so>($"Items/{data.clothNames[i]}");
                    Item_so copyItem = item.Copy();
                    if (copyItem is Cloth cloth){
                        cloth.quantity = data.clothAmounts[i];
                        inv_logic._clothList[cloth.slotNumber - 1] = cloth;
                    }
                }
            }

            //============================================ Assigning Combination List =====================

            for (int i = 0; i < data.combinationNames.Length; i++){
                if (data.combinationNames[i] != null){
                    Item_so item = Resources.Load<Item_so>($"Items/{data.combinationNames[i]}");
                    Item_so copyItem = item.Copy();

                    copyItem.quantity = data.combinationAmounts[i];
                    inv_logic._combinationList[i] = copyItem;
                    if (copyItem is Magazine magaz){
                        if (data.combinationMagazRoundsIn[i] >= 0){
                            magaz.roundsIn = data.combinationMagazRoundsIn[i];
                        }
                    }
                }
            }
        }
        else{
            Debug.LogError("Player Save file not found in " + path);
            return;
        }

        player.transform.GetChild(2).gameObject.GetComponent<Invertory_UI>().redraw();
        inv_logic.calculateWeight();
    }

    void loadWorldData(){
        string path = Application.persistentDataPath + "/TLDsaveBinWorld.b";

        worldData data;
        if (File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            data = formatter.Deserialize(stream) as worldData;
            stream.Close();

            clearItems();

            for (int i = 0; i < data.itemsNames.Length; i++){
                Vector3 position = new Vector3(data.itemsPos[i,0], data.itemsPos[i, 1], data.itemsPos[i, 2]);
                Quaternion rotation = new Quaternion(data.itemsPos[i, 3], data.itemsPos[i, 4], data.itemsPos[i, 5], data.itemsPos[i, 6]);
                GameObject spawnedItem = Instantiate(Resources.Load<GameObject>($"Item_Prefabs/{data.itemsNames[i]}"), position, rotation); 
            }
        }
        else{
            Debug.LogError("Player Save file not found in " + path);
            return;
        }
    }
}
