/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Enhanced UI Visibility component for panels with smooth fade transitions.
/// This script is a drop-in replacement for the standard UIVisibility component.
/// </summary>
public class EnhancedUIVisibility : MonoBehaviour 
{
    [Header("Visibility Settings")]
    public string visibilityKey = "";
    public bool startActive = false;
    
    [Header("Animation Settings")]
    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.3f;
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    // Private variables
    private CanvasGroup canvasGroup;
    private bool isAnimating = false;
    private Coroutine animationCoroutine;
    private bool currentVisibility = false;
    
    void Awake()
    {
        // Get or add CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Set initial state
        currentVisibility = startActive;
        UpdateVisualState(false); // No animation for initial state
    }
    
    void OnEnable()
    {
        // Re-register with GameManager if needed
        RegisterWithGameManager();
    }
    
    void Start()
    {
        // Register with GameManager for visibility changes
        RegisterWithGameManager();
    }
    
    private void RegisterWithGameManager()
    {
        if (string.IsNullOrEmpty(visibilityKey)) return;
        
        // We keep compatibility with the original UIVisibility system
        GameManager.singleton.panelChanges += OnPanelChange;
    }
    
    void OnDestroy()
    {
        // Unregister from GameManager
        if (GameManager.singleton != null)
        {
            GameManager.singleton.panelChanges -= OnPanelChange;
        }
    }
    
    public void OnPanelChange(string key, bool value)
    {
        if (key == visibilityKey && currentVisibility != value)
        {
            SetVisible(value);
        }
    }
    
    public void SetVisible(bool visible, bool animate = true)
    {
        // Only change if needed
        if (currentVisibility == visible) return;
        
        currentVisibility = visible;
        
        // Stop any ongoing animation
        if (isAnimating && animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            isAnimating = false;
        }
        
        // Update visibility with or without animation
        if (animate)
        {
            animationCoroutine = StartCoroutine(AnimateVisibility(visible));
        }
        else
        {
            UpdateVisualState(false);
        }
    }
    
    private void UpdateVisualState(bool animated)
    {
        // Set alpha and interactability
        canvasGroup.alpha = currentVisibility ? 1f : 0f;
        canvasGroup.interactable = currentVisibility;
        canvasGroup.blocksRaycasts = currentVisibility;
        
        // Enable/disable gameObject if not animating
        if (!animated)
        {
            gameObject.SetActive(currentVisibility);
        }
    }
    
    private IEnumerator AnimateVisibility(bool visible)
    {
        isAnimating = true;
        
        // Make sure object is active for animation
        gameObject.SetActive(true);
        
        // Get correct animation settings
        float duration = visible ? fadeInDuration : fadeOutDuration;
        AnimationCurve curve = visible ? fadeInCurve : fadeOutCurve;
        float startAlpha = canvasGroup.alpha;
        float targetAlpha = visible ? 1f : 0f;
        
        // Animation
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float curveValue = curve.Evaluate(t);
            
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, curveValue);
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
            
            yield return null;
        }
        
        // Ensure we reach the target value
        canvasGroup.alpha = targetAlpha;
        
        // Disable gameObject if hidden
        if (!visible)
        {
            gameObject.SetActive(false);
        }
        
        isAnimating = false;
    }
} 