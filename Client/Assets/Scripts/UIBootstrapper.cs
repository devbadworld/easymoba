/*! 
@author UI Enhancement System
@created October 30, 2023
*/

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Bootstraps the UI enhancement system across all scenes.
/// This class runs independently and ensures UI enhancements are applied.
/// Add this to ANY GameObject in the first scene to guarantee enhancements run.
/// </summary>
[DefaultExecutionOrder(-1000)] // Ultra high priority - run before anything else
public class UIBootstrapper : MonoBehaviour
{
    // Static reference to ensure it's only created once
    private static UIBootstrapper instance;
    
    // Bright, unmissable colors to prove our enhancements are working
    public Color primaryColor = new Color(1f, 0f, 0f, 1f); // Pure RED
    public Color secondaryColor = new Color(1f, 0.5f, 0f, 1f); // Bright ORANGE
    
    private GameObject enhancerObject;
    
    void Awake()
    {
        // Singleton pattern
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        Debug.Log("[UIBootstrapper] UI Enhancement System initialized");
        
        // Create the enhancer object immediately
        CreateEnhancerObject();
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[UIBootstrapper] Scene loaded: {scene.name}. Re-applying UI enhancements.");
        
        // Ensure the enhancer exists
        if (enhancerObject == null)
        {
            CreateEnhancerObject();
        }
        
        // Force UI enhancement
        FallbackUIEnhancer enhancer = enhancerObject.GetComponent<FallbackUIEnhancer>();
        if (enhancer != null)
        {
            enhancer.SendMessage("EnhanceAllUI", null, SendMessageOptions.DontRequireReceiver);
        }
    }
    
    void CreateEnhancerObject()
    {
        enhancerObject = new GameObject("[UI_ENHANCEMENT_SYSTEM]");
        DontDestroyOnLoad(enhancerObject);
        
        // Add the enhancer component
        FallbackUIEnhancer enhancer = enhancerObject.AddComponent<FallbackUIEnhancer>();
        enhancer.primaryColor = primaryColor;
        enhancer.accentColor = secondaryColor;
        
        Debug.Log("[UIBootstrapper] Created UI Enhancement System GameObject");
        
        // Create unmistakable UI version indicator that can't be missed
        CreateVersionIndicator();
    }
    
    void CreateVersionIndicator()
    {
        // Create a persistent object for the version indicator
        GameObject indicatorObj = new GameObject("UI_VERSION_INDICATOR");
        indicatorObj.transform.SetParent(enhancerObject.transform);
        DontDestroyOnLoad(indicatorObj);
        
        // Add canvas for the indicator to ensure it's always on top
        Canvas canvas = indicatorObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767; // Maximum value to ensure it's on top
        
        // Add a canvas scaler for proper scaling
        UnityEngine.UI.CanvasScaler scaler = indicatorObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Create a background with very obvious styling
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(indicatorObj.transform, false);
        
        UnityEngine.UI.RectTransform bgRect = bgObj.AddComponent<UnityEngine.UI.RectTransform>();
        bgRect.anchorMin = new Vector2(0, 1);
        bgRect.anchorMax = new Vector2(1, 1);
        bgRect.pivot = new Vector2(0.5f, 1);
        bgRect.sizeDelta = new Vector2(0, 40);
        bgRect.anchoredPosition = Vector2.zero;
        
        UnityEngine.UI.Image bgImage = bgObj.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(0, 0, 0, 0.8f);
        
        // Create text object
        GameObject textObj = new GameObject("VersionText");
        textObj.transform.SetParent(bgObj.transform, false);
        
        UnityEngine.UI.RectTransform textRect = textObj.AddComponent<UnityEngine.UI.RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        UnityEngine.UI.Text text = textObj.AddComponent<UnityEngine.UI.Text>();
        text.text = "UI ENHANCEMENT SYSTEM V2.0 - NEW VERSION ACTIVE";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 20;
        text.fontStyle = UnityEngine.FontStyle.Bold;
        text.color = primaryColor;
        text.alignment = TextAnchor.MiddleCenter;
        
        // Add outline
        UnityEngine.UI.Outline outline = textObj.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(1, -1);
        
        Debug.Log("[UIBootstrapper] Created permanent version indicator");
    }
} 