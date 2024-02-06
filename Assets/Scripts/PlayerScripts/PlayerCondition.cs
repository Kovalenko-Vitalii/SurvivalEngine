using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerCondition : MonoBehaviour
{
    public PlayerMovement playerMovement;

    [Header("Health settings")]
    public float playerHealth;
    public float playerMaxHealth;
    public float healthSpeed;

    [Header("Hunger settings")]
    public float playerHunger;
    public float playerMaxHunger;
    public float hungerSpeed;

    [Header("Hydration settings")]
    public float playerHydration;
    public float playerMaxHydration;
    public float hydrationSpeed;

    [Header("Temperature settings")]
    public float playerTemperature;
    public float playerMaxTemperature;
    public float temperatureSpeed;

    [Header("Weight settings")]
    public float playerCarryingWeight;
    public float playerMaxCarryingWeight;

    [Header("Stamina settings")]
    public float playerStamina;
    public float playerMaxStamina;
    public float staminaRunCost;
    public float staminaJumpCost;
    public float staminaReganSpeed;

    public bool staminaIsRunning;
    public Coroutine recharge;

    float walkingSpeed;
    float sprintSpeed;
    float jumpForce;
    float jumpCooldown;

    private void Start(){
        walkingSpeed = playerMovement.walkSpeed;
        sprintSpeed = playerMovement.sprintSpeed;
        jumpForce = playerMovement.jumpForce;
        jumpCooldown = playerMovement.jumpCooldown;

        staminaIsRunning = false;
    }

    public void FixedUpdate(){
        changingStats();
        manipulatingStamina();
    }

    private void changingStats(){
        playerTemperature -= temperatureSpeed * Time.deltaTime;
        playerHunger -= hungerSpeed * Time.deltaTime;
        playerHydration -= hydrationSpeed * Time.deltaTime;
        playerHealth -= healthSpeed * Time.deltaTime;

        //=== ALERT === Very bad code below. If you can contact me and know how to rewrite it better - do it! === ALERT ===
        //[BTW i do not think that all my code is good, I am just learning 29.05.2023 40:43]
        //07.08.2023 it is still trash I am too lazy to clean it xd

        if (playerTemperature > playerMaxTemperature){ 
            playerTemperature = playerMaxTemperature;
        }

        if (playerHunger > playerMaxHunger){ 
            playerHunger = playerMaxHunger;
        }

        if (playerHealth > playerMaxHealth){ 
            playerHealth = playerMaxHealth;
        }
        
        if (playerHydration > playerMaxHydration){ 
            playerHydration = playerMaxHydration;
        }   

        if (playerTemperature < 0){
            playerTemperature = 0;
        }

        if (playerHunger < 0){
            playerHunger = 0;
        }

        if (playerHealth < 0){
            playerHealth = 0;
            SceneManager.LoadScene(0);
        }

        if (playerHydration < 0){
            playerHydration = 0;
        }

        playerMovement.walkSpeed = walkingSpeed - ((playerCarryingWeight / playerMaxCarryingWeight));
        playerMovement.sprintSpeed = sprintSpeed - ((playerCarryingWeight / playerMaxCarryingWeight) * 2f);
        playerMovement.jumpForce = jumpForce - ((playerCarryingWeight / playerMaxCarryingWeight));
        playerMovement.jumpCooldown = jumpCooldown + ((playerCarryingWeight / playerMaxCarryingWeight) / 4);
    }

    private void manipulatingStamina()
    {
        
    }

    public float getPlayerHealth(){
        return playerHealth;
    }

    public float getPlayerMaxHealth(){
        return playerMaxHealth;
    }

    public float getPlayerHunger(){
        return playerHunger;
    }

    public float getPlayerMaxHunger(){
        return playerMaxHunger;
    }

    public float getPlayerHydration(){
        return playerHydration;
    }

    public float getPlayerMaxHydration(){
        return playerMaxHydration;
    }

    public float getPlayerTemperature(){
        return playerTemperature;
    }

    public float getPlayerMaxTemperature(){
        return playerMaxTemperature;
    }

    public float getPlayerCarryingWeight(){
        return playerCarryingWeight;
    }  

    public void setPlayerCarryingWeight(float weight){
        playerCarryingWeight = weight;
    }

    public void clearWeight(){
        playerCarryingWeight = 0;
    }

    public float getPlayerMaxCarryingWeight(){
        return playerMaxCarryingWeight;
    }

    public float GetRemainingHungerCapacity()
    {
        return playerMaxHunger - playerHunger;
    }

    public float GetRemainingHydrationCapacity()
    {
        return playerMaxHydration - playerHydration;
    }

    public IEnumerator rechargeStamina()
    {
        yield return new WaitForSeconds(2f);

        while(playerStamina < playerMaxStamina)
        {
            playerStamina += staminaReganSpeed;
            if (playerStamina > playerMaxStamina) playerStamina = playerMaxStamina;
            yield return new WaitForSeconds(0.01f);
        }
        staminaIsRunning = false;
    }
}
