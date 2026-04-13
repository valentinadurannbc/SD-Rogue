using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;

namespace RlGameNS;

public class Level2 : Level
{
    public Level2(Player p, string map, Game game) : base(p, map, game) { }

    protected override void spawnEnemies()
    {
        _enemies.Add(spawnEnemyAt(new Snake(Vector2.Zero, hp: 10)));
        _enemies.Add(spawnEnemyAt(new Snake(Vector2.Zero, hp: 10)));
        _enemies.Add(spawnEnemyAt(new Orc(Vector2.Zero, hp: 22)));
        _enemies.Add(spawnEnemyAt(new Orc(Vector2.Zero, hp: 22)));
        _enemies.Add(spawnEnemyAt(new Skeleton(Vector2.Zero, hp: 16)));
        _enemies.Add(spawnEnemyAt(new Skeleton(Vector2.Zero, hp: 16)));
        _enemies.Add(spawnEnemyAt(new Skeleton(Vector2.Zero, hp: 16)));
    }

    protected override void spawnItems()
    {
        spawnItemAt(new Apple(Vector2.Zero));
        spawnItemAt(new Apple(Vector2.Zero));
        spawnItemAt(new IronSword(Vector2.Zero));
        spawnItemAt(new BaseballBat(Vector2.Zero));
        spawnItemAt(new Key(Vector2.Zero));
    }
}
