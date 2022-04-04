using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Field : MonoBehaviour{
    
    [SerializeField] LeftZone left;
    [SerializeField] CenterZone center;
    [SerializeField] RightZone right;
    [SerializeField] Player player;
    [SerializeField] Motion motion;
    public int a;

    public string targetField(){

        return null;
    }
    public bool AddfromHandtoZone(Card card,GameManager.Zone zone){
        player.hand.Pull(card);
        switch (zone)
        {
            case GameManager.Zone.CENTERZONE:
                if(center.cards().Count==0){
                    StartCoroutine(player.CardMotionWait(0.3f,center.transform.position,card,GameManager.Zone.CENTERZONE));
                    return true;
                }
                break;
            case GameManager.Zone.LEFTZONE:
                if(left.cards().Count==0){
                    StartCoroutine(player.CardMotionWait(0.3f,left.transform.position,card,GameManager.Zone.LEFTZONE));
                    return true;
                }
                break;
            case GameManager.Zone.RIGHTZONE:
                if(right.cards().Count==0){
                    StartCoroutine(player.CardMotionWait(0.3f,right.transform.position,card,GameManager.Zone.RIGHTZONE));
                    return true;
                }
                break;
            
        }
        return false;
    }
    public int? fieldSigniLevelSum(){
        if(center.CenterSigniNowLevel()==null&&left.LeftSigniNowLevel()==null&&right.RightSigniNowLevel()==null){
            return null;
        }
        return (center.CenterSigniNowLevel()??0)+(left.LeftSigniNowLevel()??0)+(right.RightSigniNowLevel()??0);
    }
    public List<Card> FieldAllSigni(){
        List<Card> fieldSigni=new List<Card>();
        if(center.cards().Count>0){
            fieldSigni.Add(center.cards()[center.cards().Count-1]);
        }
        if(left.cards().Count>0){
            fieldSigni.Add(left.cards()[left.cards().Count-1]);
        }
        if(right.cards().Count>0){
            fieldSigni.Add(right.cards()[right.cards().Count-1]);
        }
        return fieldSigni;
    }
}
