using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EffectDate{
    
    public int? effectId;
    [Flags] public enum EffectType{
        GUARD           =0x00000001,
        MULTIENER       =0x00000002,
        CONTINUOUS      =0x00000004,
        APPEAR          =0x00000008,
        AUTO            =0x00000010,
        ACTIVATION      =0x00000020,
        TRIGGER         =0x00000040,
        SPELL           =0x00000080,
        ARTS            =0x00000100,
    };
    public EffectType? effectType;
    [Flags] public enum EffectTrigger{
        TURN      =0x0001,
        
    }
    public EffectTrigger? effectTrigger;
    public List<CardData.Color> effectCost=new List<CardData.Color>();
    public GameManager.Phase effectTiming;
    
    public EffectDate(int? effectId, EffectType? effectType,EffectTrigger? effectTrigger,List<CardData.Color> effectCost,GameManager.Phase effectTiming){
        this.effectId=effectId;
        this.effectType=effectType;
        this.effectTrigger=effectTrigger;
        this.effectCost=effectCost;
        this.effectTiming=effectTiming;
    }
    
}