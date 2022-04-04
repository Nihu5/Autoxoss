using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class c6 : Effect{
    
    List<EffectDate> effectDates =new List<EffectDate>(){
        new EffectDate(1,EffectDate.EffectType.ARTS,null,null,GameManager.Phase.MAIN),
    };

    public override List<int?> hasEffectTiming(GameManager.Phase effectTiming){
        List<int?> idCount=new List<int?>();
        for(int i=0;i<effectDates.Count;i++){
            if(effectDates[i].effectTiming==effectTiming){
                idCount.Add(effectDates[i].effectId);
            }
        }
        return idCount;
    }


}
