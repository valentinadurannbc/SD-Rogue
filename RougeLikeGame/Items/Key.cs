using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RlGameNS;

public class Key : IItem
{
    public Vector2 Pos { get; }
    public string Name => "Key";
    public bool IsWeapon => false;
    public int StrBonus => 0;

    public Key(Vector2 pos)
    {
        Pos = pos;
    }

    public void Draw(IRenderWindow disp) =>
       disp.Draw('k', Pos, ConsoleColor.Yellow);

    public void Use(Player player) { }
}
