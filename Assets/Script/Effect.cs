using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Effect:MonoBehaviour{
    
    protected bool effectProcessCheck=false;

    public virtual void test(){
        Debug.Log("オーバーライドされてないよ");
    }
    public virtual List<int?> hasEffectType(EffectDate.EffectType effectType){
        return new List<int?>();
    }
    public virtual List<int?> hasEffectTiming(GameManager.Phase effectTiming){
        return new List<int?>();
    }
    public virtual void LifeBurst(Player player){
        effectProcessCheck=true;
    }
    public bool effectProcessFinish(){
        return effectProcessCheck;
    }

    public virtual void Effect1(Player player,Player enemyPlayer){

    }
    public virtual void Effect2(Player player,Player enemyPlayer){

    }
    public virtual void Effect3(Player player,Player enemyPlayer){

    }
}
