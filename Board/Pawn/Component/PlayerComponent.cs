using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerComponent : Component
{
    public static PlayerComponent Main;
    public override void _EnterTree()
    {
        base._EnterTree();
        Main = this;

    }
    override protected IEnumerable<Type> GetDependencies()
    {
        yield return typeof(HealthComponent);
        yield return typeof(LocomotionComponent);
    }

    public override void _Ready()
    {
        base._Ready();
        SetCustomizationParameters();
    }

    List<Customizable> customizables = new();
    public void RegisterCustomizable(Customizable customizable)
    {
        customizables.Add(customizable);
    }

    public void SetCustomizationParameters()
    {
        if (CharacterSelection.ChosenOptions == null)
        {
            CharacterSelection.InitializeChosenOptions(customizables);
        }
        else
        {
            foreach (var custom in customizables)
            {
                custom.SetOption(CharacterSelection.ChosenOptions[custom.Id]);
            }
        }

    }

}