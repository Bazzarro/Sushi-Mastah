using System.Collections;
using UnityEngine;

public class CuttingStation : MonoBehaviour, IInteractable
{
    [SerializeField] private float chopTime = 0.8f;

    private bool chopping;
    private float remaining;

    public string GetPrompt(PlayerInventory inventory)
    {
        if (chopping)
            return $"chopping... {Mathf.Max(0f, remaining):0.0}s";

        if (inventory.TryGetRawProtein(out IngredientType protein))
            return $"E - chop {protein.ToString().ToLowerInvariant()}";

        return "need raw salmon, tuna, or shrimp";
    }

    public void Interact(PlayerInventory inventory)
    {
        if (chopping)
        {
            inventory.SetStatus("the cutting board is busy");
            return;
        }

        if (!inventory.TryGetRawProtein(out IngredientType protein))
        {
            inventory.SetStatus("grab raw salmon, tuna, or shrimp first");
            return;
        }

        StartCoroutine(ChopRoutine(inventory, protein));
    }

    private IEnumerator ChopRoutine(PlayerInventory inventory, IngredientType protein)
    {
        chopping = true;
        remaining = chopTime;
        inventory.SetStatus($"chopping {protein.ToString().ToLowerInvariant()}...");

        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;
            yield return null;
        }

        inventory.ChopProtein(protein);
        chopping = false;
        remaining = 0f;
    }
}
