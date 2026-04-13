using System.Collections.Generic;
using RogueLib.Utilities;

namespace RogueLib.Dungeon;

public interface IActor : IDrawable
{
    char Glyph { get; }
    Vector2 Pos { get; set; }
    int Hp { get; }
    int Str { get; }
    int ExpValue { get; }
    void TakeDamage(int amount);
    void Update(HashSet<Vector2> walkables, Random rng, Player player);
}