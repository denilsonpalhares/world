using Godot;
using World.Simulation;

namespace World.Game;

public partial class Main : Node
{
    public override void _Ready()
    {
        GD.Print($"{SimulationInfo.Name} v{SimulationInfo.Version} (Godot {Engine.GetVersionInfo()["string"]})");
    }
}
