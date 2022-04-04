using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class c15 : Effect{
    List<EffectDate> effectDates =new List<EffectDate>(){
        new EffectDate(1,EffectDate.EffectType.SPELL,null,null,0),
    };

    public override void Effect1(Player player, Player enemyPlayer){
        effectProcessCheck=false;
        StartCoroutine(EffectProcess(player,r=>effectProcessCheck=r));
    }
    IEnumerator EffectProcess(Player player,Action<bool> callback){
        List<Card> targetCards=new List<Card>();
        for(int i=0;i<player.deck.cardList.Count;i++){   
            if(player.deck.cardList[i].card_Type()==CardData.CardType.SIGNI){
                targetCards.Add(player.deck.cardList[i]);
            }
        }
        if(targetCards.Count>0){
            yield return StartCoroutine(player.gameManager.selectUI.DisplaySelectUI(targetCards,0,1,"手札に加えるカードを選択してください"));
            while (!player.gameManager.selectUI.SelectConfirm()) {
                yield return null;
            }
            List<Card> targetCard=player.gameManager.selectUI.SelectResult();
            yield return StartCoroutine(player.gameManager.selectUI.DisplayEnd());
            if(targetCard.Count>0){
                Card card=player.deck.Pull(targetCard[0]);
                player.DeckShuffle();
                yield return StartCoroutine(player.CardMotionWait(0.3f,player.hand.transform.position,card,GameManager.Zone.HAND));
            }
        }
        //yield return new WaitForSeconds(0.5f);
        callback(true);
    }

}
