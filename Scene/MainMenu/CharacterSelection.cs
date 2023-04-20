using Godot;
using System.Collections.Generic;

public partial class CharacterSelection : Control
{
    [Export] Pawn player;

    PlayerComponent playerComponent => player.Get<PlayerComponent>();

    public static Dictionary<int, int> ChosenOptions = null;
    public static Dictionary<int, OptionSelector> OptionSelectors = new();

    public static void InitializeChosenOptions(List<Customizable> list)
    {
        ChosenOptions = new();
        foreach (var custom in list)
        {
            ChosenOptions.Add(custom.Id, custom.CurrentOption);
        }
    }

    public void RegisterOptionSelector(OptionSelector selector)
    {
        selector.Left += () =>
        {
            ChosenOptions[selector.Id]--;
            playerComponent.SetCustomizationParameters();
        };
        selector.Right += () =>
        {
            ChosenOptions[selector.Id]++;
            playerComponent.SetCustomizationParameters();
        };

    }

}
