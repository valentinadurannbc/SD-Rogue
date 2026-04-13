using System.Collections.Generic;
using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RlGameNS;

public class Skeleton : IActor
{
    public char Glyph { get; } = 'X';
    public Vector2 Pos { get; set; }
    public int Hp { get; private set; } = 8;
    public int Str { get; } = 3;
    public int ExpValue { get; } = 2;

    private const int DetectionRadius = 2;
    private static readonly Vector2[] _directions = { Vector2.N, Vector2.S, Vector2.E, Vector2.W };

    public Skeleton(Vector2 pos, int hp = 8)
    {
        Pos = pos;
        Hp = hp;
    }

    public void TakeDamage(int amount)
    {
        Hp = Math.Max(Hp - amount, 0);
    }

    public void Draw(IRenderWindow disp) =>
       disp.Draw(Glyph, Pos, ConsoleColor.White);

    public void Update(HashSet<Vector2> walkables, Random rng, Player player)
    {
        var playerPos = player.Pos;
        var dir = (Pos - playerPos).RookLength <= DetectionRadius
           ? moveToward(playerPos, walkables)
           : randomDir(walkables, rng, playerPos);

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

    private Vector2 randomDir(HashSet<Vector2> walkables, Random rng, Vector2 playerPos)
    {
        var available = Array.FindAll(_directions,
           d => walkables.Contains(Pos + d) || (Pos + d) == playerPos);
        return available.Length == 0 ? Vector2.Zero : available[rng.Next(available.Length)];
    }
}
