/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Enhanced UI Visibility controller with multiple animation options
/// and better performance than the original UIVisibility.
/// </summary>
public class EnhancedUIVisibility : MonoBehaviour 
{
    [Header("Animation Settings")]
    public AnimationType animationType = AnimationType.Fade;
    public float animationDuration = 0.3f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Behavior")]
    public bool showOnStart = false;
    public bool destroyOnHide = false;
    public bool disableRaycastOnHide = true;
    
    [Header("Sound")]
    public bool playSound = true;
    
    // Component references
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    
    // Animation variables
    private Vector2 originalPosition;
    private Vector2 originalSize;
    private Vector3 originalScale;
    private Coroutine currentAnimation;
    
    // UI Visibility states
    private bool isVisible = false;
    
    public enum AnimationType
    {
        Fade,
        Scale,
        SlideFromTop,
        SlideFromBottom,
        SlideFromLeft,
        SlideFromRight,
        FadeAndScale
    }
    
    void Awake()
    {
        // Get or add required components
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
        rectTransform = GetComponent<RectTransform>();
        
        // Store original values
        originalPosition = rectTransform.anchoredPosition;
        originalSize = rectTransform.sizeDelta;
        originalScale = rectTransform.localScale;
        
        // Initialize visibility
        if (!showOnStart)
        {
            canvasGroup.alpha = 0;
            if (disableRaycastOnHide)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            
            // Set initial state based on animation type
            switch (animationType)
            {
                case AnimationType.Scale:
                case AnimationType.FadeAndScale:
                    rectTransform.localScale = Vector3.zero;
                    break;
                    
                case AnimationType.SlideFromTop:
                    rectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + Screen.height);
                    break;
                    
                case AnimationType.SlideFromBottom:
                    rectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y - Screen.height);
                    break;
                    
                case AnimationType.SlideFromLeft:
                    rectTransform.anchoredPosition = new Vector2(originalPosition.x - Screen.width, originalPosition.y);
                    break;
                    
                case AnimationType.SlideFromRight:
                    rectTransform.anchoredPosition = new Vector2(originalPosition.x + Screen.width, originalPosition.y);
                    break;
            }
            
            gameObject.SetActive(false);
        }
        else
        {
            isVisible = true;
            canvasGroup.alpha = 1;
        }
    }
    
    void Start()
    {
        if (showOnStart)
        {
            Open();
        }
    }
    
    /// <summary>
    /// Open/show the UI element with animation
    /// </summary>
    public void Open()
    {
        Open(true);
    }
    
    /// <summary>
    /// Close/hide the UI element with animation
    /// </summary>
    public void Close()
    {
        Open(false);
    }
    
    /// <summary>
    /// Toggle the visibility state
    /// </summary>
    public void Toggle()
    {
        Open(!isVisible);
    }
    
    /// <summary>
    /// Open or close the UI element
    /// </summary>
    public void Open(bool show)
    {
        // If already in the desired state, do nothing
        if (isVisible == show)
            return;
            
        isVisible = show;
        
        // Stop any running animation
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
        
        if (show)
        {
            // Make sure the game object is active
            gameObject.SetActive(true);
            
            // Enable interaction
            if (disableRaycastOnHide)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            
            // Play sound if enabled
            if (playSound && EnhancedUIManager.instance != null)
            {
                EnhancedUIManager.instance.PlayButtonClickSound();
            }
            
            // Start animation based on type
            switch (animationType)
            {
                case AnimationType.Fade:
                    currentAnimation = StartCoroutine(AnimateFade(0, 1));
                    break;
                    
                case AnimationType.Scale:
                    currentAnimation = StartCoroutine(AnimateScale(Vector3.zero, originalScale));
                    break;
                    
                case AnimationType.FadeAndScale:
                    currentAnimation = StartCoroutine(AnimateFadeAndScale(0, 1, Vector3.zero, originalScale));
                    break;
                    
                case AnimationType.SlideFromTop:
                    Vector2 startPos = new Vector2(originalPosition.x, originalPosition.y + Screen.height);
                    currentAnimation = StartCoroutine(AnimatePosition(startPos, originalPosition));
                    break;
                    
                case AnimationType.SlideFromBottom:
                    startPos = new Vector2(originalPosition.x, originalPosition.y - Screen.height);
                    currentAnimation = StartCoroutine(AnimatePosition(startPos, originalPosition));
                    break;
                    
                case AnimationType.SlideFromLeft:
                    startPos = new Vector2(originalPosition.x - Screen.width, originalPosition.y);
                    currentAnimation = StartCoroutine(AnimatePosition(startPos, originalPosition));
                    break;
                    
                case AnimationType.SlideFromRight:
                    startPos = new Vector2(originalPosition.x + Screen.width, originalPosition.y);
                    currentAnimation = StartCoroutine(AnimatePosition(startPos, originalPosition));
                    break;
            }
        }
        else
        {
            // Disable interaction immediately
            if (disableRaycastOnHide)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            
            // Play sound if enabled
            if (playSound && EnhancedUIManager.instance != null)
            {
                EnhancedUIManager.instance.PlayButtonClickSound();
            }
            
            // Start animation based on type
            switch (animationType)
            {
                case AnimationType.Fade:
                    currentAnimation = StartCoroutine(AnimateFade(1, 0));
                    break;
                    
                case AnimationType.Scale:
                    currentAnimation = StartCoroutine(AnimateScale(originalScale, Vector3.zero));
                    break;
                    
                case AnimationType.FadeAndScale:
                    currentAnimation = StartCoroutine(AnimateFadeAndScale(1, 0, originalScale, Vector3.zero));
                    break;
                    
                case AnimationType.SlideFromTop:
                    Vector2 endPos = new Vector2(originalPosition.x, originalPosition.y + Screen.height);
                    currentAnimation = StartCoroutine(AnimatePosition(originalPosition, endPos));
                    break;
                    
                case AnimationType.SlideFromBottom:
                    endPos = new Vector2(originalPosition.x, originalPosition.y - Screen.height);
                    currentAnimation = StartCoroutine(AnimatePosition(originalPosition, endPos));
                    break;
                    
                case AnimationType.SlideFromLeft:
                    endPos = new Vector2(originalPosition.x - Screen.width, originalPosition.y);
                    currentAnimation = StartCoroutine(AnimatePosition(originalPosition, endPos));
                    break;
                    
                case AnimationType.SlideFromRight:
                    endPos = new Vector2(originalPosition.x + Screen.width, originalPosition.y);
                    currentAnimation = StartCoroutine(AnimatePosition(originalPosition, endPos));
                    break;
            }
        }
    }
    
    private IEnumerator AnimateFade(float startAlpha, float endAlpha)
    {
        float time = 0;
        canvasGroup.alpha = startAlpha;
        
        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = animationCurve.Evaluate(time / animationDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
        
        // If we were hiding and alpha is now 0, deactivate the gameObject
        if (!isVisible && endAlpha == 0)
        {
            if (destroyOnHide)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
    
    private IEnumerator AnimateScale(Vector3 startScale, Vector3 endScale)
    {
        float time = 0;
        rectTransform.localScale = startScale;
        
        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = animationCurve.Evaluate(time / animationDuration);
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        
        rectTransform.localScale = endScale;
        
        // If we were hiding and scale is now 0, deactivate the gameObject
        if (!isVisible && endScale == Vector3.zero)
        {
            if (destroyOnHide)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
    
    private IEnumerator AnimateFadeAndScale(float startAlpha, float endAlpha, Vector3 startScale, Vector3 endScale)
    {
        float time = 0;
        canvasGroup.alpha = startAlpha;
        rectTransform.localScale = startScale;
        
        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = animationCurve.Evaluate(time / animationDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
        rectTransform.localScale = endScale;
        
        // If we were hiding and both alpha and scale are now 0, deactivate the gameObject
        if (!isVisible && endAlpha == 0 && endScale == Vector3.zero)
        {
            if (destroyOnHide)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
    
    private IEnumerator AnimatePosition(Vector2 startPos, Vector2 endPos)
    {
        float time = 0;
        rectTransform.anchoredPosition = startPos;
        
        // If opening, also fade in
        if (isVisible)
            canvasGroup.alpha = 0;
        
        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = animationCurve.Evaluate(time / animationDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            
            // Also fade if necessary
            if (isVisible)
                canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            else
                canvasGroup.alpha = Mathf.Lerp(1, 0, t);
                
            yield return null;
        }
        
        rectTransform.anchoredPosition = endPos;
        canvasGroup.alpha = isVisible ? 1 : 0;
        
        // If we were hiding, deactivate the gameObject
        if (!isVisible)
        {
            if (destroyOnHide)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Returns whether the panel is currently visible
    /// </summary>
    public bool IsVisible()
    {
        return isVisible;
    }
} 