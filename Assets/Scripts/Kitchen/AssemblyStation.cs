using System.Collections;
using UnityEngine;

public class AssemblyStation : MonoBehaviour, IInteractable
{
    private GameObject completedRollVisual;

    public string GetPrompt(PlayerInventory inventory)
    {
        string roll = inventory.GetCraftableRollName();
        if (!string.IsNullOrEmpty(roll))
            return $"E - assemble {roll}";

        return "need a complete recipe to assemble";
    }

    public void Interact(PlayerInventory inventory)
    {
        if (!inventory.TryMakeRoll(out string rollName))
            return;

        ShowCompletedRoll(rollName);
    }

    private void ShowCompletedRoll(string rollName)
    {
        if (completedRollVisual != null)
            Destroy(completedRollVisual);

        completedRollVisual = new GameObject("Completed " + rollName);
        completedRollVisual.transform.SetParent(transform, false);
        completedRollVisual.transform.localPosition = new Vector3(0f, 0.82f, 0f);

        GameObject rice = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rice.name = "Rice";
        rice.transform.SetParent(completedRollVisual.transform, false);
        rice.transform.localScale = new Vector3(0.44f, 0.12f, 0.44f);
        SetColor(rice, new Color(0.95f, 0.93f, 0.82f));
        RemoveCollider(rice);

        GameObject filling = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        filling.name = "Filling";
        filling.transform.SetParent(completedRollVisual.transform, false);
        filling.transform.localPosition = new Vector3(0f, 0.13f, 0f);
        filling.transform.localScale = new Vector3(0.27f, 0.08f, 0.27f);
        SetColor(filling, RollColor(rollName));
        RemoveCollider(filling);

        StartCoroutine(HideCompletedRoll());
    }

    private IEnumerator HideCompletedRoll()
    {
        yield return new WaitForSeconds(2.5f);

        if (completedRollVisual != null)
            Destroy(completedRollVisual);
    }

    private static Color RollColor(string rollName)
    {
        if (rollName.Contains("salmon"))
            return new Color(0.95f, 0.42f, 0.38f);
        if (rollName.Contains("tuna"))
            return new Color(0.72f, 0.18f, 0.25f);
        return new Color(1f, 0.58f, 0.35f);
    }

    private static void SetColor(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material material = new Material(shader);
        material.color = color;
        renderer.material = material;
    }

    private static void RemoveCollider(GameObject obj)
    {
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null)
            Destroy(collider);
    }
}
