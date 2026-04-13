using RogueLib.Utilities;

namespace RogueLib.Dungeon;

public interface IItem : IDrawable
{
    Vector2 Pos { get; }
    string Name { get; }
    bool IsWeapon { get; }
    int StrBonus { get; }
    void Use(Player player);
}
