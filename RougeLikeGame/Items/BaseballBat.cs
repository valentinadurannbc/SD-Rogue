using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RlGameNS;

public class BaseballBat : IItem
{
    public Vector2 Pos { get; }
    public string Name => "Baseball Bat";
    public bool IsWeapon => true;
    public int StrBonus => 3;

    public BaseballBat(Vector2 pos)
    {
        Pos = pos;
    }

    public void Draw(IRenderWindow disp) =>
       disp.Draw('/', Pos, ConsoleColor.DarkYellow);

    public void Use(Player player) { }
}
