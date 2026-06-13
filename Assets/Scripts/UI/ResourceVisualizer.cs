using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ResourceVisualizer : MonoBehaviour
{
    [Header("References")]
    public Image ResourceIcon;
    public TMP_Text ResourceQte;

    [Header("General")]
    [SerializeField, Range(0.0f, 1.0f)] private float RatioIconQte  = 0.25f; 
    [SerializeField, Range(0.0f, 1.0f)] private float Overlap       = 0.0f;
    [SerializeField] private ResourceType resourceType              = ResourceType.Wood;

    [Header("Icon")]
    [SerializeField] private Sprite Icon                            = null;
    [SerializeField] private float IconScale                        = 1.0f;

    [Header("Text")]
    [SerializeField] private int FontSize                           = 20;
    [SerializeField] private Color FontColor                        = Color.black; 

    private int CurrentQte = 0;

    // TODO : Remove on release
    [Header("Debug")]
    [SerializeField] private bool UPDATE = true;

    // TODO : Remove on release
    void Update()
    {
        if(UPDATE && Inventory.Instance)
        {
            UpdateVisualizer(Inventory.Instance.GetResourceAmount(resourceType), Icon);
        }
    }

    void Start()
    {
        if (!Inventory.Instance) return;
        UpdateQte(Inventory.Instance.GetResourceAmount(resourceType));
        UpdateIcon(Icon);
    }

    public void UpdateVisualizer(int qte, Sprite sprite)
    {
        UpdateQte(qte);
        UpdateIcon(Icon);

        ResourceIcon.rectTransform.anchorMin = new Vector2(0.0f, RatioIconQte - Overlap * 0.5f);
        ResourceQte.rectTransform.anchorMax = new Vector2(1.0f, RatioIconQte + Overlap * 0.5f);
    }

    public void UpdateQte(int qte)
    {
        CurrentQte = qte;
        ResourceQte.text = qte.ToString();
        ResourceQte.faceColor = FontColor;
        ResourceQte.fontSize = FontSize;
    }

    public void UpdateIcon(Sprite sprite) {
        Icon = sprite;
        ResourceIcon.sprite = sprite;
        ResourceIcon.rectTransform.localScale = new Vector3(IconScale, IconScale, IconScale);
    }

    public int GetDisplayedQte() { return CurrentQte; }

}
