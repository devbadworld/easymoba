/*! 
@author Enhanced UI for EasyMOBA
@lastupdate Tucker Branch
*/

using UnityEngine;
using System.Collections;

/// <summary>
/// Auto-initializer for the enhanced UI system.
/// This class automatically loads on game start and instantiates the UIEnhancer prefab.
/// </summary>
public class UIAutoInitializer : MonoBehaviour 
{
    // Static constructor to ensure this class is initialized on game start
    static UIAutoInitializer()
    {
        // Schedule initialization for next frame to ensure all systems are loaded
        GameObject initializer = new GameObject("UIAutoInitializer");
        DontDestroyOnLoad(initializer);
        initializer.AddComponent<UIAutoInitializer>();
        
        Debug.Log("UI Auto Initializer registered");
    }
    
    void Start()
    {
        // Instantiate the UIEnhancer prefab from Resources folder
        GameObject enhancerPrefab = Resources.Load<GameObject>("UIEnhancer");
        if (enhancerPrefab != null)
        {
            Instantiate(enhancerPrefab);
            Debug.Log("UI Enhancer prefab instantiated");
        }
        else
        {
            Debug.LogWarning("UI Enhancer prefab not found in Resources folder!");
            
            // Fallback: Add UIInitializer directly
            GameObject fallback = new GameObject("UIEnhancer_Fallback");
            fallback.AddComponent<UIInitializer>();
            Debug.Log("UI Initializer added as fallback");
        }
    }
    
    // This class implements the System.Runtime.CompilerServices.RuntimeInitializeOnLoadMethodAttribute
    // to ensure it's called automatically when the game starts
    [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnGameStart()
    {
        Debug.Log("Enhanced UI initialization started");
        // The static constructor will be called here
    }
} 