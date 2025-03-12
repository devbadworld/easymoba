/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Notification System that displays visually appealing in-game alerts and messages.
/// </summary>
public class NotificationSystem : MonoBehaviour
{
    public static NotificationSystem instance;
    
    [Header("Notification Settings")]
    public GameObject notificationPrefab;
    public Transform notificationContainer;
    public float displayDuration = 3f;
    public int maxNotifications = 3;
    public float notificationSpacing = 5f;
    
    [Header("Animation Settings")]
    public float fadeInTime = 0.2f;
    public float fadeOutTime = 0.3f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Style Settings")]
    public Color infoColor = new Color(0.2f, 0.6f, 1f, 0.9f);
    public Color successColor = new Color(0.2f, 0.8f, 0.2f, 0.9f);
    public Color warningColor = new Color(1f, 0.8f, 0.2f, 0.9f);
    public Color errorColor = new Color(1f, 0.3f, 0.3f, 0.9f);
    
    [Header("Sound Settings")]
    public AudioClip infoSound;
    public AudioClip successSound;
    public AudioClip warningSound;
    public AudioClip errorSound;
    
    // Private variables
    private Queue<NotificationItem> notificationQueue = new Queue<NotificationItem>();
    private List<GameObject> activeNotifications = new List<GameObject>();
    private AudioSource audioSource;
    
    private class NotificationItem
    {
        public string message;
        public NotificationType type;
        public float duration;
        
        public NotificationItem(string message, NotificationType type, float duration)
        {
            this.message = message;
            this.type = type;
            this.duration = duration;
        }
    }
    
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }
    
    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        // Add audio source if needed
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Create notification container if needed
        if (notificationContainer == null)
        {
            GameObject container = new GameObject("NotificationContainer");
            container.transform.SetParent(transform);
            
            RectTransform rect = container.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = Vector2.zero;
            
            notificationContainer = container.transform;
        }
    }
    
    /// <summary>
    /// Display an info notification
    /// </summary>
    public void ShowInfo(string message, float duration = -1)
    {
        Show(message, NotificationType.Info, duration);
    }
    
    /// <summary>
    /// Display a success notification
    /// </summary>
    public void ShowSuccess(string message, float duration = -1)
    {
        Show(message, NotificationType.Success, duration);
    }
    
    /// <summary>
    /// Display a warning notification
    /// </summary>
    public void ShowWarning(string message, float duration = -1)
    {
        Show(message, NotificationType.Warning, duration);
    }
    
    /// <summary>
    /// Display an error notification
    /// </summary>
    public void ShowError(string message, float duration = -1)
    {
        Show(message, NotificationType.Error, duration);
    }
    
    /// <summary>
    /// Show a notification with the specified type
    /// </summary>
    public void Show(string message, NotificationType type, float duration = -1)
    {
        if (duration < 0)
            duration = displayDuration;
            
        // Add to queue
        notificationQueue.Enqueue(new NotificationItem(message, type, duration));
        
        // Process queue
        ProcessQueue();
    }
    
    /// <summary>
    /// Process the notification queue
    /// </summary>
    private void ProcessQueue()
    {
        // If we have space for more notifications and items in the queue, show them
        while (activeNotifications.Count < maxNotifications && notificationQueue.Count > 0)
        {
            NotificationItem item = notificationQueue.Dequeue();
            DisplayNotification(item);
        }
    }
    
    /// <summary>
    /// Display a notification on the screen
    /// </summary>
    private void DisplayNotification(NotificationItem item)
    {
        // Create notification object
        GameObject notification = Instantiate(notificationPrefab, notificationContainer);
        activeNotifications.Add(notification);
        
        // Position notification
        RectTransform rect = notification.GetComponent<RectTransform>();
        float yOffset = rect.rect.height * activeNotifications.Count + notificationSpacing * (activeNotifications.Count - 1);
        rect.anchoredPosition = new Vector2(0, -yOffset);
        
        // Get notification components
        CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = notification.AddComponent<CanvasGroup>();
            
        Image backgroundImage = notification.GetComponent<Image>();
        Text messageText = notification.GetComponentInChildren<Text>();
        
        // Find icon if it exists
        Image iconImage = null;
        Transform iconTransform = notification.transform.Find("Icon");
        if (iconTransform != null)
            iconImage = iconTransform.GetComponent<Image>();
        
        // Set notification content
        if (messageText != null)
            messageText.text = item.message;
            
        // Set color based on type
        Color backgroundColor = infoColor;
        switch (item.type)
        {
            case NotificationType.Info:
                backgroundColor = infoColor;
                PlaySound(infoSound);
                break;
                
            case NotificationType.Success:
                backgroundColor = successColor;
                PlaySound(successSound);
                break;
                
            case NotificationType.Warning:
                backgroundColor = warningColor;
                PlaySound(warningSound);
                break;
                
            case NotificationType.Error:
                backgroundColor = errorColor;
                PlaySound(errorSound);
                break;
        }
        
        if (backgroundImage != null)
            backgroundImage.color = backgroundColor;
            
        if (iconImage != null)
        {
            // Set icon based on type (this assumes you have different sprites for each type)
            iconImage.sprite = GetIconForType(item.type);
        }
        
        // Start animation
        StartCoroutine(AnimateNotification(notification, canvasGroup, rect, item.duration));
    }
    
    /// <summary>
    /// Animate a notification (fade in, wait, fade out)
    /// </summary>
    private IEnumerator AnimateNotification(GameObject notification, CanvasGroup canvasGroup, RectTransform rectTransform, float duration)
    {
        // Initial setup
        canvasGroup.alpha = 0;
        Vector2 startPosition = rectTransform.anchoredPosition - new Vector2(50, 0);
        Vector2 targetPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = startPosition;
        
        // Fade in
        float timer = 0;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            float t = animationCurve.Evaluate(timer / fadeInTime);
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        canvasGroup.alpha = 1;
        rectTransform.anchoredPosition = targetPosition;
        
        // Wait for display duration
        yield return new WaitForSeconds(duration);
        
        // Fade out
        timer = 0;
        startPosition = targetPosition;
        targetPosition = targetPosition + new Vector2(50, 0);
        
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            float t = animationCurve.Evaluate(timer / fadeOutTime);
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        // Remove from active notifications
        activeNotifications.Remove(notification);
        Destroy(notification);
        
        // Check if we can show more notifications
        ProcessQueue();
        
        // Reposition remaining notifications
        RepositionNotifications();
    }
    
    /// <summary>
    /// Reposition active notifications after one is removed
    /// </summary>
    private void RepositionNotifications()
    {
        for (int i = 0; i < activeNotifications.Count; i++)
        {
            RectTransform rect = activeNotifications[i].GetComponent<RectTransform>();
            float targetY = -(rect.rect.height * (i + 1) + notificationSpacing * i);
            
            // Animate to new position
            StartCoroutine(AnimateRepositioning(rect, targetY));
        }
    }
    
    /// <summary>
    /// Animate repositioning of a notification
    /// </summary>
    private IEnumerator AnimateRepositioning(RectTransform rect, float targetY)
    {
        Vector2 startPos = rect.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, targetY);
        
        float timer = 0;
        float duration = 0.3f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = animationCurve.Evaluate(timer / duration);
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        
        rect.anchoredPosition = targetPos;
    }
    
    /// <summary>
    /// Get icon sprite for notification type
    /// </summary>
    private Sprite GetIconForType(NotificationType type)
    {
        // This should be replaced with your actual implementation to get icons
        // For now, return null so that the system will work without icons
        return null;
    }
    
    /// <summary>
    /// Play notification sound
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
} 