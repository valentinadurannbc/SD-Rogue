using RogueLib.Engine;
using RogueLib.Utilities;
using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

namespace RogueLib.Dungeon;

public abstract class BaseLevel : Scene
{
    protected string? _map;
    protected int _senseRadius = 6;

    protected TileSet _walkables;
    protected TileSet _floor;
    protected TileSet _tunnel;
    protected TileSet _door;
    protected TileSet _decor;
    protected TileSet _discovered;
    protected TileSet _inFov;
    protected TileSet _exit;
    protected TileSet _portal;

    public TileSet Portal => _portal;

    protected List<IActor> _enemies = new();
    protected List<IItem> _items = new();

    protected BaseLevel(Player p, string map, Game game)
    {
        if (game == null || p == null || map == null)
            throw new ArgumentNullException("game, player, or map cannot be null");

        _player = p;
        _map = map;
        _game = game;

        initMapTileSets(map);

        _discovered = new TileSet();
        _inFov = new TileSet();
    }

    public void InitPlayer(Vector2 startPos)
    {
        _player!.Pos = startPos;
        _walkables.Remove(startPos);
        updateDiscovered();
    }

    public void PlayerEnter(Vector2 arrivalPos)
    {
        _player!.Pos = arrivalPos;
        _walkables.Remove(arrivalPos);
        updateDiscovered();
    }

    public void PlayerLeave()
    {
        _walkables.Add(_player!.Pos);
    }

    protected void updateDiscovered()
    {
        _inFov = fovCalc(_player!.Pos, _senseRadius);
        _discovered.UnionWith(_inFov);
    }

    protected TileSet fovCalc(Vector2 pos, int sens)
       => Vector2.getAllTiles().Where(t => (pos - t).RookLength < sens).ToHashSet();

    public void MovePlayer(Vector2 delta)
    {
        var newPos = _player!.Pos + delta;

        var target = _enemies.FirstOrDefault(e => e.Pos == newPos);
        if (target != null)
        {
            _player.Attack(target);
            return;
        }

        if (_walkables.Contains(newPos))
        {
            var oldPos = _player!.Pos;
            _player!.Pos = newPos;
            _walkables.Remove(newPos);
            _walkables.Add(oldPos);
            updateDiscovered();
            OnPlayerMoved(newPos);
        }
    }

    protected virtual void OnPlayerMoved(Vector2 newPos) { }

    private void initMapTileSets(string map)
    {
        _floor = new TileSet();
        _tunnel = new TileSet();
        _door = new TileSet();
        _decor = new TileSet();
        _exit = new TileSet();
        _portal = new TileSet();

        foreach (var (c, p) in Vector2.Parse(map))
        {
            if (c == '.') _floor.Add(p);
            else if (c == '+') _door.Add(p);
            else if (c == '#') _tunnel.Add(p);
            else if (c == '>') { _floor.Add(p); _decor.Add(p); _exit.Add(p); }
            else if (c == '%') { _floor.Add(p); _decor.Add(p); _portal.Add(p); }
            else if (c != ' ') _decor.Add(p);
        }

        _walkables = _floor.Union(_tunnel).Union(_door).ToHashSet();
    }
}
