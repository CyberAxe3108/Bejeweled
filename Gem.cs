using System;
using System.Collections.Generic;


namespace Bejeweled;

public enum GemType { Ruby, Sapphire, Emerald, Topaz, Amethyst, Diamond }
public enum SpecialGemType { None, PowerGem, HyperCube, SuperNova }

public class Gem
{
    public GemType Type { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public bool IsSelected { get; set; }
    public SpecialGemType SpecialType { get; private set; }

    public bool IsSpecial => SpecialType != SpecialGemType.None;

    // public int PointValue => SpecialType switch
    // {
    //     SpecialGemType.PowerGem  => 200,
    //     SpecialGemType.HyperCube => 500,
    //     SpecialGemType.SuperNova => 1000,
    //     _                        => 100
    // };

    public Gem(GemType type, int row = 0, int column = 0)
    {
        Type = type;
        Row = row;
        Column = column;
        IsSelected = false;
        SpecialType = SpecialGemType.None;
    }

    public (int Row, int Column) GetPosition() => (Row, Column);

    public void SetSelected(bool selected) => IsSelected = selected;

    public void SetSpecialType(SpecialGemType type) => SpecialType = type;

}