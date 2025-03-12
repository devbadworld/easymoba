/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Enhanced UI Initializer that sets up all enhanced UI components when the game starts.
/// </summary>
public class EnhancedUIInitializer : MonoBehaviour 
{
    [Header("Manager References")]
    public GameObject enhancedUIManagerPrefab;
    public GameObject notificationSystemPrefab;
    public GameObject tooltipSystemPrefab;
    
    [Header("Button Enhancement")]
    public bool enhanceAllButtons = true;
    public List<Button> specificButtonsToEnhance;
    
    [Header("UI Panel Enhancements")]
    public bool updateAllPanels = true;
    public List<UIVisibility> specificPanelsToUpdate;
    
    [Header("Notification Prefab")]
    public GameObject notificationPrefab;
    
    [Header("Tooltip Prefab")]
    public GameObject tooltipPrefab;
    
    private EnhancedUIManager uiManager;
    private NotificationSystem notificationSystem;
    private ModernTooltipSystem tooltipSystem;
    
    void Awake()
    {
        // Create UI Manager if it doesn't exist
        if (EnhancedUIManager.instance == null && enhancedUIManagerPrefab != null)
        {
            GameObject managerObj = Instantiate(enhancedUIManagerPrefab);
            managerObj.name = "EnhancedUIManager";
            DontDestroyOnLoad(managerObj);
            uiManager = managerObj.GetComponent<EnhancedUIManager>();
        }
        else
        {
            uiManager = EnhancedUIManager.instance;
        }
        
        // Create Notification System if it doesn't exist
        if (NotificationSystem.instance == null && notificationSystemPrefab != null)
        {
            GameObject notificationObj = Instantiate(notificationSystemPrefab);
            notificationObj.name = "NotificationSystem";
            notificationSystem = notificationObj.GetComponent<NotificationSystem>();
            
            // Set notification prefab
            if (notificationPrefab != null && notificationSystem != null)
            {
                notificationSystem.notificationPrefab = notificationPrefab;
            }
        }
        else
        {
            notificationSystem = NotificationSystem.instance;
        }
        
        // Create Tooltip System if it doesn't exist
        if (ModernTooltipSystem.instance == null && tooltipSystemPrefab != null)
        {
            GameObject tooltipObj = Instantiate(tooltipSystemPrefab);
            tooltipObj.name = "TooltipSystem";
            tooltipSystem = tooltipObj.GetComponent<ModernTooltipSystem>();
            
            // Set tooltip prefab
            if (tooltipPrefab != null && tooltipSystem != null)
            {
                tooltipSystem.tooltipPrefab = tooltipPrefab;
                tooltipSystem.canvasRect = FindMainCanvasRectTransform();
            }
        }
        else
        {
            tooltipSystem = ModernTooltipSystem.instance;
        }
    }
    
    void Start()
    {
        // After all components are initialized, start enhancing UI
        StartCoroutine(EnhanceUI());
    }
    
    /// <summary>
    /// Find the main canvas RectTransform
    /// </summary>
    private RectTransform FindMainCanvasRectTransform()
    {
        // Try to find via GameManager
        if (GameManager.singleton != null && GameManager.singleton.canvas != null)
        {
            return GameManager.singleton.canvas as RectTransform;
        }
        
        // Fallback to finding any canvas
        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas != null)
        {
            return mainCanvas.GetComponent<RectTransform>();
        }
        
        return null;
    }
    
    /// <summary>
    /// Enhance all UI components
    /// </summary>
    private IEnumerator EnhanceUI()
    {
        // Wait one frame to ensure all UI components are properly initialized
        yield return null;
        
        // Enhance buttons
        if (enhanceAllButtons)
        {
            EnhanceAllButtons();
        }
        else if (specificButtonsToEnhance != null && specificButtonsToEnhance.Count > 0)
        {
            foreach (Button button in specificButtonsToEnhance)
            {
                if (button != null)
                {
                    EnhanceButton(button);
                }
            }
        }
        
        // Update UI panels
        if (updateAllPanels)
        {
            UpdateAllPanels();
        }
        else if (specificPanelsToUpdate != null && specificPanelsToUpdate.Count > 0)
        {
            foreach (UIVisibility panel in specificPanelsToUpdate)
            {
                if (panel != null)
                {
                    UpdatePanel(panel);
                }
            }
        }
        
        // Add tooltips to skill items
        AddTooltipsToSkillItems();
        
        // Test notification
        if (notificationSystem != null)
        {
            notificationSystem.ShowInfo("UI Enhancements Initialized", 3f);
        }
    }
    
    /// <summary>
    /// Enhance all buttons in the scene
    /// </summary>
    private void EnhanceAllButtons()
    {
        Button[] allButtons = FindObjectsOfType<Button>();
        
        foreach (Button button in allButtons)
        {
            EnhanceButton(button);
        }
    }
    
    /// <summary>
    /// Enhance a specific button
    /// </summary>
    private void EnhanceButton(Button button)
    {
        if (button == null) return;
        
        // Skip if already enhanced
        if (button.GetComponent<ModernUIButton>() != null) return;
        
        // Add ModernUIButton component
        ModernUIButton modernButton = button.gameObject.AddComponent<ModernUIButton>();
        
        // Apply theme color if UI Manager exists
        if (uiManager != null)
        {
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                uiManager.ApplyThemeColor(buttonImage, "primary");
            }
            
            // Initialize the enhanced button
            uiManager.EnhanceButton(button);
        }
    }
    
    /// <summary>
    /// Update all UI panels with enhanced animations
    /// </summary>
    private void UpdateAllPanels()
    {
        UIVisibility[] allPanels = FindObjectsOfType<UIVisibility>();
        
        foreach (UIVisibility panel in allPanels)
        {
            UpdatePanel(panel);
        }
    }
    
    /// <summary>
    /// Update a specific panel with enhanced animations
    /// </summary>
    private void UpdatePanel(UIVisibility panel)
    {
        if (panel == null) return;
        
        // Skip if already enhanced
        if (panel.gameObject.GetComponent<EnhancedUIVisibility>() != null) return;
        
        // Add EnhancedUIVisibility component
        EnhancedUIVisibility enhancedPanel = panel.gameObject.AddComponent<EnhancedUIVisibility>();
        
        // Copy properties from original UIVisibility
        enhancedPanel.showOnStart = panel.showOnStart;
        enhancedPanel.destroyOnHide = panel.destroyOnHide;
        
        // Set a default animation
        enhancedPanel.animationType = EnhancedUIVisibility.AnimationType.FadeAndScale;
        enhancedPanel.animationDuration = 0.3f;
        
        // Disable original UIVisibility but keep it for compatibility
        panel.enabled = false;
    }
    
    /// <summary>
    /// Add tooltips to skill items
    /// </summary>
    private void AddTooltipsToSkillItems()
    {
        UISkillItem[] skillItems = FindObjectsOfType<UISkillItem>();
        
        foreach (UISkillItem skillItem in skillItems)
        {
            if (skillItem == null) continue;
            
            // Skip if already has tooltip
            if (skillItem.GetComponent<TooltipTrigger>() != null) continue;
            
            // Add tooltip trigger
            TooltipTrigger tooltip = skillItem.gameObject.AddComponent<TooltipTrigger>();
            
            // Set default tooltip content (this would be replaced with actual skill info)
            tooltip.tooltipTitle = "Skill";
            tooltip.tooltipContent = "This skill description will be populated at runtime.";
            tooltip.showDelay = 0.3f;
        }
        
        // Upgrade skill items to EnhancedSkillItems if desired
        UpdateSkillItems();
    }
    
    /// <summary>
    /// Update skill items with enhanced visuals
    /// </summary>
    private void UpdateSkillItems()
    {
        UISkillItem[] skillItems = FindObjectsOfType<UISkillItem>();
        
        foreach (UISkillItem skillItem in skillItems)
        {
            if (skillItem == null) continue;
            
            // Skip if it's already an EnhancedSkillItem
            if (skillItem is EnhancedSkillItem) continue;
            
            // We can't directly replace the component, so we'll save the relevant data
            // and use the EnhancedSkillItem for new skill items
        }
    }
    
    /// <summary>
    /// Add a notification before scene changes
    /// </summary>
    public void OnSceneWillChange(string sceneName)
    {
        if (notificationSystem != null)
        {
            notificationSystem.ShowInfo("Loading " + sceneName + "...", 2f);
        }
    }
} 