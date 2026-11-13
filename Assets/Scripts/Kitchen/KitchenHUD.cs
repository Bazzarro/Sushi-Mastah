using UnityEngine;
using UnityEngine.UI;

public class KitchenHUD : MonoBehaviour
{
    private PlayerInventory inventory;
    private PlayerInteractor interactor;
    private Text inventoryText;
    private Text statusText;
    private Text promptText;
    private Text rollsText;

    public static KitchenHUD Create(PlayerInventory inventory, PlayerInteractor interactor)
    {
        GameObject existing = GameObject.Find("Kitchen HUD");
        if (existing != null)
            return existing.GetComponent<KitchenHUD>();

        GameObject root = new GameObject("Kitchen HUD");
        KitchenHUD hud = root.AddComponent<KitchenHUD>();
        hud.inventory = inventory;
        hud.interactor = interactor;
        hud.Build();
        return hud;
    }

    private void Update()
    {
        if (inventory == null || interactor == null)
            return;

        inventoryText.text = $"INVENTORY  {inventory.Items.Count}/{inventory.MaxItems}\n" + inventory.InventoryListText();
        rollsText.text = $"ROLLS MADE: {inventory.CompletedRolls}";
        string craftable = inventory.GetCraftableRollName();
        statusText.text = string.IsNullOrEmpty(craftable)
            ? inventory.StatusMessage
            : $"ready to assemble: {craftable}";

        bool showPrompt = interactor.HasTarget;
        promptText.transform.parent.gameObject.SetActive(showPrompt);
        if (showPrompt)
            promptText.text = interactor.CurrentPrompt;
    }

    private void Build()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Segoe UI", "Liberation Sans" }, 24);

        GameObject inventoryPanel = CreatePanel("Inventory Panel", transform, new Color(0.04f, 0.05f, 0.06f, 0.90f));
        SetRect(inventoryPanel.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -20f), new Vector2(420f, 270f), new Vector2(0f, 1f));
        inventoryText = CreateText("Inventory Text", inventoryPanel.transform, font, 28, TextAnchor.UpperLeft, Color.white);
        SetStretch(inventoryText.rectTransform, 22f, 22f, 22f, 62f);
        Text discardHint = CreateText("Discard Hint", inventoryPanel.transform, font, 20, TextAnchor.LowerRight, new Color(0.75f, 0.8f, 0.85f));
        discardHint.text = "Q - discard last item";
        SetRect(discardHint.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(-22f, 14f), new Vector2(-44f, 42f), new Vector2(1f, 0f));

        rollsText = CreateText("Rolls Text", inventoryPanel.transform, font, 24, TextAnchor.LowerLeft, new Color(1f, 0.8f, 0.3f));
        SetRect(rollsText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(22f, 14f), new Vector2(-44f, 42f), new Vector2(0f, 0f));

        GameObject recipePanel = CreatePanel("Recipe Panel", transform, new Color(0.04f, 0.05f, 0.06f, 0.90f));
        SetRect(recipePanel.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-20f, -20f), new Vector2(610f, 245f), new Vector2(1f, 1f));
        Text recipe = CreateText("Recipe Text", recipePanel.transform, font, 24, TextAnchor.UpperLeft, Color.white);
        recipe.text = "RECIPES\n\nsalmon roll: chopped salmon + seaweed + cooked rice\n\ntuna roll: chopped tuna + seaweed + cooked rice\n\nshrimp roll: chopped shrimp + avocado + cucumber + cooked rice";
        SetStretch(recipe.rectTransform, 22f, 22f, 22f, 22f);

        GameObject statusPanel = CreatePanel("Status Panel", transform, new Color(0.04f, 0.05f, 0.06f, 0.85f));
        SetRect(statusPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 82f), new Vector2(760f, 58f), new Vector2(0.5f, 0f));
        statusText = CreateText("Status Text", statusPanel.transform, font, 23, TextAnchor.MiddleCenter, new Color(0.85f, 0.92f, 1f));
        SetStretch(statusText.rectTransform, 16f, 16f, 8f, 8f);

        GameObject promptPanel = CreatePanel("Prompt Panel", transform, new Color(0.08f, 0.22f, 0.18f, 0.94f));
        SetRect(promptPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 18f), new Vector2(620f, 58f), new Vector2(0.5f, 0f));
        promptText = CreateText("Prompt Text", promptPanel.transform, font, 26, TextAnchor.MiddleCenter, Color.white);
        SetStretch(promptText.rectTransform, 14f, 14f, 6f, 6f);
    }

    private static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        panel.GetComponent<Image>().color = color;
        return panel;
    }

    private static Text CreateText(string name, Transform parent, Font font, int size, TextAnchor anchor, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Text));
        obj.transform.SetParent(parent, false);
        Text text = obj.GetComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.alignment = anchor;
        text.color = color;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private static void SetStretch(RectTransform rect, float left, float right, float bottom, float top)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 size, Vector2 pivot)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
    }
}
