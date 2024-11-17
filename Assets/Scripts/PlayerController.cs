using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Controls player movement, health, combat, and interactions
public class PlayerController : MonoBehaviour
{
    // Movement variables
    public InputAction MoveAction;
    public float speed = 3.0f;
    private Rigidbody2D rigidbody2d;
    private Vector2 move;
    private Vector2 moveDirection = new Vector2(1, 0);

    // Health system
    public int maxHealth = 5;
    private int currentHealth;
    public int health { get { return currentHealth; } }
    public float timeInvincible = 2.0f;
    private bool isInvincible;
    private float damageCooldown;

    // Animation and combat
    private Animator animator;
    public GameObject projectilePrefab;
    public InputAction launchAction;
    public InputAction talkAction;
    public BloodOverlayController bloodOverlay;

    // Audio
    private AudioSource audioSource;

    // Initialization
    void Start()
    {
        MoveAction.Enable();
        launchAction.Enable();
        launchAction.performed += Launch;

        talkAction.Enable();
        talkAction.performed += FindFriend;

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
    }

    void OnEnable()
    {
        if (launchAction != null)
        {
            launchAction.performed += Launch;
        }
    }

    void OnDisable()
    {
        if (launchAction != null)
        {
            launchAction.performed -= Launch;
        }
    }

    // Handles player movement and invincibility cooldown
    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            move = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

        if (move.sqrMagnitude > 0)
        {
            moveDirection = move.normalized;
        }

        animator.SetFloat("Move X", moveDirection.x);
        animator.SetFloat("Move Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
                isInvincible = false;
        }
    }

    // Updates physics and movement
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    // Change player's health and handle damage
    public void ChangeHealth(int amount)
    {
        if (amount < 0 && isInvincible)
            return;

        if (amount < 0)
        {
            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");

            if (bloodOverlay != null)
            {
                bloodOverlay.ShowBloodEffect();
            }
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue((float)currentHealth / maxHealth);
    }

    // Launches a projectile
    void Launch(InputAction.CallbackContext context)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;
        Vector2 launchDirection = (mousePosition - transform.position).normalized;

        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(launchDirection, 300);

        animator.SetTrigger("Launch");
    }

    // Interacts with NPCs
    void FindFriend(InputAction.CallbackContext context)
    {
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("NPC"));
        if (hit.collider != null)
        {
            NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
            if (character != null)
            {
                UIHandler.instance.DisplayDialogue();
            }
        }
    }

    // Play sound effects
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
