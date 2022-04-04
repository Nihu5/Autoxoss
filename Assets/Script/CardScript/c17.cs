using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class c17 : Effect{
    
    List<EffectDate> effectDates =new List<EffectDate>(){
        new EffectDate(1,EffectDate.EffectType.GUARD,null,null,0),
        new EffectDate(2,EffectDate.EffectType.MULTIENER,null,null,0),
    };

    public override List<int?> hasEffectType(EffectDate.EffectType effectType){
        List<int?> idCount=new List<int?>();
        for(int i=0;i<effectDates.Count;i++){
            if(effectDates[i].effectType==effectType){
                idCount.Add(effectDates[i].effectId);
            }
        }
        return idCount;
    }
    public override void LifeBurst(Player player){
        effectProcessCheck=false;
        StartCoroutine(LifeBurstProcess(player,r=>effectProcessCheck=r));
    }

    IEnumerator LifeBurstProcess(Player player,Action<bool> callback){
        player.EnerCharge(1);
        yield return new WaitForSeconds(0.5f);
        callback(true);
    }
}
