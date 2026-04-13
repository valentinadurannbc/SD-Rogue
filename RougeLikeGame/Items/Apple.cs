using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RlGameNS;

public class Apple : IItem
{
    public Vector2 Pos { get; }
    public string Name => "Apple";
    public bool IsWeapon => false;
    public int StrBonus => 0;

    public Apple(Vector2 pos)
    {
        Pos = pos;
    }

    public void Draw(IRenderWindow disp) =>
       disp.Draw('$', Pos, ConsoleColor.Red);

    public void Use(Player player) =>
       player.Heal(5);
}
