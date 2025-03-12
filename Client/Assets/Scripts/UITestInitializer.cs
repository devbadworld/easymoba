/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Simple test initializer to confirm the UI enhancement system is being loaded.
/// This creates a visible message on the screen to indicate the system is active.
/// </summary>
public class UITestInitializer : MonoBehaviour 
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnGameStart()
    {
        // Create a canvas for our test message
        GameObject canvasObj = new GameObject("TestCanvas");
        DontDestroyOnLoad(canvasObj);
        
        // Add canvas components
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // Ensure it's on top
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create text object
        GameObject textObj = new GameObject("TestText");
        textObj.transform.SetParent(canvasObj.transform, false);
        
        // Set up transform
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 1);
        textRect.anchorMax = new Vector2(0, 1);
        textRect.pivot = new Vector2(0, 1);
        textRect.anchoredPosition = new Vector2(10, -10);
        textRect.sizeDelta = new Vector2(400, 50);
        
        // Add text component
        Text text = textObj.AddComponent<Text>();
        text.text = "Enhanced UI Loaded - v1.0";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 20;
        text.color = new Color(1f, 0.5f, 0f, 1f); // Orange
        
        // Add background image
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(textRect, false);
        
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.5f);
        
        // Move text to front
        textObj.transform.SetAsLastSibling();
        
        Debug.Log("UI Test Initializer has created visible test message");
        
        // Destroy after 15 seconds
        GameObject.Destroy(canvasObj, 15f);
    }
} 