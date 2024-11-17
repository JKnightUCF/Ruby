using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

// Manages UI elements such as health bar and NPC dialogue
public class UIHandler : MonoBehaviour
{
    // Public variables
    public float displayTime = 4.0f; // Duration to display NPC dialogue

    // Private variables
    private VisualElement m_Healthbar;        // Health bar UI element
    private VisualElement m_NonPlayerDialogue; // NPC dialogue UI element
    private float m_TimerDisplay;             // Timer to hide dialogue

    // Singleton instance for global access
    public static UIHandler instance { get; private set; }

    // Initialize singleton instance
    private void Awake()
    {
        instance = this;
    }

    // Setup UI elements on start
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();

        // Initialize health bar
        m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        SetHealthValue(1.0f); // Full health initially

        // Initialize NPC dialogue
        m_NonPlayerDialogue = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        m_NonPlayerDialogue.style.display = DisplayStyle.None; // Hidden by default

        m_TimerDisplay = -1.0f; // Timer inactive initially
    }

    // Update health bar based on percentage
    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(100 * percentage);
    }

    // Countdown for hiding dialogue
    private void Update()
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;

            if (m_TimerDisplay < 0)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None; // Hide dialogue
            }
        }
    }

    // Display NPC dialogue and reset timer
    public void DisplayDialogue()
    {
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex; // Show dialogue
        m_TimerDisplay = displayTime; // Reset display timer
    }
}
