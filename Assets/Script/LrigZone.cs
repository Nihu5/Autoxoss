using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LrigZone : MonoBehaviour{
    [SerializeField]Player player;
    private List<Card> cardList = new List<Card>();
    [SerializeField] Motion motion;

    public void Add(Card _card) {
        _card.transform.SetParent(this.transform,false);
        _card.transform.localPosition = Vector3.zero;
        _card.transform.localRotation = Quaternion.identity;
        _card.ImageObverseChange();
        _card.ZoneSet(GameManager.Zone.LRIGZONE);
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
    public List<Card> cards(){
        return cardList;
    }
    public Card Get(int position) {
        return cardList[position];
    }
    public IEnumerator Open(){
        yield return StartCoroutine(motion.CardRotationMotion(cardList[cardList.Count-1].transform,0.2f,Quaternion.Euler(0,-87,0)));
        cardList[cardList.Count-1].ImageObverseChange();
        cardList[cardList.Count-1].transform.localRotation=Quaternion.Euler(0,93,0);
        yield return StartCoroutine(motion.CardRotationMotion(cardList[cardList.Count-1].transform,0.2f,Quaternion.Euler(0,0,0)));
        cardList[cardList.Count-1].image.raycastTarget=true;
    }
    public int? LrigNowLevel(){
        Debug.Log("ルリグのレベルは"+cardList[cardList.Count-1].cardLevel());
        return cardList[cardList.Count-1].cardLevel();

    }
    public int? LrigNowLimit(){
        return cardList[cardList.Count-1].cardlimit();
    }
}
