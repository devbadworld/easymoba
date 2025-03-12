/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Fallback UI enhancer that will try a different approach to applying UI changes.
/// This will create some very visible UI modifications to confirm our code is running.
/// </summary>
public class FallbackUIEnhancer : MonoBehaviour 
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnGameStart()
    {
        Debug.Log("FallbackUIEnhancer starting - v1.0");
        
        // Create the enhancer object
        GameObject enhancer = new GameObject("UI_FallbackEnhancer");
        DontDestroyOnLoad(enhancer);
        FallbackUIEnhancer instance = enhancer.AddComponent<FallbackUIEnhancer>();
        
        // Start the enhancement process with a small delay
        instance.StartCoroutine(instance.DelayedStart());
    }
    
    private IEnumerator DelayedStart()
    {
        // Wait to ensure scene is fully loaded
        yield return new WaitForSeconds(2f);
        
        // Apply the enhancements
        ApplyEnhancements();
        
        // Continue checking for new UI elements
        StartCoroutine(ContinuousUICheck());
    }
    
    private IEnumerator ContinuousUICheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            ApplyEnhancements();
        }
    }
    
    private void ApplyEnhancements()
    {
        Debug.Log("Applying fallback UI enhancements");
        
        // Create a visual indicator to confirm this script is running
        CreateUIIndicator();
        
        // Apply direct styling to all buttons
        EnhanceAllButtons();
        
        // Apply styling to all text elements
        EnhanceAllText();
        
        // Try to find and enhance the main menu specifically
        EnhanceMainMenu();
        
        // Add a pulsing effect to clickable elements
        AddPulsingEffectToButtons();
    }
    
    private void CreateUIIndicator()
    {
        // Check if indicator already exists
        if (GameObject.Find("UIEnhancementIndicator") != null) return;
        
        // Create a canvas for our indicator
        GameObject canvasObj = new GameObject("UIEnhancementIndicator");
        DontDestroyOnLoad(canvasObj);
        
        // Add canvas components
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000; // Ensure it's at the very top
        
        // Add a small indicator in the top-right corner
        GameObject indicator = new GameObject("Indicator");
        indicator.transform.SetParent(canvas.transform, false);
        
        // Configure the indicator
        RectTransform rectTransform = indicator.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);
        rectTransform.anchoredPosition = new Vector2(-10, -10);
        rectTransform.sizeDelta = new Vector2(20, 20);
        
        // Add an image component
        Image image = indicator.AddComponent<Image>();
        image.color = new Color(0.1f, 0.6f, 1f, 0.8f);
        
        // Make it circular
        image.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
        
        // Add a pulsing animation
        StartCoroutine(PulseIndicator(image));
    }
    
    private IEnumerator PulseIndicator(Image image)
    {
        while (true)
        {
            // Pulse color intensity
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                float pulse = 0.5f + 0.5f * Mathf.Sin(t * Mathf.PI * 2);
                image.color = new Color(0.1f + 0.4f * pulse, 0.6f, 1f, 0.8f);
                yield return null;
            }
            
            yield return null;
        }
    }
    
    private void EnhanceAllButtons()
    {
        // Find all buttons in the scene
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button button in buttons)
        {
            // Skip buttons that are not active in the hierarchy
            if (!button.isActiveAndEnabled) continue;
            
            // Skip buttons that have already been enhanced
            if (button.name.EndsWith("_Enhanced")) continue;
            
            // Apply a very visible style change to the button
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                // Apply a blue gradient effect
                buttonImage.color = new Color(0.2f, 0.5f, 0.9f, 1f);
                
                // Add a glowing outline
                Shadow shadow = button.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0.4f, 0.7f, 1f, 0.8f);
                shadow.effectDistance = new Vector2(4, -4);
            }
            
            // Enhance the button text
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                // Make text white for contrast
                buttonText.color = Color.white;
                
                // Add outline for readability
                Outline outline = buttonText.gameObject.AddComponent<Outline>();
                outline.effectColor = new Color(0f, 0f, 0f, 0.8f);
                outline.effectDistance = new Vector2(1, -1);
                
                // Increase font size slightly
                buttonText.fontSize = Mathf.Min(buttonText.fontSize + 2, 24);
            }
            
            // Mark as enhanced
            button.name = button.name + "_Enhanced";
            
            Debug.Log($"Enhanced button: {button.name}");
        }
    }
    
    private void EnhanceAllText()
    {
        // Find all text elements in the scene
        Text[] texts = Resources.FindObjectsOfTypeAll<Text>();
        foreach (Text text in texts)
        {
            // Skip text that is not active in the hierarchy
            if (!text.isActiveAndEnabled) continue;
            
            // Skip text that has already been enhanced
            if (text.name.EndsWith("_Enhanced")) continue;
            
            // Determine if this is a title based on size
            bool isTitle = text.fontSize >= 24;
            
            if (isTitle)
            {
                // Make titles gold with black outline
                text.color = new Color(1f, 0.84f, 0f, 1f);
                
                // Add a shadow for depth
                Shadow shadow = text.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0, 0, 0, 0.5f);
                shadow.effectDistance = new Vector2(2, -2);
            }
            else
            {
                // Make regular text more readable
                text.color = new Color(0.9f, 0.9f, 0.9f, 1f);
                
                // Add a subtle outline
                Outline outline = text.gameObject.AddComponent<Outline>();
                outline.effectColor = new Color(0, 0, 0, 0.5f);
                outline.effectDistance = new Vector2(1, -1);
            }
            
            // Mark as enhanced
            text.name = text.name + "_Enhanced";
            
            Debug.Log($"Enhanced text: {text.name}");
        }
    }
    
    private void EnhanceMainMenu()
    {
        // Try to find the main menu
        GameObject mainPanel = GameObject.Find("Panel_Main");
        if (mainPanel == null)
        {
            // Try alternative approach
            Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                Transform panelTransform = canvas.transform.Find("Panel_Main");
                if (panelTransform != null)
                {
                    mainPanel = panelTransform.gameObject;
                    break;
                }
            }
        }
        
        if (mainPanel != null)
        {
            Debug.Log("Found main menu panel - applying special enhancements");
            
            // Add a background image or tint
            Image panelImage = mainPanel.GetComponent<Image>();
            if (panelImage != null)
            {
                // Apply a subtle gradient background
                panelImage.color = new Color(0.1f, 0.2f, 0.3f, 0.9f);
            }
            
            // Find the title
            Text[] texts = mainPanel.GetComponentsInChildren<Text>();
            foreach (Text text in texts)
            {
                if (text.fontSize >= 24 || text.text.ToUpper() == text.text)
                {
                    // This is likely a title - make it stand out
                    text.color = new Color(1f, 0.84f, 0f, 1f); // Gold
                    text.fontSize = Mathf.Max(text.fontSize, 28);
                    
                    // Add a nice shadow effect
                    Shadow shadow = text.gameObject.AddComponent<Shadow>();
                    shadow.effectColor = new Color(0, 0, 0, 0.8f);
                    shadow.effectDistance = new Vector2(3, -3);
                    
                    Debug.Log($"Enhanced title text: {text.text}");
                }
            }
            
            // Enhance menu buttons with a special style
            Button[] buttons = mainPanel.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    // Create a visually striking blue button
                    buttonImage.color = new Color(0.2f, 0.4f, 0.9f, 1f);
                    
                    // Try to make it rounded if possible
                    if (buttonImage.sprite == null)
                    {
                        buttonImage.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
                        buttonImage.type = Image.Type.Sliced;
                    }
                    
                    // Add a strong glow effect
                    Shadow shadow = button.gameObject.AddComponent<Shadow>();
                    shadow.effectColor = new Color(0.4f, 0.6f, 1f, 0.8f);
                    shadow.effectDistance = new Vector2(5, -5);
                }
                
                Debug.Log($"Enhanced menu button: {button.name}");
            }
        }
    }
    
    private void AddPulsingEffectToButtons()
    {
        // Find all buttons
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        
        foreach (Button button in buttons)
        {
            // Skip buttons that are not active or already have a pulse
            if (!button.isActiveAndEnabled || button.gameObject.GetComponent<ButtonPulseEffect>() != null)
                continue;
            
            // Add a pulse effect
            ButtonPulseEffect pulseEffect = button.gameObject.AddComponent<ButtonPulseEffect>();
            
            Debug.Log($"Added pulse effect to button: {button.name}");
        }
    }
}

/// <summary>
/// Component to add a subtle pulsing effect to buttons
/// </summary>
public class ButtonPulseEffect : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private float pulseTime = 0f;
    private float pulseDuration = 2f;
    private float pulseIntensity = 0.05f;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            originalScale = rectTransform.localScale;
            
            // Randomize the starting phase for each button
            pulseTime = Random.Range(0f, pulseDuration);
        }
    }
    
    void Update()
    {
        if (rectTransform == null) return;
        
        // Update pulse effect
        pulseTime += Time.deltaTime;
        if (pulseTime > pulseDuration) pulseTime = 0f;
        
        // Calculate pulse factor
        float pulseFactor = 1f + pulseIntensity * Mathf.Sin(pulseTime / pulseDuration * 2f * Mathf.PI);
        
        // Apply scale
        rectTransform.localScale = originalScale * pulseFactor;
    }
} 