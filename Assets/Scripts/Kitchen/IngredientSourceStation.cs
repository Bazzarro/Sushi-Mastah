using UnityEngine;

public class IngredientSourceStation : MonoBehaviour, IInteractable
{
    public IngredientType ingredientType;
    public IngredientState startingState = IngredientState.Ready;

    public string GetPrompt(PlayerInventory inventory)
    {
        return $"E - grab {ingredientType.ToString().ToLowerInvariant()}";
    }

    public void Interact(PlayerInventory inventory)
    {
        inventory.Add(new KitchenIngredient(ingredientType, startingState));
    }
}
