/*! 
@author UI Enhancement System
@created October 30, 2023
*/

using UnityEngine;

/// <summary>
/// Initializes the FallbackUIEnhancer when the game starts.
/// This script should be added to the GameManager object.
/// </summary>
[DefaultExecutionOrder(-100)] // Make sure this runs before anything else
public class FallbackUIInitializer : MonoBehaviour 
{
    // Global singleton instance to ensure only one exists
    private static FallbackUIInitializer instance;
    
    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(this);
        
        Debug.Log("[FallbackUIInitializer] Initializing UI Enhancer");
        
        // Create the FallbackUIEnhancer
        GameObject enhancerObj = new GameObject("FallbackUIEnhancer");
        enhancerObj.AddComponent<FallbackUIEnhancer>();
        DontDestroyOnLoad(enhancerObj);
        
        // Set bright red color to make it very obvious
        FallbackUIEnhancer enhancer = enhancerObj.GetComponent<FallbackUIEnhancer>();
        enhancer.primaryColor = new Color(1f, 0.2f, 0.2f, 1f); // BRIGHT RED
        enhancer.accentColor = new Color(1f, 0.5f, 0f, 1f);    // ORANGE
    }
} 