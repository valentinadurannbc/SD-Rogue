using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;

using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

namespace RlGameNS;

public class Level : BaseLevel
{

    protected readonly Random _rng = new();
    private TileSet _lastEnemyPositions = new();
    private TileSet _lastItemPositions = new();

    private int _selectedSlot = -1;
    private bool _awaitingAction = false;

    public Level? Linked { get; set; }

    private const int InvX = 75;

    public Level(Player p, string map, Game game) : base(p, map, game)
    {
        _walkables.Remove(new Vector2(4, 12));
        registerCommandsWithScene();
        spawnEnemies();
        spawnItems();
    }

    protected virtual void spawnEnemies()
    {
        _enemies.Add(spawnEnemyAt(new Snake(Vector2.Zero)));
        _enemies.Add(spawnEnemyAt(new Orc(Vector2.Zero)));
        _enemies.Add(spawnEnemyAt(new Skeleton(Vector2.Zero)));
    }

    protected virtual void spawnItems()
    {
        spawnItemAt(new Apple(Vector2.Zero));
        spawnItemAt(new Apple(Vector2.Zero));
        spawnItemAt(new Apple(Vector2.Zero));
        spawnItemAt(new IronSword(Vector2.Zero));
        spawnItemAt(new BaseballBat(Vector2.Zero));
    }

    protected IActor spawnEnemyAt(IActor enemy)
    {
        var tiles = _walkables.Except(_portal).ToList();
        if (tiles.Count == 0) tiles = _walkables.ToList();
        var pos = tiles[_rng.Next(tiles.Count)];
        enemy.Pos = pos;
        _walkables.Remove(pos);
        return enemy;
    }

    protected void spawnItemAt(IItem item)
    {
        var tiles = _walkables.Except(_portal).ToList();
        if (tiles.Count == 0) tiles = _walkables.ToList();
        var pos = tiles[_rng.Next(tiles.Count)];
        _items.Add(createItemAt(item, pos));
    }

    protected IItem createItemAt(IItem template, Vector2 pos)
    {
        return template switch
        {
            Apple => new Apple(pos),
            IronSword => new IronSword(pos),
            BaseballBat => new BaseballBat(pos),
            Key => new Key(pos),
            _ => template
        };
    }

    public override void Update()
    {
        _player!.Update();
        checkItemPickup();
        foreach (var enemy in _enemies.ToList())
            enemy.Update(_walkables, _rng, _player!);
        removeDeadEnemies();
        if (_player!.IsDead)
            _game!.SwitchScene(new DeathScene(_player!, _game!));
    }

    private void checkItemPickup()
    {
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            if (_items[i].Pos == _player!.Pos)
            {
                if (_player!.AddToInventory(_items[i]))
                    _items.RemoveAt(i);
            }
        }
    }

    private void removeDeadEnemies()
    {
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            if (_enemies[i].Hp <= 0)
            {
                _walkables.Add(_enemies[i].Pos);
                _player!.GainExp(_enemies[i].ExpValue);
                _enemies.RemoveAt(i);
            }
        }
    }

    public override void Draw(IRenderWindow? disp)
    {
        var allMapTiles = new TileSet(_decor);
        allMapTiles.UnionWith(_floor);
        allMapTiles.UnionWith(_tunnel);
        allMapTiles.UnionWith(_door);

        var tilesToDraw = new TileSet(allMapTiles);
        tilesToDraw.IntersectWith(_discovered);
        tilesToDraw.UnionWith(_inFov);
        tilesToDraw.UnionWith(_lastEnemyPositions);
        tilesToDraw.UnionWith(_lastItemPositions);

        disp.fDraw(tilesToDraw, _map, ConsoleColor.Gray);

        foreach (var pt in _portal)
        {
            if (_inFov.Contains(pt)) disp.Draw('%', pt, ConsoleColor.Magenta);
            else if (_discovered.Contains(pt)) disp.Draw('%', pt, ConsoleColor.DarkMagenta);
        }

        if (_player.Turn % 5 == 0)
            _player._color = (ConsoleColor)_rng.Next(10, 16);
        _player!.Draw(disp);

        drawItems(disp);
        drawEnemies(disp);
        drawInventory(disp);
        disp.Draw(_player.HUD, new Vector2(0, 24), ConsoleColor.Green);
    }

    public override void DoCommand(Command command)
    {
        if (_awaitingAction)
        {
            handleActionCommand(command.Name);
            return;
        }

        switch (command.Name)
        {
            case "up": MovePlayer(Vector2.N); break;
            case "down": MovePlayer(Vector2.S); break;
            case "left": MovePlayer(Vector2.W); break;
            case "right": MovePlayer(Vector2.E); break;
            case "quit": QuitLevel(); break;
            case "slot1": selectSlot(0); break;
            case "slot2": selectSlot(1); break;
            case "slot3": selectSlot(2); break;
            case "slot4": selectSlot(3); break;
            case "slot5": selectSlot(4); break;
        }
    }

    protected override void OnPlayerMoved(Vector2 newPos)
    {
        if (Linked != null && _portal.Contains(newPos))
            transitionToLinked();
    }

    private void transitionToLinked()
    {
        var arrivalPos = Linked!.Portal.First();
        PlayerLeave();
        Linked.PlayerEnter(arrivalPos);
        _game!.SwitchScene(Linked);
    }

    private void selectSlot(int slot)
    {
        if (_player!.GetInventoryItem(slot) == null) return;
        _selectedSlot = slot;
        _awaitingAction = true;
    }

    private void handleActionCommand(string name)
    {
        switch (name)
        {
            case "use": useSelectedItem(); break;
            case "drop": dropSelectedItem(); break;
            case "cancel": cancelSelection(); break;
        }
    }

    private void useSelectedItem()
    {
        var item = _player!.GetInventoryItem(_selectedSlot);
        if (item is Key)
        {
            if (_exit.Contains(_player!.Pos))
                _game!.SwitchScene(new WinScene(_player!, _game!));
        }
        else
        {
            _player!.UseItem(_selectedSlot);
        }
        cancelSelection();
    }

    private void dropSelectedItem()
    {
        var item = _player!.GetInventoryItem(_selectedSlot);
        if (item is Key)
        {
            cancelSelection();
            return;
        }
        _player!.RemoveFromInventory(_selectedSlot);
        cancelSelection();
    }

    private void cancelSelection()
    {
        _selectedSlot = -1;
        _awaitingAction = false;
    }

    private void drawInventory(IRenderWindow disp)
    {
        for (int i = 0; i < 5; i++)
        {
            var item = _player!.Inventory[i];
            var label = item == null ? "---" : item.Name;
            ConsoleColor color;
            if (i == _selectedSlot)
            {
                color = ConsoleColor.Cyan;
            }
            else if (item != null && item == _player!.EquippedWeapon)
            {
                color = ConsoleColor.Yellow;
            }
            else if (item != null)
            {
                color = ConsoleColor.White;
            }
            else
            {
                color = ConsoleColor.DarkGray;
            }
            disp.Draw($"{i + 1}:{label,-12}", new Vector2(InvX, i), color);
        }

        if (_awaitingAction)
            disp.Draw("U:Use  R:Drop  C:Cancel", new Vector2(InvX, 5), ConsoleColor.DarkCyan);
        else
            disp.Draw("                       ", new Vector2(InvX, 5), ConsoleColor.DarkGray);
    }

    private void drawItems(IRenderWindow disp)
    {
        _lastItemPositions.Clear();
        foreach (var item in _items)
        {
            if (_inFov.Contains(item.Pos))
            {
                item.Draw(disp);
                _lastItemPositions.Add(item.Pos);
            }
        }
    }

    private void drawEnemies(IRenderWindow disp)
    {
        _lastEnemyPositions.Clear();
        foreach (var enemy in _enemies)
        {
            if (_inFov.Contains(enemy.Pos))
            {
                enemy.Draw(disp);
                _lastEnemyPositions.Add(enemy.Pos);
            }
        }
    }


    // ------------------------------------------------------
    // Commands 
    // ------------------------------------------------------


    private void registerCommandsWithScene()
    {
        RegisterCommand(ConsoleKey.UpArrow, "up");
        RegisterCommand(ConsoleKey.W, "up");
        RegisterCommand(ConsoleKey.K, "up");

        RegisterCommand(ConsoleKey.DownArrow, "down");
        RegisterCommand(ConsoleKey.S, "down");
        RegisterCommand(ConsoleKey.J, "down");

        RegisterCommand(ConsoleKey.LeftArrow, "left");
        RegisterCommand(ConsoleKey.A, "left");
        RegisterCommand(ConsoleKey.H, "left");

        RegisterCommand(ConsoleKey.RightArrow, "right");
        RegisterCommand(ConsoleKey.D, "right");
        RegisterCommand(ConsoleKey.L, "right");

        RegisterCommand(ConsoleKey.Q, "quit");

        RegisterCommand(ConsoleKey.D1, "slot1");
        RegisterCommand(ConsoleKey.D2, "slot2");
        RegisterCommand(ConsoleKey.D3, "slot3");
        RegisterCommand(ConsoleKey.D4, "slot4");
        RegisterCommand(ConsoleKey.D5, "slot5");

        RegisterCommand(ConsoleKey.U, "use");
        RegisterCommand(ConsoleKey.R, "drop");
        RegisterCommand(ConsoleKey.C, "cancel");
        RegisterCommand(ConsoleKey.Escape, "cancel");
    }
}
