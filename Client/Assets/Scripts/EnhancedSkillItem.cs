/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// Enhanced Skill Item that extends the base UISkillItem with better visual feedback,
/// animations, and more modern UI elements for skills.
/// </summary>
public class EnhancedSkillItem : UISkillItem
{
    [Header("Enhanced Visual Settings")]
    public Image skillIcon;
    public Image cooldownOverlay;
    public Image borderGlow;
    public Text cooldownText;
    public Text keyText;
    public ParticleSystem readyEffect;
    
    [Header("Animation Settings")]
    public bool useAnimations = true;
    public float pulseIntensity = 0.1f;
    public float pulseDuration = 0.5f;
    
    [Header("Color Settings")]
    public Color availableColor = new Color(1f, 1f, 1f, 1f);
    public Color unavailableColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color cooldownColor = new Color(0.3f, 0.3f, 0.3f, 0.7f);
    public Color borderReadyColor = new Color(1f, 0.8f, 0.2f, 0.8f);
    
    [Header("References")]
    public UISkillItem baseSkillItem;
    public RectTransform skillIconRect;
    public Image skillBorder;
    public Text levelText;
    
    [Header("Tooltip Settings")]
    public bool useModernTooltip = true;
    public float tooltipDelay = 0.5f;
    
    // Private variables
    private float remainingCooldown = 0f;
    private float totalCooldown = 0f;
    private bool wasReady = true;
    private Vector3 originalScale;
    private Coroutine pulseCoroutine;
    private Color originalBorderColor;
    private bool isAvailable = true;
    private EnhancedUIManager uiManager;
    
    // References to skill info
    private MObjects.SkillInfo skillInfo;
    private bool hasEnoughMana = true;
    private AgentInput agentInput;
    
    void Awake()
    {
        // Get references if not set
        if (baseSkillItem == null) baseSkillItem = GetComponent<UISkillItem>();
        if (skillIconRect == null && skillIcon != null) skillIconRect = skillIcon.rectTransform;
        
        // Store original values
        if (skillIconRect != null) originalScale = skillIconRect.localScale;
        if (skillBorder != null) originalBorderColor = skillBorder.color;
        
        // Initialize cooldown overlay
        if (cooldownOverlay != null)
        {
            cooldownOverlay.type = Image.Type.Filled;
            cooldownOverlay.fillMethod = Image.FillMethod.Radial360;
            cooldownOverlay.fillOrigin = (int)Image.Origin360.Top;
            cooldownOverlay.fillClockwise = false;
            cooldownOverlay.fillAmount = 0f;
            cooldownOverlay.color = new Color(0f, 0f, 0f, 0.7f);
        }
        
        // Find UI Manager
        uiManager = FindObjectOfType<EnhancedUIManager>();
    }
    
    void Start()
    {
        // Store original scale for animations
        originalScale = transform.localScale;
        
        // Initialize UI elements
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f;
            cooldownOverlay.gameObject.SetActive(false);
        }
        
        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(false);
        }
        
        if (borderGlow != null)
        {
            borderGlow.gameObject.SetActive(false);
        }
        
        // Get reference to AgentInput
        if (GameManager.singleton != null)
        {
            agentInput = GameManager.singleton.GetComponent<AgentInput>();
        }
        
        // Connect to the base skill item for events
        if (baseSkillItem != null)
        {
            // Setup tooltip
            if (useModernTooltip && uiManager != null)
            {
                // Replace standard tooltip with modern tooltip
                SetupModernTooltip();
            }
        }
    }
    
    void OnEnable()
    {
        // Reset cooldown display
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f;
        }
        
        // Reset icon color
        if (skillIcon != null)
        {
            skillIcon.color = isAvailable ? availableColor : unavailableColor;
        }
        
        // Reset border
        if (skillBorder != null)
        {
            skillBorder.color = originalBorderColor;
        }
        
        // Reset scale
        if (skillIconRect != null)
        {
            skillIconRect.localScale = originalScale;
        }
    }
    
    public void Update()
    {
        // No need to update if we don't have a skill info or agent input
        if (skillInfo == null || agentInput == null) return;
        
        // Check if we need to update cooldown
        if (remainingCooldown > 0)
        {
            remainingCooldown -= Time.deltaTime;
            
            // Update cooldown overlay
            if (cooldownOverlay != null)
            {
                cooldownOverlay.fillAmount = remainingCooldown / totalCooldown;
            }
            
            // Update cooldown text
            if (cooldownText != null)
            {
                cooldownText.text = Mathf.Ceil(remainingCooldown).ToString();
            }
            
            // If cooldown just finished
            if (remainingCooldown <= 0)
            {
                remainingCooldown = 0;
                SetCooldownVisible(false);
                
                // Check if skill is now ready
                CheckSkillReadyStatus();
            }
        }
        else
        {
            // Always check skill readiness even when not on cooldown
            // This handles mana changes
            CheckSkillReadyStatus();
        }
        
        // Update cooldown visualization
        if (remainingCooldown > 0 && cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = remainingCooldown / totalCooldown;
        }
    }
    
    /// <summary>
    /// Set the skill info for this skill item
    /// </summary>
    public void SetSkillInfo(MObjects.SkillInfo info)
    {
        skillInfo = info;
        
        // Update visual elements based on the skill info
        if (skillIcon != null && info != null)
        {
            // Set the skill icon image based on skill info
            // This assumes you have a way to map skill types to sprites
            skillIcon.sprite = GetSkillSprite(info.skillId);
        }
        
        // Set key text if available
        if (keyText != null)
        {
            int skillIndex = transform.GetSiblingIndex();
            string keyBinding = GetKeyBindingText(skillIndex);
            keyText.text = keyBinding;
        }
    }
    
    /// <summary>
    /// Set the cooldown for this skill
    /// </summary>
    public void SetCooldown(float cooldown)
    {
        remainingCooldown = cooldown;
        totalCooldown = cooldown;
        
        if (cooldown > 0)
        {
            SetCooldownVisible(true);
        }
        else
        {
            SetCooldownVisible(false);
        }
    }
    
    /// <summary>
    /// Refresh the skill item based on mana availability
    /// </summary>
    public void RefreshManaCost(float currentMana)
    {
        if (skillInfo == null) return;
        
        bool hasMana = currentMana >= skillInfo.manaCost;
        
        // Only update if mana status changed
        if (hasEnoughMana != hasMana)
        {
            hasEnoughMana = hasMana;
            UpdateVisualState();
        }
    }
    
    /// <summary>
    /// Check if the skill is ready to use
    /// </summary>
    private void CheckSkillReadyStatus()
    {
        if (skillInfo == null) return;
        
        bool isReady = remainingCooldown <= 0 && hasEnoughMana;
        
        // If state changed from not ready to ready, play the ready effect
        if (!wasReady && isReady)
        {
            PlayReadyEffect();
        }
        
        wasReady = isReady;
        UpdateVisualState();
    }
    
    /// <summary>
    /// Update the visual state based on current conditions
    /// </summary>
    private void UpdateVisualState()
    {
        if (skillIcon == null) return;
        
        // On cooldown state takes precedence
        if (remainingCooldown > 0)
        {
            skillIcon.color = cooldownColor;
            
            if (borderGlow != null)
            {
                borderGlow.gameObject.SetActive(false);
            }
        }
        // Not enough mana
        else if (!hasEnoughMana)
        {
            skillIcon.color = unavailableColor;
            
            if (borderGlow != null)
            {
                borderGlow.gameObject.SetActive(false);
            }
        }
        // Ready to use
        else
        {
            skillIcon.color = availableColor;
            
            if (borderGlow != null)
            {
                borderGlow.gameObject.SetActive(true);
                borderGlow.color = borderReadyColor;
            }
        }
    }
    
    /// <summary>
    /// Show or hide the cooldown overlay and text
    /// </summary>
    private void SetCooldownVisible(bool visible)
    {
        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(visible);
        }
        
        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(visible);
        }
    }
    
    /// <summary>
    /// Play a visual effect when the skill becomes ready
    /// </summary>
    private void PlayReadyEffect()
    {
        // Play particle effect if available
        if (readyEffect != null)
        {
            readyEffect.Stop();
            readyEffect.Play();
        }
        
        // Pulse animation
        if (useAnimations)
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
            }
            
            pulseCoroutine = StartCoroutine(PulseAnimation());
        }
    }
    
    /// <summary>
    /// Pulse animation when skill becomes ready
    /// </summary>
    private IEnumerator PulseAnimation()
    {
        float time = 0;
        Vector3 targetScale = originalScale * (1 + pulseIntensity);
        
        // Pulse out
        while (time < pulseDuration / 2)
        {
            time += Time.deltaTime;
            float t = time / (pulseDuration / 2);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        // Pulse in
        time = 0;
        while (time < pulseDuration / 2)
        {
            time += Time.deltaTime;
            float t = time / (pulseDuration / 2);
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
    
    /// <summary>
    /// Get the sprite for a skill based on its ID
    /// </summary>
    private Sprite GetSkillSprite(int skillId)
    {
        // This should be replaced with your actual implementation to get skill icons
        if (GameManager.singleton != null)
        {
            // Assuming there's a method to get skill icons in GameManager
            // return GameManager.singleton.GetSkillIcon(skillId);
        }
        
        return skillIcon.sprite; // Fallback to current sprite
    }
    
    /// <summary>
    /// Get the key binding text for a skill based on its index
    /// </summary>
    private string GetKeyBindingText(int skillIndex)
    {
        switch (skillIndex)
        {
            case 0: return "Q";
            case 1: return "W";
            case 2: return "E";
            case 3: return "R";
            default: return (skillIndex + 1).ToString();
        }
    }
    
    // Override the base class methods to add our enhanced functionality
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        
        // Additional hover effects
        if (borderGlow != null && remainingCooldown <= 0 && hasEnoughMana)
        {
            borderGlow.color = new Color(borderReadyColor.r, borderReadyColor.g, borderReadyColor.b, 1f);
        }
    }
    
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        
        // Reset hover effects
        if (borderGlow != null && remainingCooldown <= 0 && hasEnoughMana)
        {
            borderGlow.color = borderReadyColor;
        }
    }
    
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        
        // Add click animation
        if (useAnimations)
        {
            StartCoroutine(ClickAnimation());
        }
    }
    
    private IEnumerator ClickAnimation()
    {
        Vector3 clickScale = originalScale * 0.9f;
        
        // Shrink
        float time = 0;
        float duration = 0.1f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.localScale = Vector3.Lerp(originalScale, clickScale, t);
            yield return null;
        }
        
        // Return to original
        time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.localScale = Vector3.Lerp(clickScale, originalScale, t);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
    
    private void SetupModernTooltip()
    {
        // Find or get references to tooltip components
        ModernTooltipSystem tooltipSystem = FindObjectOfType<ModernTooltipSystem>();
        
        if (tooltipSystem != null)
        {
            // Connect this skill item to the tooltip system
            tooltipSystem.RegisterTooltipTrigger(gameObject, UpdateTooltipContent, tooltipDelay);
        }
    }
    
    private string UpdateTooltipContent()
    {
        // Generate skill tooltip content
        string content = "";
        
        if (baseSkillItem != null && baseSkillItem.skillId >= 0)
        {
            GameManager gm = GameManager.singleton;
            if (gm != null)
            {
                // Add skill name and description
                content = $"<b>{baseSkillItem.mObject.name}</b>\n\n";
                content += baseSkillItem.mObject.description + "\n\n";
                
                // Add cooldown info
                if (remainingCooldown > 0)
                {
                    content += $"<color=#FF6A00>Cooldown: {remainingCooldown:F1}s remaining</color>\n";
                }
                else
                {
                    content += $"Cooldown: {baseSkillItem.mObject.cooldown}s\n";
                }
                
                // Add level requirement
                if (!hasEnoughMana)
                {
                    content += $"<color=#FF0000>Requires Level {baseSkillItem.mObject.reqLvl}</color>";
                }
            }
        }
        
        return content;
    }
} 