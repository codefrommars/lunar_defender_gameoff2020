using Godot;
using System;

public class HUD : Node2D
{
    private Sprite helmet, barContainer;
    private Sprite[] bars;
    private Label energyLabel;

    private Sprite block, mines, beam;

    private Node2D machineUI;
    private Sprite[] machineBars;
    private Label machineEnergyLabel;

    public override void _Ready()
    {
        helmet = GetNode<Sprite>("PlayerEnergy/HUD_helmet");
        barContainer = GetNode<Sprite>("PlayerEnergy/HUD_bar_container");
        bars = new Sprite[5];

        for (int i = 0; i < 5; i++)
            bars[i] = GetNode<Sprite>("PlayerEnergy/HUD_bar" + (i + 1));

        energyLabel = GetNode<Label>("PlayerEnergy/EnergyLabel");

        block = GetNode<Sprite>("Powerups/Block");
        mines = GetNode<Sprite>("Powerups/Mines");
        beam = GetNode<Sprite>("Powerups/Beam");

        machineUI = GetNode<Node2D>("MachineEnergy");
        machineEnergyLabel = GetNode<Label>("MachineEnergy/EnergyLabel");
        machineBars = new Sprite[5];

        for (int i = 0; i < 5; i++)
            machineBars[i] = GetNode<Sprite>("MachineEnergy/HUD_bar" + (i + 1));
    }

    public override void _Process(float delta)
    {
        int playerEnergy = MoonHunter.Instance.Player.Energy;
        energyLabel.Text = playerEnergy + "";

        block.Visible = MoonHunter.Instance.GameState.IsUnlocked(MoonHunterState.Powerup.Blocks);
        mines.Visible = MoonHunter.Instance.GameState.IsUnlocked(MoonHunterState.Powerup.Mines);
        beam.Visible = MoonHunter.Instance.GameState.IsUnlocked(MoonHunterState.Powerup.Beam);

        int barPower = (MoonHunter.Constants.PLAYER_MAX_ENERGY / bars.Length);
        int barLevel = playerEnergy / barPower;
        int left = playerEnergy % barPower;

        float b = 1;
        float g = 1;

        if (playerEnergy <= barPower)
        {
            g = 0;
        }

        if (playerEnergy <= barPower * 0.5f)
        {
            b = 0;
        }

        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].Visible = i <= barLevel;

            float a = 1;
            if (i == barLevel)
            {
                a = Mathf.Lerp(0.5f, 1.0f, (float)left / barPower);
            }
            bars[i].Modulate = new Color(1, b, g, a);
        }

        Home machine = MoonHunter.Instance.GetCurrentMachine();
        machineUI.Visible = machine != null;
        if (machine != null)
            UpdateMachineHP(machine.Energy);
    }

    private void UpdateMachineHP(int machineEnergy)
    {
        this.machineEnergyLabel.Text = machineEnergy + "";

        int barPower = (MoonHunter.Constants.MACHINE_MAX_ENERGY / machineBars.Length);
        int barLevel = machineEnergy / barPower;
        int left = machineEnergy % barPower;

        float b = 1;
        float g = 1;

        if (machineEnergy <= barPower)
        {
            g = 0;
        }

        if (machineEnergy <= barPower * 0.5f)
        {
            b = 0;
        }

        for (int i = 0; i < machineBars.Length; i++)
        {
            machineBars[i].Visible = i <= barLevel;

            float a = 1;
            if (i == barLevel)
            {
                a = Mathf.Lerp(0.5f, 1.0f, (float)left / barPower);
            }
            machineBars[i].Modulate = new Color(1, b, g, a);
        }
    }
}
