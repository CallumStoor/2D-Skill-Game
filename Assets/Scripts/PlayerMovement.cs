using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speed")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private float staminaDrainRate = 1f;
    [SerializeField] private float staminaRegenRate = 1f;
    [SerializeField] private float staminaRegenDelay = 1f;

    public float currentStamina { get; private set; }
    private float staminaRegenTimer;

    private bool isSprinting;
    private bool sprintHeld;

    private Rigidbody2D rb;
    private Vector2 moveInput;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        HandleStamina(); 
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * GetMoveSpeed();
    }

    // Movement Logic //

    private float GetMoveSpeed()
    {
        //checks if can sprint and is moving to increase speed
        if (isSprinting && currentStamina > 0f && moveInput != Vector2.zero)
        {
            return moveSpeed * sprintMultiplier;
        }
        //otherwise use base speed
        return moveSpeed;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Stamina Logic // 

    public void OnSprint(InputAction.CallbackContext context)
    {
        //checks if the button is being held
        sprintHeld = context.ReadValueAsButton();

        if (context.performed && currentStamina > 0f)
        {
            isSprinting = true;
        }

        //resets the stamina delay so that when the player lets go of sprint it starts the delay
        if (context.canceled)
        {
            isSprinting = false;
            staminaRegenTimer = staminaRegenDelay;
        }
    }

    private void HandleStamina()
    {
        // checks if sprint is avalible 
        if (isSprinting && currentStamina > 0f && moveInput != Vector2.zero )
        {
            currentStamina -= staminaDrainRate * Time.deltaTime; // decrease stamina

            // stop sprint if stamina reaches below 0 and changes sprint bool
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isSprinting = false;
            }
        }

        // checks if sprint is not being held, starts a delay before increasing stamina
        if (!sprintHeld)
        {
            if (staminaRegenTimer > 0f)
            {
                staminaRegenTimer -= Time.deltaTime;
            }
            else
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
            }
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        Debug.Log("Stamina: " + currentStamina + "/" + maxStamina);
    }

}