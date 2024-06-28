using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField]
    private UpgradeInfo scriptableObject;

    [Header("__________ In-Prefab References __________")]
    
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI subtitle;
    [SerializeField]
    private TextMeshProUGUI cooldown;
    [SerializeField]
    private TextMeshProUGUI description;
    [SerializeField]
    private Image farbverlauf;
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private Image glowImage;
    [SerializeField]
    private Image foregroundImage;
    [SerializeField]
    private Image hintergrundstruktur;
    [SerializeField]
    private Image border;

    [Header("__________ Coloring __________")]
    [SerializeField]
    private ColorSettings commonColorScheme;
    [SerializeField]
    private ColorSettings rareColorScheme;
    [SerializeField]
    private ColorSettings epicColorScheme;

    [Header("__________ Image Sources __________")]
    public Sprite epicBGStructure;
    public Sprite rareBGStructure;

    [Header("__________ Other Stuff __________")]
    [SerializeField]
    private float descriptionLineHeight;
    [SerializeField]
    private float cooldownVertOffset;

    // Call this to change the card's appearance!
    public void SetUpgradeInfo(UpgradeInfo upgradeInfo)
    {
        scriptableObject = upgradeInfo;
        FullSetupIfAssigned();
    }

    private void FullSetupIfAssigned()
    {
        if (scriptableObject == null)
        {
            Debug.LogWarning("No ScriptableObject assigned!");
            return;
        }
        SetCardColors();
        SetCardValues();
        SetCardPictures();
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        FullSetupIfAssigned();
    }
#endif

    private void SetCardValues()
    {
        title.text = scriptableObject.title;
        subtitle.text = scriptableObject.subtitle;
        description.text = scriptableObject.description;
        if (scriptableObject.cooldown == 0)
        {
            cooldown.text = "Passive";
            cooldown.color = new Color(0.7f, 0.7f, 0.7f, 1);
        } else
        {
            cooldown.text = "Cooldown: " + scriptableObject.cooldown + "s";
            cooldown.color = new Color(1, 1, 1, 1);
        }
        cooldown.transform.localPosition = 
            new Vector3(cooldown.transform.localPosition.x,
                        cooldownVertOffset + scriptableObject.lineCount * descriptionLineHeight,
                        cooldown.transform.localPosition.z);
    }

    public void Update()
    {
        transform.rotation = Quaternion.Euler(2 * Mathf.Cos(Time.time), 2*Mathf.Sin(Time.time), Mathf.Cos(Time.time + 3.14f));
    }

    private void SetCardColors()
    {
        switch (scriptableObject.rarety)
        {
            case Rarity.Common:
                SetColorScheme(commonColorScheme);
                break;
            case Rarity.Rare:
                SetColorScheme(rareColorScheme);
                break;
            case Rarity.Epic:
                SetColorScheme(epicColorScheme);
                break;
        }
    }

    private void SetColorScheme(ColorSettings colorScheme)
    {
        farbverlauf.color = colorScheme.backgroundColor;
        glowImage.color = colorScheme.glowColor;
        title.color = colorScheme.titleColor;
        border.color = colorScheme.borderColor;
    }

    private void SetCardPictures()
    {
        foregroundImage.sprite = scriptableObject.foreground;
        backgroundImage.sprite = scriptableObject.background;
        glowImage.sprite = scriptableObject.glow;
        switch (scriptableObject.rarety)
        {
            case Rarity.Common:
                hintergrundstruktur.sprite = null;
                break;
            case Rarity.Rare:
                hintergrundstruktur.sprite = rareBGStructure;
                hintergrundstruktur.color = new Color(1, 1, 1, 0.2f);
                break;
            case Rarity.Epic:
                hintergrundstruktur.sprite = epicBGStructure;
                hintergrundstruktur.color = new Color(1, 1, 1, 0.02f);
                break;
        }
    }
}

[Serializable]
public class ColorSettings
{
    public Color titleColor = Color.white;
    public Color borderColor = Color.white;
    public Color backgroundColor = Color.white;
    public Color glowColor = Color.white;
}
