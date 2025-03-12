/*! 
@author UI Enhancement System
@created October 30, 2023
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Adds a hover animation effect to buttons
/// </summary>
public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public float hoverScaleMultiplier = 1.1f;
    public float animationSpeed = 3f;
    public Graphic targetGraphic;
    
    private Vector3 originalScale;
    private Vector3 hoverScale;
    private Vector3 targetScale;
    private bool isTransitioning = false;
    
    void Start()
    {
        // Store the original scale
        originalScale = transform.localScale;
        hoverScale = originalScale * hoverScaleMultiplier;
        targetScale = originalScale;
        
        // If no target graphic specified, try to get it from this gameobject
        if (targetGraphic == null)
            targetGraphic = GetComponent<Graphic>();
    }
    
    void Update()
    {
        // Only animate if we're transitioning
        if (isTransitioning)
        {
            // Smoothly interpolate to the target scale
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
            
            // Check if we're close enough to the target to stop transitioning
            if (Vector3.Distance(transform.localScale, targetScale) < 0.01f)
            {
                transform.localScale = targetScale;
                isTransitioning = false;
            }
        }
    }
    
    // Called when the pointer enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Scale up on hover
        targetScale = hoverScale;
        isTransitioning = true;
        
        // Change color of the button if there's a graphic
        if (targetGraphic != null)
        {
            // Store the original color if we haven't already
            if (originalColor == Color.clear)
                originalColor = targetGraphic.color;
                
            // Brighten the color slightly
            targetGraphic.color = new Color(
                Mathf.Min(originalColor.r * 1.2f, 1f),
                Mathf.Min(originalColor.g * 1.2f, 1f),
                Mathf.Min(originalColor.b * 1.2f, 1f),
                originalColor.a
            );
        }
    }
    
    // Called when the pointer leaves the button
    public void OnPointerExit(PointerEventData eventData)
    {
        // Scale back to original size
        targetScale = originalScale;
        isTransitioning = true;
        
        // Restore original color
        if (targetGraphic != null && originalColor != Color.clear)
        {
            targetGraphic.color = originalColor;
        }
    }
    
    // Called when the button is pressed
    public void OnPointerDown(PointerEventData eventData)
    {
        // Scale down slightly when pressed
        targetScale = originalScale * 0.95f;
        isTransitioning = true;
    }
    
    // Called when the button is released
    public void OnPointerUp(PointerEventData eventData)
    {
        // Go back to hover scale if still hovering, otherwise to original scale
        if (RectTransformUtility.RectangleContainsScreenPoint(
            GetComponent<RectTransform>(), 
            eventData.position, 
            eventData.pressEventCamera))
        {
            targetScale = hoverScale;
        }
        else
        {
            targetScale = originalScale;
            
            // Restore original color
            if (targetGraphic != null && originalColor != Color.clear)
            {
                targetGraphic.color = originalColor;
            }
        }
        
        isTransitioning = true;
    }
    
    // Store the original color of the graphic
    private Color originalColor = Color.clear;
} 