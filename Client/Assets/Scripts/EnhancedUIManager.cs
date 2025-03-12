/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Enhanced UI Manager to coordinate UI animations, themes, and transitions.
/// This class supplements the existing UI system with more modern features.
/// </summary>
public class EnhancedUIManager : MonoBehaviour 
{
    public static EnhancedUIManager instance;

    [Header("UI Settings")]
    public Color primaryColor = new Color(0.1f, 0.6f, 0.9f, 1f);
    public Color secondaryColor = new Color(0.9f, 0.3f, 0.1f, 1f);
    public Color accentColor = new Color(0.9f, 0.8f, 0.1f, 1f);
    public Color neutralColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    
    [Header("Font Settings")]
    public string primaryFontName = "Riffic";
    public string secondaryFontName = "Arial";
    public float defaultFontSize = 16f;
    public float titleFontSize = 24f;
    public float smallFontSize = 12f;
    public bool useModernFontRendering = true;
    public Color fontColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    
    [Header("Animation Settings")]
    public float defaultTransitionSpeed = 0.3f;
    public float fastTransitionSpeed = 0.15f;
    public float slowTransitionSpeed = 0.5f;
    
    [Header("Particle Effects")]
    public GameObject buttonClickEffect;
    public GameObject panelTransitionEffect;
    
    [Header("Sound Effects")]
    public AudioClip buttonClickSound;
    public AudioClip panelOpenSound;
    public AudioClip panelCloseSound;
    public AudioClip notificationSound;
    
    // Runtime variables
    private Dictionary<string, Color> themePalette = new Dictionary<string, Color>();
    private AudioSource audioSource;
    
    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        // Setup theme palette
        themePalette.Add("primary", primaryColor);
        themePalette.Add("secondary", secondaryColor);
        themePalette.Add("accent", accentColor);
        themePalette.Add("neutral", neutralColor);
    }
    
    /// <summary>
    /// Apply a pulsing effect to a UI element to draw attention to it
    /// </summary>
    public void PulseElement(RectTransform element, float intensity = 0.1f, float duration = 1f)
    {
        StartCoroutine(PulseElementCoroutine(element, intensity, duration));
    }
    
    private IEnumerator PulseElementCoroutine(RectTransform element, float intensity, float duration)
    {
        Vector3 originalScale = element.localScale;
        Vector3 targetScale = originalScale * (1 + intensity);
        
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.PingPong(timer * 2, 1f);
            element.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        element.localScale = originalScale;
    }
    
    /// <summary>
    /// Enhance a button with visual effects and sound
    /// </summary>
    public void EnhanceButton(Button button)
    {
        // Remove any existing listeners to avoid duplicates
        button.onClick.RemoveAllListeners();
        
        // Add our enhanced click handler
        button.onClick.AddListener(() => {
            PlayButtonClickSound();
            ShowButtonClickEffect(button.transform.position);
            
            // Scale effect
            RectTransform rect = button.GetComponent<RectTransform>();
            if (rect != null)
            {
                StartCoroutine(ButtonClickAnimation(rect));
            }
        });
    }
    
    private IEnumerator ButtonClickAnimation(RectTransform buttonRect)
    {
        Vector3 originalScale = buttonRect.localScale;
        buttonRect.localScale = originalScale * 0.9f;
        yield return new WaitForSeconds(0.05f);
        buttonRect.localScale = originalScale * 1.1f;
        yield return new WaitForSeconds(0.05f);
        buttonRect.localScale = originalScale;
    }
    
    /// <summary>
    /// Play button click sound
    /// </summary>
    public void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    /// <summary>
    /// Apply font settings to a Text component
    /// </summary>
    public void ApplyFontSettings(Text textComponent, string fontStyle = "normal")
    {
        if (textComponent == null) return;
        
        // Set font size based on style
        switch (fontStyle.ToLower())
        {
            case "title":
                textComponent.fontSize = Mathf.RoundToInt(titleFontSize);
                textComponent.fontStyle = FontStyle.Bold;
                break;
            case "small":
                textComponent.fontSize = Mathf.RoundToInt(smallFontSize);
                textComponent.fontStyle = FontStyle.Normal;
                break;
            default:
                textComponent.fontSize = Mathf.RoundToInt(defaultFontSize);
                textComponent.fontStyle = FontStyle.Normal;
                break;
        }
        
        // Apply font color
        textComponent.color = fontColor;
        
        // Apply other text rendering improvements
        if (useModernFontRendering)
        {
            textComponent.material = Resources.Load<Material>("UI/ModernTextMaterial");
            textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;
        }
    }
    
    /// <summary>
    /// Show visual effect at button position
    /// </summary>
    public void ShowButtonClickEffect(Vector3 position)
    {
        if (buttonClickEffect != null)
        {
            GameObject effect = Instantiate(buttonClickEffect, position, Quaternion.identity);
            effect.transform.SetParent(transform);
            Destroy(effect, 1f);
        }
    }
    
    /// <summary>
    /// Apply theme colors to UI element
    /// </summary>
    public void ApplyThemeColor(Graphic graphic, string colorName)
    {
        if (themePalette.ContainsKey(colorName))
        {
            graphic.color = themePalette[colorName];
        }
    }
    
    /// <summary>
    /// Apply a smooth animation to open a UI panel
    /// </summary>
    public void AnimatePanelOpen(RectTransform panel, float duration = -1)
    {
        if (duration < 0) duration = defaultTransitionSpeed;
        
        if (panelOpenSound != null)
        {
            audioSource.PlayOneShot(panelOpenSound);
        }
        
        panel.gameObject.SetActive(true);
        
        // Get or add CanvasGroup
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
        }
        
        canvasGroup.alpha = 0;
        StartCoroutine(FadeIn(canvasGroup, duration));
    }
    
    /// <summary>
    /// Apply a smooth animation to close a UI panel
    /// </summary>
    public void AnimatePanelClose(RectTransform panel, float duration = -1)
    {
        if (duration < 0) duration = defaultTransitionSpeed;
        
        if (panelCloseSound != null)
        {
            audioSource.PlayOneShot(panelCloseSound);
        }
        
        // Get or add CanvasGroup
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
        }
        
        StartCoroutine(FadeOut(canvasGroup, duration, panel.gameObject));
    }
    
    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / duration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }
    
    private IEnumerator FadeOut(CanvasGroup canvasGroup, float duration, GameObject panel)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / duration);
            yield return null;
        }
        canvasGroup.alpha = 0;
        panel.SetActive(false);
    }
    
    /// <summary>
    /// Show a temporary notification message on screen
    /// </summary>
    public void ShowNotification(string message, float duration = 3f)
    {
        if (notificationSound != null)
        {
            audioSource.PlayOneShot(notificationSound);
        }
        
        // Find notification panel in GameManager
        GameManager gm = GameManager.singleton;
        if (gm != null && gm.panel_Game != null)
        {
            Transform notificationPanel = gm.panel_Game.transform.Find("NotificationPanel");
            if (notificationPanel != null)
            {
                Text notificationText = notificationPanel.GetComponentInChildren<Text>();
                if (notificationText != null)
                {
                    notificationText.text = message;
                    notificationPanel.gameObject.SetActive(true);
                    
                    // Auto-hide after duration
                    StartCoroutine(HideAfterDuration(notificationPanel.gameObject, duration));
                }
            }
        }
    }
    
    private IEnumerator HideAfterDuration(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
    }
} 