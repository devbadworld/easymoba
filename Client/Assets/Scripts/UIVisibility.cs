/*! 
@author EasyMOBA <easymoba.com>
@lastupdate 16 February 2018
*/

using UnityEngine;
using UnityEngine.UI;

public class UIVisibility : MonoBehaviour 
{
    // === ENHANCED UI START === //
    // Modified with modern visual enhancements
    public Color primaryColor = new Color(0.1f, 0.6f, 0.9f, 1f); // Vibrant blue theme
    public Color accentColor = new Color(1f, 0.5f, 0f, 1f);    // Bright orange accent - CHANGED
    public bool useEnhancedUI = true;                            // Enable enhanced UI
    
    // Animation properties - NEW
    private bool animateElements = true;
    private float animationSpeed = 3f;
    private float hoverScaleAmount = 1.1f;
    // === ENHANCED UI END === //
    
    public float aSpeed = 4f; 
    float defaultA = 1f;
    public float minAlpha = 0.01f;
    public float hideIn = 0;
	public CanvasGroup myCanvas;

	public bool showOnStart, falseOnHide = true, destroyOnHide = false;
    
    // Flag to track if this component has been enhanced
    private bool hasBeenEnhanced = false;
    
    // Cache for UI elements we find
    private Button[] cachedButtons;
    private Text[] cachedTexts;
    private Image[] cachedImages;

	void Start ()
	{
        if (showOnStart)
            Open();
            
        // Apply enhanced UI if enabled
        if (useEnhancedUI && !hasBeenEnhanced)
        {
            ApplyEnhancedUI();
        }
	}

	void AddCanvas ()
	{
		if (myCanvas) return;

		myCanvas = gameObject.GetComponent<CanvasGroup> ();
		if (!myCanvas)
		myCanvas = gameObject.AddComponent<CanvasGroup> ();
		myCanvas.alpha = (!showOnStart) ? 0 : 1;
	}

    public void Close()
    {
        Open(false);
    }

	public bool activeSelf;

	public void Open (bool show = true)
	{
		if (activeSelf && !gameObject.activeSelf)
		{
			activeSelf = false;
			return;
		}

        if (!myCanvas)
        {
            AddCanvas();
            myCanvas.alpha = (showOnStart) ? 1 : 0;
        }

        activeSelf = show;

        if (show) 
		{
            if (hideIn != 0)
            {
                CancelInvoke();
                Invoke("Close", hideIn);
            }

            gameObject.SetActive (true);
			alphaDown = 1;
            
            // Apply enhanced UI whenever shown
            if (useEnhancedUI && !hasBeenEnhanced)
            {
                ApplyEnhancedUI();
            }
		} 
		else 
		{
            if (!gameObject.activeSelf)
                return;

			alphaDown = -1;
		}
	}

	int alphaDown = 0;
	void Update ()
	{
		if (alphaDown != 0) 
		{
			myCanvas.alpha += alphaDown * Time.deltaTime * aSpeed;

			if ((alphaDown == 1 && myCanvas.alpha >= defaultA) ||
			(alphaDown == -1 && myCanvas.alpha <= minAlpha))
			{
				if (alphaDown == -1) 
				{
                    if (falseOnHide)
					gameObject.SetActive (false);
                    if (destroyOnHide)
                    {
                        Destroy(gameObject);
                        return;
                    }
                        
					myCanvas.alpha = minAlpha - 0.01f;
				}
				else 
				{
					myCanvas.alpha = 1;
				}

				alphaDown = 0;
			}
		}
	}
    
    // === ENHANCED UI IMPLEMENTATION === //
    private void ApplyEnhancedUI()
    {
        // Set enhancement flag to prevent multiple applications
        hasBeenEnhanced = true;
        
        // Find all UI elements in this panel
        cachedButtons = GetComponentsInChildren<Button>(true);
        cachedTexts = GetComponentsInChildren<Text>(true);
        cachedImages = GetComponentsInChildren<Image>(true);
        
        // Enhance buttons
        foreach (Button button in cachedButtons)
        {
            EnhanceButton(button);
        }
        
        // Enhance texts
        foreach (Text text in cachedTexts)
        {
            EnhanceText(text);
        }
        
        // Enhance background panels
        foreach (Image image in cachedImages)
        {
            // Skip button images since we already handled them
            if (image.GetComponent<Button>() != null)
                continue;
            
            // Basic panel enhancements
            if (image.transform.childCount > 0)
            {
                // This is likely a panel
                EnhancePanel(image);
            }
        }
        
        Debug.Log($"Enhanced UI panel: {gameObject.name}");
    }
    
    private void EnhanceButton(Button button)
    {
        if (button == null) return;
        
        // Add name suffix to avoid re-processing
        if (button.name.EndsWith("_Enhanced")) return;
        button.name = button.name + "_Enhanced";
        
        // Get the image component
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            // Changed to a gradient with primary color - NEW STYLE
            buttonImage.color = new Color(primaryColor.r, primaryColor.g, primaryColor.b, 0.9f);
            
            // Add rounded corners if using a standard sprite - NEW
            if (buttonImage.sprite == null || buttonImage.sprite.name == "Background" || buttonImage.sprite.name == "UISprite")
            {
                buttonImage.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
                buttonImage.type = Image.Type.Sliced;
                buttonImage.pixelsPerUnitMultiplier = 1;
            }
            
            // Add a stronger outline to the button - CHANGED
            Outline outline = button.GetComponent<Outline>();
            if (outline == null)
            {
                outline = button.gameObject.AddComponent<Outline>();
                outline.effectColor = new Color(0.0f, 0.0f, 0.0f, 0.8f);
                outline.effectDistance = new Vector2(2, -2);
            }
            else
            {
                outline.effectColor = new Color(0.0f, 0.0f, 0.0f, 0.8f);
                outline.effectDistance = new Vector2(2, -2);
            }
            
            // Add hover animation - NEW
            if (animateElements)
            {
                // Add hover animation component if it doesn't exist
                ButtonHoverEffect hoverEffect = button.GetComponent<ButtonHoverEffect>();
                if (hoverEffect == null)
                {
                    hoverEffect = button.gameObject.AddComponent<ButtonHoverEffect>();
                    hoverEffect.hoverScaleMultiplier = hoverScaleAmount;
                    hoverEffect.animationSpeed = animationSpeed;
                    hoverEffect.targetGraphic = buttonImage;
                }
            }
        }
        
        // Enhance text if it exists
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            // Make the text more readable with white text - CHANGED
            buttonText.color = Color.white;
            buttonText.fontStyle = FontStyle.Bold;  // Make all button text bold - NEW
            
            // Add an outline for better contrast - CHANGED
            Outline textOutline = buttonText.GetComponent<Outline>();
            if (textOutline == null)
            {
                textOutline = buttonText.gameObject.AddComponent<Outline>();
                textOutline.effectColor = new Color(0, 0, 0, 0.8f);
                textOutline.effectDistance = new Vector2(1, -1);
            }
            else
            {
                textOutline.effectColor = new Color(0, 0, 0, 0.8f);
                textOutline.effectDistance = new Vector2(1, -1);
            }
            
            // Make font size more readable - INCREASED
            buttonText.fontSize = Mathf.Max(buttonText.fontSize, 18);
        }
    }
    
    private void EnhanceText(Text text)
    {
        if (text == null) return;
        
        // Skip texts that are part of buttons (we already handled them)
        if (text.GetComponentInParent<Button>() != null)
            return;
            
        // Add name suffix to avoid re-processing
        if (text.name.EndsWith("_Enhanced")) return;
        text.name = text.name + "_Enhanced";
        
        // Determine if this is a title
        bool isTitle = text.fontSize >= 20 || text.fontStyle == FontStyle.Bold;
        
        if (isTitle)
        {
            // Make titles gold
            text.color = accentColor;
            
            // Add an outline for better contrast
            Outline outline = text.GetComponent<Outline>();
            if (outline == null)
            {
                outline = text.gameObject.AddComponent<Outline>();
                outline.effectColor = new Color(0, 0, 0, 0.8f);
                outline.effectDistance = new Vector2(1, -1);
            }
        }
        else
        {
            // Make regular text light gray for better contrast
            text.color = new Color(0.9f, 0.9f, 0.9f);
            
            // Ensure font size is readable
            if (text.fontSize < 14)
            {
                text.fontSize = 14;
            }
        }
    }
    
    private void EnhancePanel(Image panelImage)
    {
        // Add name suffix to avoid re-processing
        if (panelImage.name.EndsWith("_Enhanced")) return;
        panelImage.name = panelImage.name + "_Enhanced";
        
        // Apply a more distinctive styling to the panel - CHANGED
        Color originalColor = panelImage.color;
        
        // Apply a stronger blue tint to the panel background
        panelImage.color = new Color(
            originalColor.r * 0.7f,
            originalColor.g * 0.7f,
            Mathf.Min(originalColor.b * 1.4f, 0.9f),
            originalColor.a
        );
        
        // Add rounded corners to panels - NEW
        if (panelImage.sprite == null || panelImage.sprite.name == "Background" || panelImage.sprite.name == "UISprite")
        {
            panelImage.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
            panelImage.type = Image.Type.Sliced;
            panelImage.pixelsPerUnitMultiplier = 1;
        }
        
        // Add subtle border to panels - NEW
        Outline panelOutline = panelImage.GetComponent<Outline>();
        if (panelOutline == null)
        {
            panelOutline = panelImage.gameObject.AddComponent<Outline>();
            panelOutline.effectColor = new Color(0.2f, 0.6f, 1f, 0.3f); // Subtle blue glow
            panelOutline.effectDistance = new Vector2(3, -3);
        }
        
        // Add shadow - NEW
        Shadow panelShadow = panelImage.GetComponent<Shadow>();
        if (panelShadow == null)
        {
            panelShadow = panelImage.gameObject.AddComponent<Shadow>();
            panelShadow.effectColor = new Color(0, 0, 0, 0.5f);
            panelShadow.effectDistance = new Vector2(4, -4);
        }
    }
}
