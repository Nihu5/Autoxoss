using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LrigDeck : MonoBehaviour{
    [SerializeField]Player player;
    [SerializeField] RaycastDetector raycastDetector;
    public List<Card> cardList = new List<Card>();

    public void Add(Card _card) {
        _card.transform.SetParent(this.transform,false);
        _card.transform.localPosition = Vector3.zero;
        _card.transform.localRotation = Quaternion.identity;
        _card.ZoneSet(GameManager.Zone.LRIGDECK);
        _card.image.raycastTarget=false;
        cardList.Add(_card);
    }
    public Card Pull(int _position) {
        Card card = cardList[_position];
        card.image.raycastTarget=true;
        cardList.Remove(card);
        card.transform.SetParent(player.transform);
        return card;
    }
    public Card Pull(Card card) {
        card.image.raycastTarget=true;
        cardList.Remove(card);
        card.transform.SetParent(player.transform);
        return card;
    }
    public List<Card> LevelCards(int? level){
        Debug.Log("レベル探す");
        Debug.Log(level);
        List<Card> tmpCards=new List<Card>();
        for(int i=0;i<cardList.Count;i++){
            if(cardList[i].cardLevel()==level){
                tmpCards.Add(cardList[i]);
            }
        }
        Debug.Log("探し終わった");
        Debug.Log(tmpCards.Count);
        return tmpCards;
    }
    public void ArtsEffectCard(){
        if(player.LrigDeckEffectEnable().Count>0){
            if(raycastDetector.InsButton(null,this.transform)){
                raycastDetector.AddText("発動");
                raycastDetector.AddArtsEvent(this);
            }
        }
    }
    public void AddSelectArts(){
        StartCoroutine(SelectArts());
    }
    public IEnumerator SelectArts(){
        List<Card> tmpCards= new List<Card>();
        tmpCards=player.LrigDeckEffectEnable();
        yield return StartCoroutine(player.gameManager.selectUI.DisplaySelectUI(tmpCards,1,1,"使用するアーツを選択してください"));
        while(!player.gameManager.selectUI.SelectConfirm()){
            yield return null;
        }
        tmpCards=player.gameManager.selectUI.SelectResult();
        yield return StartCoroutine(player.gameManager.selectUI.DisplayEnd());
        yield return StartCoroutine(player.SpellArtsEffect(0.3f,tmpCards[0]));
    }
}
