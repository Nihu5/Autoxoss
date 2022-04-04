using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardData {
    [Flags]public enum Color{
        WHITE=0x01,
        RED=0x02,
        BULE=0x04,
        GREEN=0x08,
        BRACK=0x10,
        COLORLESS=0x20,

    };
    public enum CardType{
        NULL,
        SIGNI,
        SPELL,
        LRIG,
        ARTS,
        REZONA,

    };
    [Flags] public enum LrigType{
        TAMA=0x0001,
        HANAYO=0x0002,
        PILURUK=0x0004,

    };
    [Flags]public enum RaceType{
        SEIZOU=0x0001,
        SEIBU=0x0002,
        SEIRA=0x0004,
        SEIKAI=0x0008,
        SEISEI=0x0010,
        SEIGEN=0x0020,

        ARM=0x0040,
        
    }

    public int id;
    public string name;
    public int? at;
    public int? lv;
    public int? limit;
    public CardType cardType;
    
    public LrigType? lrigType;
    public RaceType? raceType;
    public LrigType? lrigLimit;
    public Color color;
    public List<Color> cost =new List<Color>();
    public bool effect;
    public bool lifeBurst;
    
    
    
    public CardData(int _id, string _name, int? _at, int? _lv ,int? _limit,
                        CardType _cardType, LrigType? _lrigType, RaceType? _raceType, LrigType? _lrigLimit,
                        Color _color, List<Color> _cost, bool _effect, bool _lifeBurst) {
        id = _id;
        name = _name;
        at = _at;
        lv = _lv;
        limit=_limit;
        cardType=_cardType;
        lrigType=_lrigType;
        raceType=_raceType;
        lrigLimit=_lrigLimit;
        color=_color;
        cost=_cost;
        effect=_effect;
        lifeBurst=_lifeBurst;
    }
    
}