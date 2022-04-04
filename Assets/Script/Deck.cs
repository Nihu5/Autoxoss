using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour{

    [SerializeField]Player player;
    public List<Card> cardList = new List<Card>();

    public void Add(Card _card) {
        _card.transform.SetParent(this.transform,false);
        _card.transform.localPosition = Vector3.zero;
        _card.transform.localRotation = Quaternion.identity;
        _card.ImageReverseChange();
        _card.ZoneSet(GameManager.Zone.DECK);
        _card.PlayerSet(player);
        _card.image.raycastTarget=false;
        cardList.Add(_card);
    }
    public void GenerateAdd(Card _card) {
        _card.transform.SetParent(this.transform,false);
        _card.transform.localPosition = Vector3.zero;
        _card.transform.localRotation = Quaternion.identity;
        _card.ZoneSet(GameManager.Zone.DECK);
        _card.PlayerSet(player);
        _card.image.raycastTarget=false;
        cardList.Add(_card);
    }
    public Card Pull(int _position) {
        Card card = cardList[_position];
        cardList.Remove(card);
        card.image.raycastTarget=true;
        card.transform.SetParent(player.transform);
        return card;
    }
    public Card Pull(Card card) {
        cardList.Remove(card);
        card.image.raycastTarget=true;
        card.transform.SetParent(player.transform);
        return card;
    }
}
