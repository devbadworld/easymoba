/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Modern tooltip system that provides informative and visually appealing tooltips for UI elements.
/// </summary>
public class ModernTooltipSystem : MonoBehaviour 
{
    public static ModernTooltipSystem instance;
    
    [Header("Tooltip References")]
    public GameObject tooltipPrefab;
    public RectTransform canvasRect;
    
    [Header("Tooltip Settings")]
    public float fadeInTime = 0.15f;
    public float fadeOutTime = 0.1f;
    public Vector2 offset = new Vector2(15, 15);
    public float padding = 10f;
    
    private GameObject currentTooltip;
    private RectTransform tooltipRect;
    private CanvasGroup tooltipCanvasGroup;
    private Text tooltipTitleText;
    private Text tooltipContentText;
    private Image tooltipIcon;
    private RectTransform tooltipBackgroundRect;
    
    private Coroutine showHideCoroutine;
    
    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }
    
    /// <summary>
    /// Show a tooltip with the given title and content
    /// </summary>
    public void ShowTooltip(string title, string content, Vector2 position, Sprite icon = null)
    {
        // Create tooltip if it doesn't exist
        if (currentTooltip == null)
        {
            CreateTooltip();
        }
        
        // Update tooltip content
        if (tooltipTitleText != null)
        {
            tooltipTitleText.text = title;
            tooltipTitleText.gameObject.SetActive(!string.IsNullOrEmpty(title));
        }
        
        if (tooltipContentText != null)
        {
            tooltipContentText.text = content;
        }
        
        if (tooltipIcon != null)
        {
            if (icon != null)
            {
                tooltipIcon.sprite = icon;
                tooltipIcon.gameObject.SetActive(true);
            }
            else
            {
                tooltipIcon.gameObject.SetActive(false);
            }
        }
        
        // Show tooltip
        currentTooltip.SetActive(true);
        
        // Position tooltip
        PositionTooltip(position);
        
        // Animate tooltip
        if (showHideCoroutine != null)
        {
            StopCoroutine(showHideCoroutine);
        }
        
        showHideCoroutine = StartCoroutine(FadeInTooltip());
    }
    
    /// <summary>
    /// Hide the current tooltip
    /// </summary>
    public void HideTooltip()
    {
        if (currentTooltip == null) return;
        
        if (showHideCoroutine != null)
        {
            StopCoroutine(showHideCoroutine);
        }
        
        showHideCoroutine = StartCoroutine(FadeOutTooltip());
    }
    
    /// <summary>
    /// Create tooltip game object
    /// </summary>
    private void CreateTooltip()
    {
        // Instantiate tooltip prefab
        currentTooltip = Instantiate(tooltipPrefab, transform);
        tooltipRect = currentTooltip.GetComponent<RectTransform>();
        tooltipCanvasGroup = currentTooltip.GetComponent<CanvasGroup>();
        
        if (tooltipCanvasGroup == null)
        {
            tooltipCanvasGroup = currentTooltip.AddComponent<CanvasGroup>();
        }
        
        // Get tooltip components
        tooltipTitleText = currentTooltip.transform.Find("Title")?.GetComponent<Text>();
        tooltipContentText = currentTooltip.transform.Find("Content")?.GetComponent<Text>();
        tooltipIcon = currentTooltip.transform.Find("Icon")?.GetComponent<Image>();
        tooltipBackgroundRect = currentTooltip.GetComponent<RectTransform>();
        
        // Initial state
        tooltipCanvasGroup.alpha = 0;
        currentTooltip.SetActive(false);
    }
    
    /// <summary>
    /// Position tooltip based on pointer position
    /// </summary>
    private void PositionTooltip(Vector2 position)
    {
        if (tooltipRect == null || canvasRect == null) return;
        
        // Calculate tooltip size
        Vector2 tooltipSize = tooltipBackgroundRect.sizeDelta;
        
        // Convert screen position to canvas position
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, position, null, out canvasPosition);
        
        // Default position (below and to the right of the pointer)
        Vector2 tooltipPosition = canvasPosition + offset;
        
        // Adjust position if tooltip goes out of screen bounds
        if (tooltipPosition.x + tooltipSize.x > canvasRect.rect.width - padding)
        {
            // Tooltip goes off the right edge, position to the left
            tooltipPosition.x = canvasPosition.x - offset.x - tooltipSize.x;
        }
        
        if (tooltipPosition.y - tooltipSize.y < -canvasRect.rect.height + padding)
        {
            // Tooltip goes off the bottom edge, position above
            tooltipPosition.y = canvasPosition.y + offset.y + tooltipSize.y;
        }
        
        // Set position
        tooltipRect.anchoredPosition = tooltipPosition;
    }
    
    /// <summary>
    /// Animate tooltip fade in
    /// </summary>
    private IEnumerator FadeInTooltip()
    {
        float timer = 0;
        
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeInTime);
            tooltipCanvasGroup.alpha = alpha;
            yield return null;
        }
        
        tooltipCanvasGroup.alpha = 1;
    }
    
    /// <summary>
    /// Animate tooltip fade out
    /// </summary>
    private IEnumerator FadeOutTooltip()
    {
        float timer = 0;
        
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / fadeOutTime);
            tooltipCanvasGroup.alpha = alpha;
            yield return null;
        }
        
        tooltipCanvasGroup.alpha = 0;
        currentTooltip.SetActive(false);
    }
}

/// <summary>
/// Component to add to UI elements that should display tooltips
/// </summary>
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [Header("Tooltip Content")]
    public string tooltipTitle;
    [TextArea(1, 5)]
    public string tooltipContent;
    public Sprite tooltipIcon;
    
    [Header("Tooltip Behavior")]
    public float showDelay = 0.5f;
    public bool followMouse = false;
    
    private bool isPointerOver = false;
    private Coroutine showTooltipCoroutine;
    
    void OnDisable()
    {
        // Hide tooltip if this object is disabled
        if (isPointerOver && ModernTooltipSystem.instance != null)
        {
            ModernTooltipSystem.instance.HideTooltip();
            isPointerOver = false;
        }
        
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
            showTooltipCoroutine = null;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
        }
        
        if (string.IsNullOrEmpty(tooltipContent)) return;
        
        showTooltipCoroutine = StartCoroutine(ShowTooltipAfterDelay(eventData));
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
            showTooltipCoroutine = null;
        }
        
        if (ModernTooltipSystem.instance != null)
        {
            ModernTooltipSystem.instance.HideTooltip();
        }
    }
    
    public void OnPointerMove(PointerEventData eventData)
    {
        if (followMouse && isPointerOver && ModernTooltipSystem.instance != null)
        {
            // Only update position if the tooltip is already showing
            if (showTooltipCoroutine == null)
            {
                ModernTooltipSystem.instance.ShowTooltip(tooltipTitle, tooltipContent, eventData.position, tooltipIcon);
            }
        }
    }
    
    private IEnumerator ShowTooltipAfterDelay(PointerEventData eventData)
    {
        yield return new WaitForSeconds(showDelay);
        
        if (isPointerOver && ModernTooltipSystem.instance != null)
        {
            ModernTooltipSystem.instance.ShowTooltip(tooltipTitle, tooltipContent, eventData.position, tooltipIcon);
        }
        
        showTooltipCoroutine = null;
    }
} 