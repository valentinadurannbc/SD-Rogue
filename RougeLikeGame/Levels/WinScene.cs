using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;

namespace RlGameNS;

public class WinScene : Scene
{
    private static readonly string _winArt =
       """
      ┌─────────────────────────────────────────────────────────┐
      │            *   *   *   YOU WIN!   *   *   *             │
      │                                                         │
      │          You found the exit and escaped the dungeon!    │
      │                                                         │
      │               Press Q or Enter to exit                  │
      │                                                         │
      └─────────────────────────────────────────────────────────┘

                                 \@/         
                                  |         
                                 / \                              

      """;

    public WinScene(Player player, Game game)
    {
        _player = player;
        _game = game;
        RegisterCommand(ConsoleKey.Q, "quit");
        RegisterCommand(ConsoleKey.Enter, "quit");
        RegisterCommand(ConsoleKey.Escape, "quit");
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(_winArt, ConsoleColor.Yellow);
    }

    public override void Update() { }

    public override void DoCommand(Command command)
    {
        if (command.Name == "quit") QuitLevel();
    }
}
