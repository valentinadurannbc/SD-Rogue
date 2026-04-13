using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RlGameNS;

public class IronSword : IItem
{
    public Vector2 Pos { get; }
    public string Name => "Iron Sword";
    public bool IsWeapon => true;
    public int StrBonus => 6;

    public IronSword(Vector2 pos)
    {
        Pos = pos;
    }

    public void Draw(IRenderWindow disp) =>
       disp.Draw('T', Pos, ConsoleColor.Cyan);

    public void Use(Player player) { }
}
