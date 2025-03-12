/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Modern UI Button with hover effects, animations, and sound feedback.
/// </summary>
public class ModernUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    public bool enableAnimation = true;
    public float hoverScaleMultiplier = 1.05f;
    public float clickScaleMultiplier = 0.95f;
    public float animationSpeed = 10f;
    
    [Header("Color Settings")]
    public bool useColorTransition = true;
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.9f, 0.9f, 1f);
    public Color pressedColor = new Color(0.7f, 0.7f, 0.8f);
    public Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    
    [Header("Sound and Effects")]
    public bool useSound = true;
    public bool useRippleEffect = false;
    public GameObject rippleEffectPrefab;
    
    [Header("Border Effects")]
    public bool useBorderAnimation = false;
    public Image borderImage;
    public float borderGlowIntensity = 0.2f;
    public Color borderGlowColor = new Color(1f, 1f, 1f, 0.5f);
    
    // Private variables
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Image buttonImage;
    private Button unityButton;
    private Color originalBorderColor;
    private bool isInteractable = true;
    
    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        unityButton = GetComponent<Button>();
        
        if (borderImage != null)
        {
            originalBorderColor = borderImage.color;
        }
    }
    
    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        
        // If we have a Unity Button component, monitor its interactable property
        if (unityButton != null)
        {
            isInteractable = unityButton.interactable;
            
            // Set initial color based on interactable state
            if (useColorTransition && buttonImage != null)
            {
                buttonImage.color = isInteractable ? normalColor : disabledColor;
            }
        }
    }
    
    private void Update()
    {
        // Check if interactable state has changed
        if (unityButton != null && unityButton.interactable != isInteractable)
        {
            isInteractable = unityButton.interactable;
            
            if (useColorTransition && buttonImage != null)
            {
                buttonImage.color = isInteractable ? normalColor : disabledColor;
            }
            
            // Reset scale if disabled
            if (!isInteractable)
            {
                targetScale = originalScale;
            }
        }
        
        // Animate scale
        if (enableAnimation && transform.localScale != targetScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        if (enableAnimation)
        {
            targetScale = originalScale * hoverScaleMultiplier;
        }
        
        if (useColorTransition && buttonImage != null)
        {
            buttonImage.color = hoverColor;
        }
        
        if (useBorderAnimation && borderImage != null)
        {
            borderImage.color = borderGlowColor;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        if (enableAnimation)
        {
            targetScale = originalScale;
        }
        
        if (useColorTransition && buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
        
        if (useBorderAnimation && borderImage != null)
        {
            borderImage.color = originalBorderColor;
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        if (enableAnimation)
        {
            targetScale = originalScale * clickScaleMultiplier;
        }
        
        if (useColorTransition && buttonImage != null)
        {
            buttonImage.color = pressedColor;
        }
        
        if (useSound && EnhancedUIManager.instance != null)
        {
            EnhancedUIManager.instance.PlayButtonClickSound();
        }
        
        if (useRippleEffect && rippleEffectPrefab != null)
        {
            CreateRippleEffect(eventData.position);
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        if (enableAnimation)
        {
            // If still hovering, go to hover scale, otherwise back to normal
            if (RectTransformUtility.RectangleContainsScreenPoint(
                transform as RectTransform, eventData.position, eventData.pressEventCamera))
            {
                targetScale = originalScale * hoverScaleMultiplier;
                
                if (useColorTransition && buttonImage != null)
                {
                    buttonImage.color = hoverColor;
                }
            }
            else
            {
                targetScale = originalScale;
                
                if (useColorTransition && buttonImage != null)
                {
                    buttonImage.color = normalColor;
                }
                
                if (useBorderAnimation && borderImage != null)
                {
                    borderImage.color = originalBorderColor;
                }
            }
        }
    }
    
    private void CreateRippleEffect(Vector2 position)
    {
        if (rippleEffectPrefab == null) return;
        
        // Convert screen position to local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform, position, null, out Vector2 localPoint);
            
        // Create ripple effect
        GameObject ripple = Instantiate(rippleEffectPrefab, transform);
        ripple.transform.localPosition = localPoint;
        
        // Start ripple animation
        StartCoroutine(AnimateRipple(ripple));
    }
    
    private IEnumerator AnimateRipple(GameObject ripple)
    {
        // Get ripple components
        RectTransform rippleRect = ripple.GetComponent<RectTransform>();
        Image rippleImage = ripple.GetComponent<Image>();
        
        if (rippleRect == null || rippleImage == null)
        {
            Destroy(ripple);
            yield break;
        }
        
        // Start with small transparent circle
        rippleRect.sizeDelta = new Vector2(0, 0);
        Color startColor = rippleImage.color;
        rippleImage.color = new Color(startColor.r, startColor.g, startColor.b, 0.8f);
        
        // Calculate target size (diagonal of the button)
        RectTransform buttonRect = transform as RectTransform;
        float targetSize = Mathf.Sqrt(
            buttonRect.rect.width * buttonRect.rect.width +
            buttonRect.rect.height * buttonRect.rect.height) * 2f;
        
        // Animate expansion
        float duration = 0.5f;
        float timer = 0;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            
            // Scale up
            float currentSize = Mathf.Lerp(0, targetSize, t);
            rippleRect.sizeDelta = new Vector2(currentSize, currentSize);
            
            // Fade out
            float alpha = Mathf.Lerp(0.8f, 0f, t);
            rippleImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            
            yield return null;
        }
        
        Destroy(ripple);
    }
    
    /// <summary>
    /// Set the button's interactable state
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        
        if (unityButton != null)
        {
            unityButton.interactable = interactable;
        }
        
        if (useColorTransition && buttonImage != null)
        {
            buttonImage.color = interactable ? normalColor : disabledColor;
        }
        
        if (!interactable)
        {
            targetScale = originalScale;
            transform.localScale = originalScale;
            
            if (useBorderAnimation && borderImage != null)
            {
                borderImage.color = originalBorderColor;
            }
        }
    }
} 