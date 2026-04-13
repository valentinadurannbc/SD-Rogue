using RogueLib.Dungeon;
using RogueLib.Utilities;
using System.Collections.Generic;
using System.ComponentModel;

public abstract class Player : IActor, IDrawable
{
    public string Name { get; set; }
    public Vector2 Pos { get; set; }
    public char Glyph => '@';
    public ConsoleColor _color = ConsoleColor.White;

    protected int _level = 0;
    protected int _hp = 12;
    protected int _str = 16;
    protected int _arm = 4;
    protected int _exp = 0;
    protected int _gold = 0;
    protected int _maxHp = 12;
    protected int _maxStr = 16;
    protected int _turn = 0;

    private IItem?[] _inventory = new IItem?[5];
    private IItem? _equippedWeapon = null;

    public int Turn => _turn;
    public int Hp => _hp;
    public int Str => _str;
    public int ExpValue => 0;
    public bool IsDead => _hp <= 0;
    public int WeaponBonus => _equippedWeapon?.StrBonus ?? 0;
    public IItem?[] Inventory => _inventory;
    public IItem? EquippedWeapon => _equippedWeapon;

    public Player()
    {
        Name = "Rogue";
        Pos = Vector2.Zero;
    }

    public string HUD =>
       $"Level:{_level}  Gold: {_gold}    Hp: {_hp}({_maxHp})" +
       $"  Str: {_str}({_maxStr}){(WeaponBonus > 0 ? $"+{WeaponBonus}" : "")}" +
       $"  Arm: {_arm}   Exp: {_exp}/5";

    public virtual void Update()
    {
        _turn++;
    }

    public virtual void Update(HashSet<Vector2> walkables, Random rng, Player player)
    {
    }

    public virtual void Heal(int amount)
    {
        _hp = Math.Min(_hp + amount, _maxHp);
    }

    public virtual void TakeDamage(int amount)
    {
        _hp = Math.Max(_hp - amount, 0);
    }

    public void Attack(IActor enemy)
    {
        enemy.TakeDamage(_str + WeaponBonus);
    }

    public void GainExp(int amount)
    {
        _exp += amount;
        if (_exp >= 5)
        {
            _exp -= 5;
            _level++;
            _str++;
            _maxStr++;
            _maxHp += 2;
            _hp = Math.Min(_hp + 2, _maxHp);
        }
    }

    public bool AddToInventory(IItem item)
    {
        for (int i = 0; i < _inventory.Length; i++)
        {
            if (_inventory[i] == null)
            {
                _inventory[i] = item;
                return true;
            }
        }
        return false;
    }

    public IItem? RemoveFromInventory(int slot)
    {
        if (slot < 0 || slot >= _inventory.Length) return null;
        var item = _inventory[slot];
        _inventory[slot] = null;
        if (_equippedWeapon == item) _equippedWeapon = null;
        return item;
    }

    public void UseItem(int slot)
    {
        if (slot < 0 || slot >= _inventory.Length) return;
        var item = _inventory[slot];
        if (item == null) return;
        if (item.IsWeapon)
        {
            _equippedWeapon = item;
        }
        else
        {
            item.Use(this);
            _inventory[slot] = null;
        }
    }

    public IItem? GetInventoryItem(int slot)
    {
        if (slot < 0 || slot >= _inventory.Length) return null;
        return _inventory[slot];
    }

    public virtual void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, _color);
    }
}
