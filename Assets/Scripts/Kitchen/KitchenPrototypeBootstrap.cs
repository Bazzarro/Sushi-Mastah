using UnityEngine;
using UnityEngine.SceneManagement;

public static class KitchenPrototypeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void BuildPrototype()
    {
        PlayerMovement player = Object.FindAnyObjectByType<PlayerMovement>();
        if (player == null)
        {
            Debug.LogError("prep stations could not start because no PlayerMovement object was found in the scene");
            return;
        }

        if (GameObject.Find("Kitchen Prep Prototype") != null)
            return;

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null)
            inventory = player.gameObject.AddComponent<PlayerInventory>();

        PlayerInteractor interactor = player.GetComponent<PlayerInteractor>();
        if (interactor == null)
            interactor = player.gameObject.AddComponent<PlayerInteractor>();

        KitchenHUD hud = KitchenHUD.Create(inventory, interactor);
        if (hud == null)
            Debug.LogError("prep stations started, but the kitchen HUD could not be created");

        SetupCamera(player.transform);

        GameObject root = new GameObject("Kitchen Prep Prototype");

        // source bins sit around the outside so the center stays open for movement
        // CreateIngredientSource(root.transform, "Salmon Bin", IngredientType.Salmon, IngredientState.Raw, new Vector3(-3.6f, -0.15f, 3.1f), new Color(0.94f, 0.45f, 0.48f));
        // CreateIngredientSource(root.transform, "Tuna Bin", IngredientType.Tuna, IngredientState.Raw, new Vector3(-2.35f, -0.15f, 3.1f), new Color(0.75f, 0.28f, 0.34f));
        // CreateIngredientSource(root.transform, "Shrimp Bin", IngredientType.Shrimp, IngredientState.Raw, new Vector3(-1.1f, -0.15f, 3.1f), new Color(1f, 0.58f, 0.45f));
        // CreateIngredientSource(root.transform, "Seaweed Bin", IngredientType.Seaweed, IngredientState.Ready, new Vector3(0.4f, -0.15f, 3.1f), new Color(0.16f, 0.45f, 0.28f));
        // CreateIngredientSource(root.transform, "Rice Bin", IngredientType.Rice, IngredientState.Raw, new Vector3(1.65f, -0.15f, 3.1f), new Color(0.93f, 0.9f, 0.78f));
        // CreateIngredientSource(root.transform, "Avocado Bin", IngredientType.Avocado, IngredientState.Ready, new Vector3(3.55f, -0.15f, 1.55f), new Color(0.48f, 0.72f, 0.28f));
        // CreateIngredientSource(root.transform, "Cucumber Bin", IngredientType.Cucumber, IngredientState.Ready, new Vector3(3.55f, -0.15f, 0.15f), new Color(0.28f, 0.72f, 0.4f));

        CreateStation<CuttingStation>(root.transform, "Cutting Board", new Vector3(-3.2f, -0.15f, -2.8f), new Color(0.67f, 0.44f, 0.25f));
        CreateStation<RiceCookerStation>(root.transform, "Rice Cooker", new Vector3(0f, -0.15f, -2.8f), new Color(0.82f, 0.84f, 0.88f));
        CreateStation<AssemblyStation>(root.transform, "Assembly Counter", new Vector3(3.2f, -0.15f, -2.8f), new Color(0.25f, 0.55f, 0.78f));

        AddPrepLight(root.transform, "Cutting Light", new Vector3(-3.2f, 3.2f, -2.8f));
        AddPrepLight(root.transform, "Cooker Light", new Vector3(0f, 3.2f, -2.8f));
        AddPrepLight(root.transform, "Assembly Light", new Vector3(3.2f, 3.2f, -2.8f));

        Debug.Log($"prep stations ready in {SceneManager.GetActiveScene().name}: inventory, E interaction, chopping, rice cooking, assembly, camera follow, and point lights loaded");
    }

    private static void SetupCamera(Transform player)
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("prep station camera follow could not start because there is no camera tagged MainCamera");
            return;
        }

        TopDownCameraFollow follow = camera.GetComponent<TopDownCameraFollow>();
        if (follow == null)
            follow = camera.gameObject.AddComponent<TopDownCameraFollow>();

        follow.target = player;
        follow.SnapToTarget();
    }

    private static void CreateIngredientSource(Transform parent, string name, IngredientType type, IngredientState state, Vector3 position, Color color)
    {
        GameObject station = CreateCounter(parent, name, position, color);
        IngredientSourceStation source = station.AddComponent<IngredientSourceStation>();
        source.ingredientType = type;
        source.startingState = state;
    }

    private static void CreateStation<T>(Transform parent, string name, Vector3 position, Color color) where T : MonoBehaviour
    {
        GameObject station = CreateCounter(parent, name, position, color);
        station.AddComponent<T>();
    }

    private static GameObject CreateCounter(Transform parent, string name, Vector3 position, Color color)
    {
        GameObject station = GameObject.CreatePrimitive(PrimitiveType.Cube);
        station.name = name;
        station.transform.SetParent(parent);
        station.transform.position = position;
        station.transform.localScale = new Vector3(1.15f, 1.05f, 0.85f);

        Renderer renderer = station.GetComponent<Renderer>();
        renderer.material = CreateMaterial(color);

        CreateLabel(station.transform, name);
        return station;
    }

    private static void CreateLabel(Transform parent, string label)
    {
        GameObject textObject = new GameObject("Label");
        textObject.transform.SetParent(parent);
        textObject.transform.localPosition = new Vector3(0f, 1.05f, 0f);
        textObject.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
        textObject.transform.localScale = Vector3.one * 0.16f;

        TextMesh text = textObject.AddComponent<TextMesh>();
        text.text = label;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.fontSize = 40;
        text.characterSize = 0.15f;
        text.color = Color.white;
    }

    private static Material CreateMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material material = new Material(shader);
        material.color = color;
        return material;
    }

    private static void AddPrepLight(Transform parent, string name, Vector3 position)
    {
        GameObject lightObject = new GameObject(name);
        lightObject.transform.SetParent(parent);
        lightObject.transform.position = position;

        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 5f;
        light.intensity = 2.2f;
        light.color = new Color(1f, 0.84f, 0.65f);
    }
}
