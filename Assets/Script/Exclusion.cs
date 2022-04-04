using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exclusion : MonoBehaviour{
    [SerializeField]Player player;
    public List<Card> cardList = new List<Card>();

    public void Add(Card _card) {
        _card.transform.SetParent(this.transform,false);
        _card.transform.localPosition = Vector3.zero;
        _card.transform.localRotation = Quaternion.identity;
        _card.ZoneSet(GameManager.Zone.EXCLUSIONZONE);
        cardList.Add(_card);
    }
    public Card Pull(int _position) {
        Card card = cardList[_position];
        cardList.Remove(card);
        return card;
    }
    
}
