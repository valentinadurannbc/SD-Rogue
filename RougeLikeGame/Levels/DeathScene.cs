using RogueLib;
using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;

namespace RlGameNS;

public class DeathScene : Scene
{
    public DeathScene(Player player, Game game)
    {
        _player = player;
        _game = game;
        RegisterCommand(ConsoleKey.Q, "quit");
        RegisterCommand(ConsoleKey.Enter, "quit");
        RegisterCommand(ConsoleKey.Escape, "quit");
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(DungeonConfig.RIP, ConsoleColor.White);
        disp.Draw("Press Q or Enter to exit", new Vector2(0, 24), ConsoleColor.Yellow);
    }

    public override void Update() { }

    public override void DoCommand(Command command)
    {
        if (command.Name == "quit") QuitLevel();
    }
}
