using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int maxItems = 8;

    private readonly List<KitchenIngredient> items = new List<KitchenIngredient>();

    public string StatusMessage { get; private set; } = "grab ingredients and make a roll";
    public int CompletedRolls { get; private set; }
    public int MaxItems => maxItems;
    public IReadOnlyList<KitchenIngredient> Items => items;

    public bool Add(KitchenIngredient ingredient)
    {
        if (items.Count >= maxItems)
        {
            SetStatus("your hands are full");
            return false;
        }

        items.Add(ingredient);
        SetStatus($"picked up {ingredient.DisplayName().ToLowerInvariant()}");
        return true;
    }

    public bool DiscardLast()
    {
        if (items.Count == 0)
        {
            SetStatus("nothing to discard");
            return false;
        }

        int lastIndex = items.Count - 1;
        KitchenIngredient discarded = items[lastIndex];
        items.RemoveAt(lastIndex);
        SetStatus($"discarded {discarded.DisplayName().ToLowerInvariant()}");
        return true;
    }

    public bool Has(IngredientType type, IngredientState state)
    {
        return FindIndex(type, state) >= 0;
    }

    public bool Remove(IngredientType type, IngredientState state)
    {
        int index = FindIndex(type, state);
        if (index < 0)
            return false;

        items.RemoveAt(index);
        return true;
    }

    public bool Replace(IngredientType type, IngredientState fromState, IngredientState toState)
    {
        int index = FindIndex(type, fromState);
        if (index < 0)
            return false;

        items[index] = new KitchenIngredient(type, toState);
        return true;
    }

    public bool TryGetRawProtein(out IngredientType protein)
    {
        IngredientType[] proteins = { IngredientType.Salmon, IngredientType.Tuna, IngredientType.Shrimp };
        foreach (IngredientType type in proteins)
        {
            if (Has(type, IngredientState.Raw))
            {
                protein = type;
                return true;
            }
        }

        protein = default;
        return false;
    }

    public bool ChopProtein(IngredientType type)
    {
        if (!Replace(type, IngredientState.Raw, IngredientState.Chopped))
            return false;

        SetStatus($"chopped {type.ToString().ToLowerInvariant()} (raw -> chopped)");
        return true;
    }

    public bool TryMakeRoll(out string rollName)
    {
        if (CanMakeSalmonRoll())
        {
            Consume(IngredientType.Salmon, IngredientState.Chopped);
            Consume(IngredientType.Seaweed, IngredientState.Ready);
            Consume(IngredientType.Rice, IngredientState.Cooked);
            return FinishRoll("salmon roll", out rollName);
        }

        if (CanMakeTunaRoll())
        {
            Consume(IngredientType.Tuna, IngredientState.Chopped);
            Consume(IngredientType.Seaweed, IngredientState.Ready);
            Consume(IngredientType.Rice, IngredientState.Cooked);
            return FinishRoll("tuna roll", out rollName);
        }

        if (CanMakeShrimpRoll())
        {
            Consume(IngredientType.Shrimp, IngredientState.Chopped);
            Consume(IngredientType.Avocado, IngredientState.Ready);
            Consume(IngredientType.Cucumber, IngredientState.Ready);
            Consume(IngredientType.Rice, IngredientState.Cooked);
            return FinishRoll("shrimp roll", out rollName);
        }

        rollName = string.Empty;
        SetStatus("you do not have a complete sushi recipe yet");
        return false;
    }

    public string GetCraftableRollName()
    {
        if (CanMakeSalmonRoll()) return "salmon roll";
        if (CanMakeTunaRoll()) return "tuna roll";
        if (CanMakeShrimpRoll()) return "shrimp roll";
        return string.Empty;
    }

    public string InventoryText()
    {
        if (items.Count == 0)
            return "empty";

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < items.Count; i++)
        {
            if (i > 0)
                builder.Append(" | ");

            builder.Append(items[i].DisplayName());
        }

        return builder.ToString();
    }

    public string InventoryListText()
    {
        if (items.Count == 0)
            return "- empty";

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < items.Count; i++)
        {
            builder.Append("- ");
            builder.Append(items[i].DisplayName());
            if (i < items.Count - 1)
                builder.Append('\n');
        }

        return builder.ToString();
    }

    public void SetStatus(string message)
    {
        StatusMessage = message;
    }

    private bool CanMakeSalmonRoll()
    {
        return Has(IngredientType.Salmon, IngredientState.Chopped)
            && Has(IngredientType.Seaweed, IngredientState.Ready)
            && Has(IngredientType.Rice, IngredientState.Cooked);
    }

    private bool CanMakeTunaRoll()
    {
        return Has(IngredientType.Tuna, IngredientState.Chopped)
            && Has(IngredientType.Seaweed, IngredientState.Ready)
            && Has(IngredientType.Rice, IngredientState.Cooked);
    }

    private bool CanMakeShrimpRoll()
    {
        return Has(IngredientType.Shrimp, IngredientState.Chopped)
            && Has(IngredientType.Avocado, IngredientState.Ready)
            && Has(IngredientType.Cucumber, IngredientState.Ready)
            && Has(IngredientType.Rice, IngredientState.Cooked);
    }

    private bool FinishRoll(string name, out string rollName)
    {
        CompletedRolls++;
        rollName = name;
        SetStatus($"made a {name}! completed rolls: {CompletedRolls}");
        return true;
    }

    private void Consume(IngredientType type, IngredientState state)
    {
        Remove(type, state);
    }

    private int FindIndex(IngredientType type, IngredientState state)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].type == type && items[i].state == state)
                return i;
        }

        return -1;
    }
}
