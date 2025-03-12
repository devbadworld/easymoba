/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Modern UI Button with enhanced visuals and interactions.
/// Extends the standard button with hover effects, animations, and sound feedback.
/// </summary>
public class ModernUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler 
{
    [Header("Button References")]
    public Button button;
    public RectTransform buttonRect;
    public Text buttonText;
    public Image buttonImage;
    
    [Header("Visual Effects")]
    public bool useHoverEffect = true;
    public bool useClickEffect = true;
    public bool useRippleEffect = true;
    public float hoverScaleFactor = 1.05f;
    public float clickScaleFactor = 0.95f;
    
    [Header("Color Settings")]
    public Color normalColor = new Color(0.1f, 0.6f, 0.9f, 1f);
    public Color hoverColor = new Color(0.3f, 0.7f, 1f, 1f);
    public Color pressedColor = new Color(0.08f, 0.5f, 0.8f, 1f);
    public Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    
    [Header("Audio")]
    public bool useSound = true;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    
    // Private variables
    private Vector3 originalScale;
    private Color originalTextColor;
    private bool isPointerOver = false;
    private bool isPointerDown = false;
    private AudioSource audioSource;
    private EnhancedUIManager uiManager;
    
    void Awake()
    {
        // Get component references if not set
        if (button == null) button = GetComponent<Button>();
        if (buttonRect == null) buttonRect = GetComponent<RectTransform>();
        if (buttonText == null) buttonText = GetComponentInChildren<Text>();
        if (buttonImage == null) buttonImage = GetComponent<Image>();
        
        // Store original values
        if (buttonRect) originalScale = buttonRect.localScale;
        if (buttonText) originalTextColor = buttonText.color;
        
        // Create audio source if needed
        if (useSound && audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Try to find UI Manager
        uiManager = FindObjectOfType<EnhancedUIManager>();
    }
    
    void OnEnable()
    {
        // Reset state
        isPointerOver = false;
        isPointerDown = false;
        
        // Reset scale
        if (buttonRect) buttonRect.localScale = originalScale;
        
        // Apply initial color based on button state
        UpdateVisualState();
    }
    
    // Implement pointer event handlers
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;
        
        isPointerOver = true;
        
        if (useHoverEffect && buttonRect)
        {
            // Scale up effect
            buttonRect.localScale = originalScale * hoverScaleFactor;
        }
        
        // Update colors
        UpdateVisualState();
        
        // Play hover sound
        if (useSound && hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
        else if (uiManager != null)
        {
            // Use UI Manager sound if available
            uiManager.PlayButtonClickSound();
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable) return;
        
        isPointerOver = false;
        
        if (useHoverEffect && buttonRect)
        {
            // Restore original scale
            buttonRect.localScale = originalScale;
        }
        
        // Update colors
        UpdateVisualState();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        
        isPointerDown = true;
        
        if (useClickEffect && buttonRect)
        {
            // Scale down effect
            buttonRect.localScale = originalScale * clickScaleFactor;
        }
        
        // Update colors
        UpdateVisualState();
        
        // Play click sound
        if (useSound && clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        
        // Create ripple effect
        if (useRippleEffect)
        {
            CreateRippleEffect(eventData.position);
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;
        
        isPointerDown = false;
        
        if (useClickEffect && buttonRect)
        {
            // Return to hover or normal scale
            if (isPointerOver)
            {
                buttonRect.localScale = originalScale * hoverScaleFactor;
            }
            else
            {
                buttonRect.localScale = originalScale;
            }
        }
        
        // Update colors
        UpdateVisualState();
    }
    
    private void UpdateVisualState()
    {
        if (buttonImage == null) return;
        
        // Set button colors based on state
        if (!button.interactable)
        {
            buttonImage.color = disabledColor;
        }
        else if (isPointerDown)
        {
            buttonImage.color = pressedColor;
        }
        else if (isPointerOver)
        {
            buttonImage.color = hoverColor;
        }
        else
        {
            buttonImage.color = normalColor;
        }
        
        // Update text color for better contrast
        if (buttonText != null)
        {
            // Make text slightly brighter for better contrast
            buttonText.color = new Color(
                Mathf.Min(buttonImage.color.r + 0.3f, 1f),
                Mathf.Min(buttonImage.color.g + 0.3f, 1f),
                Mathf.Min(buttonImage.color.b + 0.3f, 1f),
                1f
            );
        }
    }
    
    private void CreateRippleEffect(Vector2 position)
    {
        if (uiManager == null) return;
        
        // Use UI Manager to create effect at the click position
        uiManager.ShowButtonClickEffect(transform.position);
    }
} 