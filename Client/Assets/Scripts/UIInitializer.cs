/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Initializes the enhanced UI system on game startup.
/// This script is added to the scene as an Initializer component.
/// </summary>
public class UIInitializer : MonoBehaviour 
{
    [Header("UI System References")]
    public GameObject enhancedUIManagerPrefab;
    
    [Header("UI Assets")]
    public Sprite buttonBackground;
    public Sprite panelBackground;
    public Material textMaterial;
    
    void Awake()
    {
        Debug.Log("Initializing Enhanced UI System...");
        
        // Find GameManager
        GameManager gameManager = GameManager.singleton;
        if (gameManager == null)
        {
            Debug.LogError("Cannot find GameManager!");
            return;
        }
        
        // Check if an EnhancedUIManager already exists
        EnhancedUIManager existingManager = gameManager.GetComponent<EnhancedUIManager>();
        if (existingManager != null)
        {
            Debug.Log("Enhanced UI Manager already exists.");
            return;
        }
        
        // Create or add EnhancedUIManager
        EnhancedUIManager uiManager;
        if (enhancedUIManagerPrefab != null)
        {
            // Instantiate the prefab and attach to GameManager
            GameObject enhancedUI = Instantiate(enhancedUIManagerPrefab, gameManager.transform);
            uiManager = enhancedUI.GetComponent<EnhancedUIManager>();
        }
        else
        {
            // Add the component directly
            uiManager = gameManager.gameObject.AddComponent<EnhancedUIManager>();
        }
        
        // Create a Resources/UI directory if it doesn't exist
        if (!System.IO.Directory.Exists(Application.dataPath + "/Resources/UI"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/UI");
            Debug.Log("Created Resources/UI directory");
        }
        
        // Add UIEnhancer component
        UIEnhancer enhancer = gameManager.gameObject.AddComponent<UIEnhancer>();
        
        // Create notification prefab
        CreateNotificationPrefab();
        
        // Apply UI theme to main panels
        StartCoroutine(DelayedEnhancement(enhancer));
        
        Debug.Log("Enhanced UI System initialized successfully!");
    }
    
    private IEnumerator DelayedEnhancement(UIEnhancer enhancer)
    {
        // Wait for the next frame to make sure all UI elements are initialized
        yield return null;
        
        if (enhancer != null)
        {
            // Apply the enhancement to all UI
            enhancer.EnhanceAllUI();
        }
    }
    
    private void CreateNotificationPrefab()
    {
        // Create a notification panel prefab
        GameObject notificationObj = new GameObject("NotificationPanel");
        RectTransform rectTransform = notificationObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.anchoredPosition = new Vector2(0, -50);
        rectTransform.sizeDelta = new Vector2(500, 100);
        
        // Add panel image
        Image panelImage = notificationObj.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        if (panelBackground != null)
            panelImage.sprite = panelBackground;
        
        // Add text
        GameObject textObj = new GameObject("NotificationText");
        textObj.transform.SetParent(notificationObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 10);
        textRect.offsetMax = new Vector2(-10, -10);
        
        Text notificationText = textObj.AddComponent<Text>();
        notificationText.text = "Notification Text";
        notificationText.alignment = TextAnchor.MiddleCenter;
        notificationText.color = Color.white;
        notificationText.fontSize = 24;
        
        // Add outline
        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.5f);
        outline.effectDistance = new Vector2(1, -1);
        
        // Add animation
        NotificationSystem notificationSystem = notificationObj.AddComponent<NotificationSystem>();
        
        // Set inactive by default
        notificationObj.SetActive(false);
        
        // Find GameManager.panel_Game and add notification as child
        GameManager gameManager = GameManager.singleton;
        if (gameManager != null && gameManager.panel_Game != null)
        {
            notificationObj.transform.SetParent(gameManager.panel_Game.transform, false);
        }
        
        // Save as prefab in Resources folder
        #if UNITY_EDITOR
        // This code only executes in the Unity Editor
        // UnityEditor.PrefabUtility.SaveAsPrefabAsset(notificationObj, "Assets/Resources/UI/NotificationPanel.prefab");
        #endif
    }
} 