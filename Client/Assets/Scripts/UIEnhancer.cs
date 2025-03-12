/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UI Enhancer that applies our modern UI styling to existing UI elements.
/// Attaches to the GameManager object to enhance the entire UI.
/// </summary>
[RequireComponent(typeof(EnhancedUIManager))]
public class UIEnhancer : MonoBehaviour 
{
    private EnhancedUIManager uiManager;
    
    [Header("Main UI References")]
    public Canvas mainCanvas;
    public Transform[] panelsToEnhance;
    
    [Header("Enhancement Settings")]
    public bool applyFontChanges = true;
    public bool applyColorScheme = true;
    public bool applyAnimations = true;
    public bool enhanceButtons = true;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get reference to UI Manager
        uiManager = GetComponent<EnhancedUIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIEnhancer requires EnhancedUIManager component!");
            return;
        }
        
        // If no panels specified, try to find them from the GameManager
        if (panelsToEnhance == null || panelsToEnhance.Length == 0)
        {
            GameManager gm = GetComponent<GameManager>();
            if (gm != null)
            {
                List<Transform> panels = new List<Transform>();
                
                // Add relevant panels from GameManager
                if (gm.panel_Main != null) panels.Add(gm.panel_Main.transform);
                if (gm.panel_Lobby != null) panels.Add(gm.panel_Lobby.transform);
                if (gm.panel_Game != null) panels.Add(gm.panel_Game.transform);
                if (gm.panel_Heroes != null) panels.Add(gm.panel_Heroes.transform);
                if (gm.panel_Skills != null) panels.Add(gm.panel_Skills.transform);
                if (gm.panel_Rankings != null) panels.Add(gm.panel_Rankings.transform);
                
                panelsToEnhance = panels.ToArray();
            }
        }
        
        // Start enhancing the UI
        EnhanceAllUI();
    }
    
    public void EnhanceAllUI()
    {
        if (panelsToEnhance == null) return;
        
        foreach (Transform panel in panelsToEnhance)
        {
            if (panel != null)
            {
                EnhancePanel(panel);
            }
        }
        
        // Also enhance prefabs
        EnhancePrefabs();
    }
    
    private void EnhancePanel(Transform panel)
    {
        // Apply fonts to all Text components
        if (applyFontChanges)
        {
            Text[] texts = panel.GetComponentsInChildren<Text>(true);
            foreach (Text text in texts)
            {
                EnhanceText(text);
            }
        }
        
        // Enhance all buttons
        if (enhanceButtons)
        {
            Button[] buttons = panel.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                uiManager.EnhanceButton(button);
                
                // Also enhance the button text
                Text buttonText = button.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    EnhanceText(buttonText, "button");
                }
                
                // Apply color scheme to button images
                if (applyColorScheme)
                {
                    Image buttonImage = button.GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        // Apply slightly different colors based on interactable state
                        if (button.interactable)
                        {
                            uiManager.ApplyThemeColor(buttonImage, "primary");
                        }
                        else
                        {
                            uiManager.ApplyThemeColor(buttonImage, "neutral");
                        }
                    }
                }
            }
        }
        
        // Add fade transitions to panels with CanvasGroup
        if (applyAnimations)
        {
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
            }
            
            // Add panel transition component if it doesn't exist
            EnhancedUIVisibility enhancedVisibility = panel.GetComponent<EnhancedUIVisibility>();
            if (enhancedVisibility == null)
            {
                UIVisibility oldVisibility = panel.GetComponent<UIVisibility>();
                if (oldVisibility != null)
                {
                    // Replace old visibility system with enhanced one
                    enhancedVisibility = panel.gameObject.AddComponent<EnhancedUIVisibility>();
                    enhancedVisibility.visibilityKey = oldVisibility.visibilityKey;
                    Destroy(oldVisibility);
                }
            }
        }
    }
    
    private void EnhanceText(Text text, string style = "normal")
    {
        if (text == null) return;
        
        // Determine text style based on context or size
        string textStyle = style;
        if (style == "normal")
        {
            // Auto-detect style based on existing properties
            if (text.fontSize >= 24 || text.fontStyle == FontStyle.Bold)
            {
                textStyle = "title";
            }
            else if (text.fontSize <= 12)
            {
                textStyle = "small";
            }
        }
        
        // Apply font settings
        uiManager.ApplyFontSettings(text, textStyle);
        
        // Add outline component for better readability if it doesn't exist
        Outline outline = text.GetComponent<Outline>();
        if (outline == null && textStyle == "title")
        {
            outline = text.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0, 0, 0, 0.5f);
            outline.effectDistance = new Vector2(1, -1);
        }
    }
    
    private void EnhancePrefabs()
    {
        GameManager gm = GetComponent<GameManager>();
        if (gm == null) return;
        
        // Enhance hero prefab
        if (gm.heroPrefab != null)
        {
            EnhancePanel(gm.heroPrefab);
        }
        
        // Enhance skill item
        if (gm.skillItem != null)
        {
            EnhancePanel(gm.skillItem);
        }
        
        // Enhance level prefab
        if (gm.level_Prefab != null)
        {
            EnhancePanel(gm.level_Prefab);
        }
        
        // Enhance score prefabs
        if (gm.score_playerPrefab != null)
        {
            EnhancePanel(gm.score_playerPrefab);
        }
        
        if (gm.score_teamPrefab != null)
        {
            EnhancePanel(gm.score_teamPrefab);
        }
    }
} 