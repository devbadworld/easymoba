/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Direct UI enhancer that makes obvious visual changes to the main UI elements.
/// This approach bypasses the complexity of our previous enhancer and focuses on direct modifications.
/// </summary>
public class UIDirectEnhancer : MonoBehaviour 
{
    // This will be automatically called when the game starts
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoInitialize()
    {
        Debug.Log("UIDirectEnhancer starting - v1.0");
        
        // Create the enhancer object that will survive scene changes
        GameObject enhancer = new GameObject("UI_DirectEnhancer");
        DontDestroyOnLoad(enhancer);
        UIDirectEnhancer instance = enhancer.AddComponent<UIDirectEnhancer>();
        
        // Start the enhancement process
        instance.StartCoroutine(instance.EnhanceWithDelay());
    }
    
    private IEnumerator EnhanceWithDelay()
    {
        // Wait for 1 second to ensure the game UI is loaded
        yield return new WaitForSeconds(1f);
        
        // Apply enhancements
        EnhanceMainUI();
        
        // Wait and continue applying enhancements periodically
        while (true)
        {
            yield return new WaitForSeconds(3f);
            EnhanceMainUI();
        }
    }
    
    private void EnhanceMainUI()
    {
        Debug.Log("Applying direct UI enhancements...");
        
        // Find and enhance the main UI canvas
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.name.Contains("Canvas"))
            {
                Debug.Log($"Enhancing canvas: {canvas.name}");
                EnhanceCanvas(canvas);
            }
        }
        
        // Find and enhance all buttons
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button button in allButtons)
        {
            EnhanceButton(button);
        }
        
        // Try to find GameManager for more specific enhancements
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            Debug.Log("Found GameManager - applying specific enhancements");
            
            // Enhance main panel
            if (gm.panel_Main != null)
            {
                EnhancePanel(gm.panel_Main);
            }
            
            // Enhance game panel
            if (gm.panel_Game != null)
            {
                EnhancePanel(gm.panel_Game);
            }
            
            // Enhance menu buttons
            EnhanceMainMenuButtons(gm);
        }
    }
    
    private void EnhanceCanvas(Canvas canvas)
    {
        // Add a subtle gradient background to the canvas
        GameObject bg = new GameObject("EnhancedBackground");
        bg.transform.SetParent(canvas.transform, false);
        
        // Ensure it's at the very back
        bg.transform.SetAsFirstSibling();
        
        // Set up the RectTransform to cover the entire canvas
        RectTransform rectTransform = bg.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        // Add and configure the Image component
        Image image = bg.AddComponent<Image>();
        image.color = new Color(0.05f, 0.1f, 0.2f, 0.2f);
    }
    
    private void EnhanceButton(Button button)
    {
        if (button == null) return;
        
        // Get the image component
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            // Change the button color to a more vibrant blue
            Color originalColor = buttonImage.color;
            buttonImage.color = new Color(0.2f, 0.6f, 1f, originalColor.a);
            
            // Add a subtle glow effect
            Outline outline = button.GetComponent<Outline>();
            if (outline == null)
            {
                outline = button.gameObject.AddComponent<Outline>();
                outline.effectColor = new Color(0.4f, 0.8f, 1f, 0.5f);
                outline.effectDistance = new Vector2(2, -2);
            }
        }
        
        // Enhance text if it exists
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            // Make the text more readable
            buttonText.color = Color.white;
            
            // Add an outline to the text for better contrast
            Outline textOutline = buttonText.GetComponent<Outline>();
            if (textOutline == null)
            {
                textOutline = buttonText.gameObject.AddComponent<Outline>();
                textOutline.effectColor = new Color(0, 0, 0, 0.5f);
                textOutline.effectDistance = new Vector2(1, -1);
            }
            
            // Slightly increase font size for better readability
            buttonText.fontSize = Mathf.Max(buttonText.fontSize, 16);
        }
        
        // Update the button name to show it's been enhanced
        button.name = button.name + "_Enhanced";
    }
    
    private void EnhancePanel(GameObject panel)
    {
        if (panel == null) return;
        
        Debug.Log($"Enhancing panel: {panel.name}");
        
        // Add a subtle background tint
        Image panelImage = panel.GetComponent<Image>();
        if (panelImage != null)
        {
            // Add a subtle blue tint
            Color originalColor = panelImage.color;
            panelImage.color = new Color(
                originalColor.r * 0.9f, 
                originalColor.g * 0.9f,
                Mathf.Min(originalColor.b * 1.2f, 1f),
                originalColor.a
            );
        }
        
        // Make the panel corners slightly rounded
        if (panelImage != null && panelImage.sprite == null)
        {
            panelImage.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd");
            panelImage.type = Image.Type.Sliced;
        }
        
        // Find and enhance all buttons within the panel
        Button[] buttons = panel.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            EnhanceButton(button);
        }
        
        // Find and enhance all texts within the panel
        Text[] texts = panel.GetComponentsInChildren<Text>();
        foreach (Text text in texts)
        {
            EnhanceText(text);
        }
    }
    
    private void EnhanceText(Text text)
    {
        if (text == null) return;
        
        // Determine if this is a title or regular text based on size
        bool isTitle = text.fontSize >= 20;
        
        // Enhance the text appearance
        if (isTitle)
        {
            // Title text
            text.color = new Color(0.9f, 0.8f, 0.2f); // Gold
            
            // Add outline for better contrast
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
            // Regular text
            text.color = new Color(0.9f, 0.9f, 0.9f); // Light gray
            
            // Increase font size slightly for better readability
            if (text.fontSize < 14)
            {
                text.fontSize = 14;
            }
        }
        
        // Update the text name to show it's been enhanced
        text.name = text.name + "_Enhanced";
    }
    
    private void EnhanceMainMenuButtons(GameManager gm)
    {
        if (gm.panel_Main == null) return;
        
        // Try to find main menu buttons
        Button[] menuButtons = gm.panel_Main.GetComponentsInChildren<Button>();
        
        foreach (Button button in menuButtons)
        {
            // Special enhancement for menu buttons
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                // Create a more vibrant appearance
                buttonImage.color = new Color(0.2f, 0.5f, 0.9f, 1f);
                
                // Add a subtle glow effect with larger outline
                Outline outline = button.GetComponent<Outline>();
                if (outline == null)
                {
                    outline = button.gameObject.AddComponent<Outline>();
                }
                
                outline.effectColor = new Color(0.4f, 0.7f, 1f, 0.7f);
                outline.effectDistance = new Vector2(3, -3);
            }
            
            // Make the text stand out more
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.color = Color.white;
                buttonText.fontSize = Mathf.Max(buttonText.fontSize, 18); // Ensure good size
                
                // Add shadow
                Shadow shadow = buttonText.GetComponent<Shadow>();
                if (shadow == null)
                {
                    shadow = buttonText.gameObject.AddComponent<Shadow>();
                    shadow.effectColor = new Color(0, 0, 0, 0.8f);
                    shadow.effectDistance = new Vector2(2, -2);
                }
            }
        }
    }
} 