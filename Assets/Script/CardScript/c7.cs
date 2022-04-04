using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class c7 : Effect{
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
    public override void Effect1(Player player, Player enemyPlayer){
        effectProcessCheck=false;
        StartCoroutine(EffectProcess(player,r=>effectProcessCheck=r));
    }
    IEnumerator EffectProcess(Player player,Action<bool> callback){
        List<Card> targetCards=new List<Card>();
        for(int i=0;i<player.deck.cardList.Count;i++){   
            if(player.deck.cardList[i].card_Type()==CardData.CardType.SIGNI&&player.deck.cardList[i].cardColor()==CardData.Color.WHITE){
                targetCards.Add(player.deck.cardList[i]);
            }
        }
        if(targetCards.Count>0){
            yield return StartCoroutine(player.gameManager.selectUI.DisplaySelectUI(targetCards,0,2,"手札に加えるカードを選択してください"));
            while (!player.gameManager.selectUI.SelectConfirm()) {
                yield return null;
            }
            List<Card> targetCard=player.gameManager.selectUI.SelectResult();
            yield return StartCoroutine(player.gameManager.selectUI.DisplayEnd());
            foreach(Card selectCard in targetCard){
                Card card=player.deck.Pull(selectCard);
                yield return StartCoroutine(player.CardMotionWait(0.3f,player.hand.transform.position,card,GameManager.Zone.HAND));
            }
            player.DeckShuffle();
        }
        //yield return new WaitForSeconds(0.5f);
        callback(true);
    }
}
