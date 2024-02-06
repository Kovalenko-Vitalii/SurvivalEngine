using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_Manager : MonoBehaviour
{
    [Header("Links")]
    public PlayerCondition playerCondition;
    public Invertory_logic inventoryLogic;

    [Header("For debugging")]
    public TMP_Text statsDebug;

    [Header("Links to stats bars")]
    public Image healthBar;
    public Image hungerBar;
    public Image hydrationBar;
    public Image temperatureBar;
    public Image weightBar;

    [Header("Stamina indication links")]
    public Image staminaBarR;
    public Image staminaBarL;
    public Image staminaBackground;

    [Header("Text for weight")]
    public TMP_Text weightText;

    [Header("VFX for freezing and damage")]
    public Image temperatureScreen;
    public Image healthScreen;

    private bool isStaminaAlphaEnabled = false;

    [Header("Speed of disabling ad enabling stamina indidcation")]
    public float durationStaminaAlpha;

    [Header("Tool slots images")]
    public Image[] ToolSlots = new Image[2];

    private void Start()
    {
        Color targetColor = new Color(staminaBarL.color.r, staminaBarL.color.g, staminaBarL.color.b, 0f); // Target color with alpha = 0

        staminaBarL.color = new Color(staminaBarL.color.r, staminaBarL.color.g, staminaBarL.color.b, 0f);
        staminaBarR.color = new Color(staminaBarR.color.r, staminaBarR.color.g, staminaBarR.color.b, 0f);
        staminaBackground.color = new Color(staminaBackground.color.r, staminaBackground.color.g, staminaBackground.color.b, 0f);
    }
    public void Update()
    {
        refreshStats();
        refreshTools();
        if (playerCondition.playerStamina < playerCondition.playerMaxStamina && !isStaminaAlphaEnabled)
        {
            isStaminaAlphaEnabled = true;
            StartCoroutine(DisableStaminaAlpha());
        }
        else if (playerCondition.playerStamina >= playerCondition.playerMaxStamina && isStaminaAlphaEnabled)
        {
            isStaminaAlphaEnabled = false;
            StartCoroutine(EnableStaminaAlpha());
        }
    }

    void refreshStats()
    {
        statsDebug.text = "Health: " + playerCondition.getPlayerHealth() + "/" + playerCondition.getPlayerMaxHealth() +
                          "Hunger: " + playerCondition.getPlayerHunger() + "/" + playerCondition.getPlayerMaxHunger() +
                          "Hydration: " + playerCondition.getPlayerHydration() + "/" + playerCondition.getPlayerMaxHydration() +
                          "Temperature: " + playerCondition.getPlayerTemperature() + "/" + playerCondition.getPlayerMaxTemperature() +
                          "CarryingWeight: " + playerCondition.getPlayerCarryingWeight() + "/" + playerCondition.getPlayerMaxCarryingWeight();

        healthBar.fillAmount = (float)Math.Round(playerCondition.getPlayerHealth() / playerCondition.getPlayerMaxHealth(), 2);
        hungerBar.fillAmount = (float)Math.Round(playerCondition.getPlayerHunger() / playerCondition.getPlayerMaxHunger(), 2);
        hydrationBar.fillAmount = (float)Math.Round(playerCondition.getPlayerHydration() / playerCondition.getPlayerMaxHydration(), 2);
        temperatureBar.fillAmount = (float)Math.Round(playerCondition.getPlayerTemperature() / playerCondition.getPlayerMaxTemperature(), 2);
        weightBar.fillAmount = playerCondition.getPlayerCarryingWeight() / playerCondition.getPlayerMaxCarryingWeight();

        staminaBarR.fillAmount = playerCondition.playerStamina / playerCondition.playerMaxStamina;
        staminaBarL.fillAmount = playerCondition.playerStamina / playerCondition.playerMaxStamina;

        weightText.text = playerCondition.getPlayerCarryingWeight() + "/" + playerCondition.getPlayerMaxCarryingWeight() + "\n" + "KG";

        temperatureScreen.color = new Color(temperatureScreen.color.r, temperatureScreen.color.g, temperatureScreen.color.b, (100f - (float)playerCondition.playerTemperature) / 200f);
        //Debug.Log(100f - (float)playerCondition.playerTemperature);
        healthScreen.color = new Color(healthScreen.color.r, healthScreen.color.g, healthScreen.color.b, (100f - (float)playerCondition.playerHealth) / 200f);
    }

    void refreshTools()
    {
        for (int i = 0; i < ToolSlots.Length; i++)
        {
            Image buttonImage = ToolSlots[i].transform.GetChild(0).GetComponent<Image>();
            TMP_Text name = ToolSlots[i].transform.GetChild(1).GetComponent<TMP_Text>();
            TMP_Text durability = ToolSlots[i].transform.GetChild(2).GetComponent<TMP_Text>();
            if (inventoryLogic._toolList[i] != null)
            {
                buttonImage.enabled = true;
                ToolSlots[i].enabled = true;
                buttonImage.sprite = inventoryLogic._toolList[i].sprite;
                name.text = inventoryLogic._toolList[i].name;
                switch (inventoryLogic._toolList[i])
                {
                    case Flashlight flashlight:
                        durability.text = flashlight.getDurability().ToString();
                        break;
                }
            }
            else
            {
                buttonImage.sprite = null;
                name.text = "";
                durability.text = "";
                ToolSlots[i].enabled = false;
                buttonImage.enabled = false;
            }
        }
    }

    IEnumerator EnableStaminaAlpha()
    {
        Color startColor = staminaBarL.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Target color with alpha = 0

        float startTime = Time.time;

        while (Time.time < startTime + durationStaminaAlpha)
        {
            float t = (Time.time - startTime) / durationStaminaAlpha; // Calculate interpolation factor
            staminaBarL.color = Color.Lerp(new Color(staminaBarL.color.r, staminaBarL.color.g, staminaBarL.color.b, 1f), 
                new Color(staminaBarL.color.r, staminaBarL.color.g, staminaBarL.color.b, 0f), t); // Interpolate color with updated alpha
            staminaBarR.color = Color.Lerp(new Color(staminaBarR.color.r, staminaBarR.color.g, staminaBarR.color.b, 1f),
                new Color(staminaBarR.color.r, staminaBarR.color.g, staminaBarR.color.b, 0f), t);
            staminaBackground.color = Color.Lerp(new Color(staminaBackground.color.r, staminaBackground.color.g, staminaBackground.color.b, 1f),
                new Color(staminaBackground.color.r, staminaBackground.color.g, staminaBackground.color.b, 0f), t);
            yield return null; // Wait for the next frame
        }

        staminaBarL.color = targetColor; // Ensure final color is set to target
    }
    IEnumerator DisableStaminaAlpha()
    {
        Color startColor = staminaBarL.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f); // Target color with alpha = 1

        float startTime = Time.time;

        while (Time.time < startTime + durationStaminaAlpha)
        {
            float t = (Time.time - startTime) / durationStaminaAlpha; // Calculate interpolation factor
            staminaBarL.color = Color.Lerp(new Color(staminaBarL.color.r, staminaBarL.color.g, staminaBarL.color.b, 0f),
                 new Color(staminaBarL.color.r, staminaBarL.color.g, staminaBarL.color.b, 1f), t); // Interpolate color with updated alpha
            staminaBarR.color = Color.Lerp(new Color(staminaBarR.color.r, staminaBarR.color.g, staminaBarR.color.b, 0f),
                new Color(staminaBarR.color.r, staminaBarR.color.g, staminaBarR.color.b, 1f), t);
            staminaBackground.color = Color.Lerp(new Color(staminaBackground.color.r, staminaBackground.color.g, staminaBackground.color.b, 0f),
                new Color(staminaBackground.color.r, staminaBackground.color.g, staminaBackground.color.b, 1f), t);
            yield return null; // Wait for the next frame
        }

        staminaBarL.color = targetColor; // Ensure final color is set to target
    }
}
