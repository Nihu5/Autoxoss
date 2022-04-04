using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterZone : MonoBehaviour{
    [SerializeField]Player player;
    private List<Card> cardList = new List<Card>();
    public void Add(Card _card) {
        _card.transform.SetParent(this.transform,false);
        _card.transform.localPosition = Vector3.zero;
        _card.transform.localRotation = Quaternion.identity;
        _card.ZoneSet(GameManager.Zone.CENTERZONE);
        cardList.Add(_card);
    }
    public Card Pull(int _position) {
        Card card = cardList[_position];
        cardList.Remove(card);
        card.transform.SetParent(player.transform);
        return card;
    }
    public Card Pull(Card card) {
        cardList.Remove(card);
        card.transform.SetParent(player.transform);
        return card;
    }
    public void down(int _position) {
        Card card = cardList[_position];
        card.transform.Rotate(0.0f,0.0f,90.0f);
    }
    public Card Get(int position) {
        return cardList[position];
    }
    public List<Card> cards(){
        return cardList;
    }
    public int? CenterSigniNowLevel(){
        Debug.Log("中央のシグニのレベル取得");
        if(cardList.Count!=0){
            Debug.Log("0じゃないよ");
            return cardList[cardList.Count-1].cardLevel();
        }else{
            return null;
        }
    }
}
