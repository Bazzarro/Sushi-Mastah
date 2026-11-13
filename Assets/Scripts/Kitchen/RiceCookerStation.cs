using UnityEngine;

public class RiceCookerStation : MonoBehaviour, IInteractable
{
    [SerializeField] private float cookTime = 3f;

    private float timer;
    private bool cooking;
    private bool riceReady;

    private void Update()
    {
        if (!cooking)
            return;

        timer -= Time.deltaTime;
        if (timer > 0f)
            return;

        cooking = false;
        riceReady = true;
    }

    public string GetPrompt(PlayerInventory inventory)
    {
        if (riceReady)
            return "E - collect cooked rice";

        if (cooking)
            return $"rice cooking... {Mathf.CeilToInt(timer)}s";

        if (!inventory.Has(IngredientType.Rice, IngredientState.Raw))
            return "need raw rice";

        return "E - cook raw rice";
    }

    public void Interact(PlayerInventory inventory)
    {
        if (riceReady)
        {
            if (inventory.Add(new KitchenIngredient(IngredientType.Rice, IngredientState.Cooked)))
            {
                riceReady = false;
                inventory.SetStatus("collected cooked rice");
            }
            return;
        }

        if (cooking)
        {
            inventory.SetStatus("the rice is still cooking");
            return;
        }

        if (!inventory.Remove(IngredientType.Rice, IngredientState.Raw))
        {
            inventory.SetStatus("grab raw rice before using the cooker");
            return;
        }

        cooking = true;
        timer = cookTime;
        inventory.SetStatus("rice started cooking - come back when it is ready");
    }
}
