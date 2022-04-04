using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class c11 : Effect{
    

    public override void LifeBurst(Player player){
        effectProcessCheck=false;
        StartCoroutine(LifeBurstProcess(player,r=>effectProcessCheck=r));
    }

    IEnumerator LifeBurstProcess(Player player,Action<bool> callback){
        player.Draw(1);
        yield return new WaitForSeconds(0.5f);
        callback(true);
    }

}
