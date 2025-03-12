/*! 
@author UI Enhancement System
@created October 30, 2023
*/

using UnityEngine;

// This class is automatically executed before any scene loads
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
public static class UIEnhancementPreloader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeUIEnhancement()
    {
        Debug.Log("[UIEnhancementPreloader] Preloading UI Enhancement System");
        
        // Create the bootstrapper game object
        GameObject bootstrapperObj = new GameObject("[UI_BOOTSTRAP]");
        Object.DontDestroyOnLoad(bootstrapperObj);
        
        // Add the bootstrapper component
        bootstrapperObj.AddComponent<UIBootstrapper>();
        
        Debug.Log("[UIEnhancementPreloader] UI Enhancement System preloaded successfully");
    }
} 