using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Shows a "New Order" announcement on the left of the screen every orderInterval seconds
// and keeps a running "Current Orders" list underneath the inventory panel.
// Creates itself automatically in any scene that has a PlayerMovement, same as the prep stations.
public class Requests : MonoBehaviour
{
    [SerializeField] private float orderInterval = 20f;
    [SerializeField] private float firstOrderDelay = 5f;
    [SerializeField] private float announcementDuration = 4f;
    [SerializeField] private int maxOrders = 8; // 0 = no limit

    private static readonly string[] RollNames = { "Shrimp Roll", "Salmon Roll", "Tuna Roll" };

    private static readonly string[] CustomerNames =
    {
        "Liam", "Maya", "Noah", "Elena", "Caleb", "Aria", "Julian", "Chloe", "Mateo", "Naomi",
        "Owen", "Lila", "Lucas", "Nora", "Ezra", "Hannah", "Leo", "Sofia", "Gabriel", "Claire",
        "Miles", "Stella", "Isaac", "Audrey", "Nathan", "Zara", "Theo", "Iris", "Adrian", "Ruby",
        "Xavier", "Evelyn", "Asher", "Camille", "Simon", "Tessa", "Roman", "Hazel", "Felix", "Cora"
    };

    public struct Order
    {
        public string roll;
        public string customer;
        public float startTime;

        public Order(string roll, string customer, float startTime)
        {
            this.roll = roll;
            this.customer = customer;
            this.startTime = startTime;
        }

        // Whole seconds this order has been waiting.
        public int SecondsWaiting => Mathf.FloorToInt(Time.time - startTime);

        public override string ToString() => $"{roll} ({customer})";
    }

    private readonly List<Order> currentOrders = new List<Order>();
    private float nextOrderTimer;
    private float announcementTimer;
    private int lastTotalSeconds = -1;

    private const string TimerColor = "#FF4040";

    private GameObject announcementPanel;
    private Text announcementText;
    private Text ordersText;

    public IReadOnlyList<Order> CurrentOrders => currentOrders;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateForScene()
    {
        if (UnityEngine.Object.FindAnyObjectByType<PlayerMovement>() == null)
            return;

        if (UnityEngine.Object.FindAnyObjectByType<Requests>() != null)
            return;

        new GameObject("Order Requests").AddComponent<Requests>();
    }

    private void Awake()
    {
        Build();
        nextOrderTimer = firstOrderDelay;
        announcementPanel.SetActive(false);
        RefreshOrdersText();
    }

    private void Update()
    {
        nextOrderTimer -= Time.deltaTime;
        if (nextOrderTimer <= 0f)
        {
            nextOrderTimer += Mathf.Max(0.1f, orderInterval);
            AddRandomOrder();
        }

        if (announcementTimer > 0f)
        {
            announcementTimer -= Time.deltaTime;
            if (announcementTimer <= 0f)
                announcementPanel.SetActive(false);
        }

        // Keep the waiting timers live. Only rebuild the text when a displayed second changes.
        int totalSeconds = 0;
        foreach (Order order in currentOrders)
            totalSeconds += order.SecondsWaiting;

        if (totalSeconds != lastTotalSeconds)
            RefreshOrdersText();
    }

    private void AddRandomOrder()
    {
        if (maxOrders > 0 && currentOrders.Count >= maxOrders)
        {
            ShowAnnouncement("Orders are piling up!");
            return;
        }

        string roll = RollNames[UnityEngine.Random.Range(0, RollNames.Length)];
        string customer = CustomerNames[UnityEngine.Random.Range(0, CustomerNames.Length)];
        Order order = new Order(roll, customer, Time.time);

        currentOrders.Add(order);
        ShowAnnouncement($"New Order: {order}");
        RefreshOrdersText();
    }

    // Removes the oldest order for this roll. Roll names are matched ignoring case,
    // so "salmon roll" from PlayerInventory matches "Salmon Roll".
    public bool TryCompleteOrder(string rollName)
    {
        return TryCompleteOrder(rollName, out _);
    }

    // Same as above, but also tells you whose order it was.
    public bool TryCompleteOrder(string rollName, out string customer)
    {
        int index = currentOrders.FindIndex(o => string.Equals(o.roll, rollName, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
        {
            customer = string.Empty;
            return false;
        }

        customer = currentOrders[index].customer;
        currentOrders.RemoveAt(index);
        RefreshOrdersText();
        return true;
    }

    private void ShowAnnouncement(string message)
    {
        announcementText.text = message;
        announcementPanel.SetActive(true);
        announcementTimer = announcementDuration;
    }

    private void RefreshOrdersText()
    {
        StringBuilder builder = new StringBuilder("Current Orders:");
        int totalSeconds = 0;

        if (currentOrders.Count == 0)
        {
            builder.Append("\n- none yet");
        }
        else
        {
            for (int i = 0; i < currentOrders.Count; i++)
            {
                Order order = currentOrders[i];
                int seconds = order.SecondsWaiting;
                totalSeconds += seconds;
                builder.Append($"\n{i + 1}. {order} <color={TimerColor}>{seconds}s</color>");
            }
        }

        ordersText.text = builder.ToString();
        lastTotalSeconds = totalSeconds;
    }

    private void Build()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 101;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Segoe UI", "Liberation Sans" }, 24);

        // The inventory panel in KitchenHUD sits at the top-left (y -20 to -290), so these go below it.
        announcementPanel = CreatePanel("New Order Panel", transform, new Color(0.45f, 0.18f, 0.08f, 0.94f));
        SetRect(announcementPanel.GetComponent<RectTransform>(), new Vector2(20f, -310f), new Vector2(500f, 58f));
        announcementText = CreateText("New Order Text", announcementPanel.transform, font, 26, TextAnchor.MiddleLeft, Color.white);
        SetStretch(announcementText.rectTransform, 22f, 14f, 6f, 6f);

        int visibleLines = maxOrders > 0 ? maxOrders : 8;
        GameObject ordersPanel = CreatePanel("Current Orders Panel", transform, new Color(0.04f, 0.05f, 0.06f, 0.90f));
        SetRect(ordersPanel.GetComponent<RectTransform>(), new Vector2(20f, -385f), new Vector2(500f, 70f + 30f * visibleLines));
        ordersText = CreateText("Current Orders Text", ordersPanel.transform, font, 24, TextAnchor.UpperLeft, new Color(1f, 0.8f, 0.3f));
        ordersText.supportRichText = true; // needed for the red <color> timer text
        SetStretch(ordersText.rectTransform, 22f, 22f, 16f, 18f);
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

    // Anchored to the top-left corner of the screen.
    private static void SetRect(RectTransform rect, Vector2 anchoredPosition, Vector2 size)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
    }

    private static void SetStretch(RectTransform rect, float left, float right, float bottom, float top)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }
}