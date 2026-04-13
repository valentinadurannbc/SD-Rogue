using System.Collections.Generic;
using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RlGameNS;

public class Snake : IActor
{
    public char Glyph { get; } = 'S';
    public Vector2 Pos { get; set; }
    public int Hp { get; private set; }
    public int Str { get; } = 2;
    public int ExpValue { get; } = 1;

    private const int DetectionRadius = 1;
    private static readonly Vector2[] _directions = { Vector2.N, Vector2.S, Vector2.E, Vector2.W };

    public Snake(Vector2 pos, int hp = 6)
    {
        Pos = pos;
        Hp = hp;
    }

    public void TakeDamage(int amount)
    {
        Hp = Math.Max(Hp - amount, 0);
    }

    public void Draw(IRenderWindow disp) =>
       disp.Draw(Glyph, Pos, ConsoleColor.Green);

    public void Update(HashSet<Vector2> walkables, Random rng, Player player)
    {
        var playerPos = player.Pos;
        if ((Pos - playerPos).RookLength > DetectionRadius) return;

        var dir = moveToward(playerPos, walkables);
        if (dir == Vector2.Zero) return;

        var nextPos = Pos + dir;
        if (nextPos == playerPos)
        {
            player.TakeDamage(Str);
            return;
        }

        walkables.Add(Pos);
        Pos = nextPos;
        walkables.Remove(Pos);
    }

    private Vector2 moveToward(Vector2 target, HashSet<Vector2> walkables)
    {
        var best = Vector2.Zero;
        var bestDist = int.MaxValue;
        foreach (var d in _directions)
        {
            var next = Pos + d;
            if (!walkables.Contains(next) && next != target) continue;
            var dist = (next - target).RookLength;
            if (dist < bestDist) { bestDist = dist; best = d; }
        }
        return best;
    }
}
