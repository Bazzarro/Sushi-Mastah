using System;

public enum IngredientType
{
    Salmon,
    Tuna,
    Shrimp,
    Seaweed,
    Rice,
    Avocado,
    Cucumber
}

public enum IngredientState
{
    Ready,
    Raw,
    Chopped,
    Cooked
}

[Serializable]
public struct KitchenIngredient
{
    public IngredientType type;
    public IngredientState state;

    public KitchenIngredient(IngredientType type, IngredientState state)
    {
        this.type = type;
        this.state = state;
    }

    public string DisplayName()
    {
        if (state == IngredientState.Ready)
            return type.ToString();

        return $"{state} {type}";
    }
}
