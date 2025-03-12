/*! 
@author UI Enhancement System
@created October 30, 2023
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Runtime UI enhancer that forcefully applies UI enhancements using direct GameObject references.
/// This is a fallback approach when the UIVisibility enhancements aren't being applied.
/// </summary>
[DefaultExecutionOrder(10000)] // Make sure this runs after everything else
public class FallbackUIEnhancer : MonoBehaviour 
{
    // Styling options
    public Color primaryColor = new Color(1f, 0.2f, 0.2f, 1f); // Bright RED to be very obvious
    public Color accentColor = new Color(1f, 0.5f, 0f, 1f);
    
    // Cache for tracking enhanced objects
    private HashSet<string> enhancedObjects = new HashSet<string>();
    private float timeSinceLastCheck = 0;
    private float checkInterval = 2.0f; // Check every 2 seconds
    
    // Debug options
    [Header("Debug")]
    public bool showDebugMessages = true;
    
    void Awake()
    {
        // Important: Don't destroy this object when loading new scenes
        DontDestroyOnLoad(this.gameObject);
        
        // Create a simple version label that will always be visible
        CreateVersionLabel();
        
        if (showDebugMessages)
            Debug.Log("[FallbackUIEnhancer] Initialized and ready to apply UI enhancements");
    }
    
    void Start()
    {
        // Start enhancement coroutine
        StartCoroutine(DelayedEnhancement());
    }
    
    void Update()
    {
        // Periodically check for new UI elements
        timeSinceLastCheck += Time.deltaTime;
        if (timeSinceLastCheck > checkInterval)
        {
            timeSinceLastCheck = 0;
            EnhanceAllUI();
        }
    }
    
    IEnumerator DelayedEnhancement()
    {
        // First attempt at 0.5 seconds
        yield return new WaitForSeconds(0.5f);
        EnhanceAllUI();
        
        // Second attempt at 2 seconds
        yield return new WaitForSeconds(1.5f);
        EnhanceAllUI();
        
        // Third attempt at 5 seconds
        yield return new WaitForSeconds(3.0f);
        EnhanceAllUI();
    }
    
    void CreateVersionLabel()
    {
        // Find the main canvas
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        Canvas canvas = null;
        
        // Find the canvas with the highest sorting order
        int highestOrder = -1;
        foreach (Canvas c in canvases)
        {
            if (c.sortingOrder > highestOrder)
            {
                canvas = c;
                highestOrder = c.sortingOrder;
            }
        }
        
        if (canvas == null && canvases.Length > 0)
            canvas = canvases[0];
            
        if (canvas == null)
        {
            Debug.LogWarning("[FallbackUIEnhancer] No canvas found for version label");
            return;
        }
        
        // Create version label gameobject
        GameObject versionObj = new GameObject("UI_Version_2.0_Label");
        versionObj.transform.SetParent(canvas.transform, false);
        
        // Set position (top right corner)
        RectTransform rectTransform = versionObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);
        rectTransform.anchoredPosition = new Vector2(-20, -20);
        rectTransform.sizeDelta = new Vector2(150, 30);
        
        // Add background
        Image background = versionObj.AddComponent<Image>();
        background.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        // Add text
        GameObject textObj = new GameObject("VersionText");
        textObj.transform.SetParent(versionObj.transform, false);
        
        RectTransform textRectTransform = textObj.AddComponent<RectTransform>();
        textRectTransform.anchorMin = Vector2.zero;
        textRectTransform.anchorMax = Vector2.one;
        textRectTransform.offsetMin = new Vector2(5, 2);
        textRectTransform.offsetMax = new Vector2(-5, -2);
        
        Text versionText = textObj.AddComponent<Text>();
        versionText.text = "UI VERSION 2.0";
        versionText.fontSize = 14;
        versionText.color = primaryColor;
        versionText.alignment = TextAnchor.MiddleCenter;
        versionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        
        // Add outline for better visibility
        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(1, -1);
        
        if (showDebugMessages)
            Debug.Log("[FallbackUIEnhancer] Version label created");
    }
    
    void EnhanceAllUI()
    {
        // Get GameManager to access panels
        GameManager gm = FindObjectOfType<GameManager>();
        
        if (gm != null)
        {
            if (showDebugMessages)
                Debug.Log("[FallbackUIEnhancer] Found GameManager, enhancing UI panels");
            
            // Enhance main panel
            if (gm.panel_Main != null)
            {
                EnhancePanel(gm.panel_Main);
            }
            
            // Enhance all other panels
            EnhancePanelIfExists(gm.panel_Lobby);
            EnhancePanelIfExists(gm.panel_Loading);
            EnhancePanelIfExists(gm.panel_Session);
            EnhancePanelIfExists(gm.panel_Game);
            EnhancePanelIfExists(gm.panel_Counter);
            EnhancePanelIfExists(gm.panel_Leave);
            EnhancePanelIfExists(gm.panel_Heroes);
            EnhancePanelIfExists(gm.panel_Leveling);
            EnhancePanelIfExists(gm.panel_Skills);
            EnhancePanelIfExists(gm.panel_Rankings);
            EnhancePanelIfExists(gm.panel_skillTooltip);
            EnhancePanelIfExists(gm.panel_levelTooltip);
            EnhancePanelIfExists(gm.panel_Round_Complete);
            EnhancePanelIfExists(gm.panel_GameStart);
            EnhancePanelIfExists(gm.panel_RoundStart);
            EnhancePanelIfExists(gm.panel_AddBot);
        }
        else
        {
            // If GameManager not found, try direct GameObject finding approach
            Debug.LogWarning("[FallbackUIEnhancer] GameManager not found, using direct GameObject finding");
            
            // Try to find the main canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                // Find all UI panels in the canvas
                foreach (Transform child in canvas.transform)
                {
                    if (child.name.StartsWith("Panel_") || child.name.StartsWith("panel_"))
                    {
                        EnhanceUIObject(child.gameObject);
                    }
                }
            }
            else
            {
                // No canvas found, find all panels in the scene
                GameObject mainPanel = GameObject.Find("Panel_Main");
                if (mainPanel != null)
                {
                    EnhanceUIObject(mainPanel);
                }
                else
                {
                    // Last resort - search for canvas by name pattern
                    Transform panelTransform = canvas.transform.Find("Panel_Main");
                    if (panelTransform != null)
                    {
                        EnhanceUIObject(panelTransform.gameObject);
                    }
                }
            }
        }
    }
    
    void EnhancePanelIfExists(UIVisibility panel)
    {
        if (panel != null)
        {
            EnhancePanel(panel);
        }
    }
    
    void EnhancePanel(UIVisibility panel)
    {
        if (panel == null || enhancedObjects.Contains(panel.gameObject.name))
            return;
            
        // Mark as enhanced
        enhancedObjects.Add(panel.gameObject.name);
        
        // Log what we're enhancing
        if (showDebugMessages)
            Debug.Log("[FallbackUIEnhancer] Enhancing panel: " + panel.gameObject.name);
        
        // Get all buttons in the panel
        Button[] buttons = panel.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            EnhanceButton(button);
        }
        
        // Get all texts in the panel
        Text[] texts = panel.GetComponentsInChildren<Text>(true);
        foreach (Text text in texts)
        {
            EnhanceText(text);
        }
        
        // Get all images in the panel
        Image[] images = panel.GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            // Skip button images
            if (image.GetComponent<Button>() != null)
                continue;
                
            // Basic panel enhancements
            if (image.transform.childCount > 0)
            {
                EnhanceBackgroundImage(image);
            }
        }
    }
    
    void EnhanceUIObject(GameObject obj)
    {
        if (obj == null || enhancedObjects.Contains(obj.name))
            return;
            
        // Mark as enhanced
        enhancedObjects.Add(obj.name);
        
        // Log what we're enhancing
        if (showDebugMessages)
            Debug.Log("[FallbackUIEnhancer] Enhancing UI object: " + obj.name);
        
        // Get all buttons in the object
        Button[] buttons = obj.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            EnhanceButton(button);
        }
        
        // Get all texts in the object
        Text[] texts = obj.GetComponentsInChildren<Text>(true);
        foreach (Text text in texts)
        {
            EnhanceText(text);
        }
        
        // Get all images in the object
        Image[] images = obj.GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            // Skip button images
            if (image.GetComponent<Button>() != null)
                continue;
                
            // Basic panel enhancements
            if (image.transform.childCount > 0)
            {
                EnhanceBackgroundImage(image);
            }
        }
    }
    
    void EnhanceButton(Button button)
    {
        if (button == null || enhancedObjects.Contains("btn_" + button.name))
            return;
            
        // Mark as enhanced
        enhancedObjects.Add("btn_" + button.name);
        
        // Get the image component
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            // Change the button color to a very distinctive bright red
            buttonImage.color = primaryColor;
            
            // Add rounded corners if using a standard sprite
            if (buttonImage.sprite == null || buttonImage.sprite.name == "Background" || buttonImage.sprite.name == "UISprite")
            {
                buttonImage.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
                buttonImage.type = Image.Type.Sliced;
                buttonImage.pixelsPerUnitMultiplier = 1;
            }
            
            // Add a strong outline
            Outline outline = button.GetComponent<Outline>();
            if (outline == null)
            {
                outline = button.gameObject.AddComponent<Outline>();
                outline.effectColor = new Color(0.0f, 0.0f, 0.0f, 0.8f);
                outline.effectDistance = new Vector2(2, -2);
            }
            
            // Add hover animation
            ButtonHoverEffect hoverEffect = button.GetComponent<ButtonHoverEffect>();
            if (hoverEffect == null)
            {
                hoverEffect = button.gameObject.AddComponent<ButtonHoverEffect>();
                hoverEffect.hoverScaleMultiplier = 1.1f;
                hoverEffect.animationSpeed = 3f;
                hoverEffect.targetGraphic = buttonImage;
            }
        }
        
        // Enhance text if it exists
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            // Make text white and bold
            buttonText.color = Color.white;
            buttonText.fontStyle = FontStyle.Bold;
            
            // Add an outline
            Outline textOutline = buttonText.GetComponent<Outline>();
            if (textOutline == null)
            {
                textOutline = buttonText.gameObject.AddComponent<Outline>();
                textOutline.effectColor = new Color(0, 0, 0, 0.8f);
                textOutline.effectDistance = new Vector2(1, -1);
            }
            
            // Increase font size
            buttonText.fontSize = Mathf.Max(buttonText.fontSize, 18);
        }
    }
    
    void EnhanceText(Text text)
    {
        if (text == null || enhancedObjects.Contains("txt_" + text.name))
            return;
            
        // Skip texts that are part of buttons
        if (text.GetComponentInParent<Button>() != null)
            return;
            
        // Mark as enhanced
        enhancedObjects.Add("txt_" + text.name);
        
        // Determine if this is a title
        bool isTitle = text.fontSize >= 20 || text.fontStyle == FontStyle.Bold;
        
        if (isTitle)
        {
            // Make titles use accent color
            text.color = accentColor;
            
            // Add an outline
            Outline outline = text.GetComponent<Outline>();
            if (outline == null)
            {
                outline = text.gameObject.AddComponent<Outline>();
                outline.effectColor = new Color(0, 0, 0, 0.8f);
                outline.effectDistance = new Vector2(1, -1);
            }
        }
        else
        {
            // Make regular text light gray for better contrast
            text.color = new Color(0.9f, 0.9f, 0.9f);
            
            // Ensure font size is readable
            if (text.fontSize < 14)
            {
                text.fontSize = 14;
            }
        }
    }
    
    void EnhanceBackgroundImage(Image panelImage)
    {
        if (panelImage == null || enhancedObjects.Contains("img_" + panelImage.name))
            return;
            
        // Mark as enhanced
        enhancedObjects.Add("img_" + panelImage.name);
        
        // Get the original color
        Color originalColor = panelImage.color;
        
        // Apply a subtle tint - keep opacity the same
        panelImage.color = new Color(
            originalColor.r * 0.7f,
            originalColor.g * 0.7f,
            Mathf.Min(originalColor.b * 1.4f, 0.9f),
            originalColor.a
        );
        
        // Add rounded corners if using a standard sprite
        if (panelImage.sprite == null || panelImage.sprite.name == "Background" || panelImage.sprite.name == "UISprite")
        {
            panelImage.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
            panelImage.type = Image.Type.Sliced;
            panelImage.pixelsPerUnitMultiplier = 1;
        }
        
        // Add border
        Outline panelOutline = panelImage.GetComponent<Outline>();
        if (panelOutline == null)
        {
            panelOutline = panelImage.gameObject.AddComponent<Outline>();
            panelOutline.effectColor = new Color(0.2f, 0.6f, 1f, 0.3f);
            panelOutline.effectDistance = new Vector2(3, -3);
        }
        
        // Add shadow
        Shadow panelShadow = panelImage.GetComponent<Shadow>();
        if (panelShadow == null)
        {
            panelShadow = panelImage.gameObject.AddComponent<Shadow>();
            panelShadow.effectColor = new Color(0, 0, 0, 0.5f);
            panelShadow.effectDistance = new Vector2(4, -4);
        }
    }
} 