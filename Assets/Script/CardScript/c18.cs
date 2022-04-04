using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class c18 : Effect{

    List<EffectDate> effectDates =new List<EffectDate>(){
        new EffectDate(1,EffectDate.EffectType.SPELL,null,null,0),
    };

    public override void Effect1(Player player, Player enemyPlayer){
        effectProcessCheck=false;
        StartCoroutine(EffectProcess(player,r=>effectProcessCheck=r));
    }

    public override void LifeBurst(Player player){
        effectProcessCheck=false;
        StartCoroutine(LifeBurstProcess(player,r=>effectProcessCheck=r));
    }

    IEnumerator EffectProcess(Player player,Action<bool> callback){
        player.Draw(1);
        yield return new WaitForSeconds(0.5f);
        callback(true);
    }

    IEnumerator LifeBurstProcess(Player player,Action<bool> callback){
        player.EnerCharge(1);
        yield return new WaitForSeconds(0.5f);
        callback(true);
    }

}
