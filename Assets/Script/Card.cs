using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Card : MonoBehaviour {
    [SerializeField]int id;
    public Text atText;
    [SerializeField]string cardName;
    [SerializeField] int? at;
    public int? lv;
    [SerializeField]int? limit;
    [SerializeField]CardData.CardType cardType;
    [SerializeField]CardData.LrigType? lrigType;
    [SerializeField]CardData.RaceType? raceType;
    [SerializeField]CardData.LrigType? lrigLimit;
    CardData.Color color;
    List<CardData.Color> cost =new List<CardData.Color>();
    bool effecthas;
    bool lifeBurst;
    Player player;
    public Image image;
    public Sprite sprite;

    [SerializeField] Image button;
    private RaycastDetector Rd;
    private EventTrigger Trigger;
    private GameManager Gm;
    private GameManager.Zone zone;
    private Effect effect;

    private int? effectAt ;

    private void Start() {
        Rd=GameObject.Find("RaycastDetector").GetComponent<RaycastDetector>();
        Trigger=GetComponent<EventTrigger>();
        AddEvent();
    }

    public void Load(CardData _cardData) {
        Gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.id=_cardData.id;
        cardName = _cardData.name;
        at = _cardData.at;
        effectAt=at;
        atText.text = at.ToString();
        lv = _cardData.lv;
        limit=_cardData.limit;
        cardType=_cardData.cardType;
        lrigType=_cardData.lrigType;
        raceType=_cardData.raceType;
        lrigLimit=_cardData.lrigLimit;
        color=_cardData.color;
        cost=_cardData.cost;
        effecthas=_cardData.effect;
        lifeBurst=_cardData.lifeBurst;
        switch(cardType){
            case CardData.CardType.NULL:
                break;
            case CardData.CardType.SIGNI:
            case CardData.CardType.SPELL:
                sprite = Gm.deckGenerater.ImageLoad("WX00-000");
                break;
            default:
                sprite = Gm.deckGenerater.ImageLoad("WX00-001");
                break;
        }
        image = this.GetComponent<Image>();
        image.sprite = sprite;
        if(effecthas||lifeBurst){
            effect=(Effect)gameObject.AddComponent(Type.GetType("c"+id.ToString()));
        }
    }
    public int? cardId(){
        return id;
    }
    public int? cardAt(){
        if(effectAt!=at){
            return effectAt;
        }
        return at;
    }
    public int? cardEffectAt(){
        return effectAt;
    }
    public string CardName(){
        return cardName;
    }
    public int? cardLevel(){
        return lv;
    }
    public int? cardlimit(){
        return limit;
    }
    public CardData.CardType card_Type(){
        return cardType;
    }
    public CardData.Color cardColor(){
        return color;
    }
    public Effect CardEffect(){
        return effect;
    }
    public bool CardHasEffect(){
        return effecthas;
    }
    public bool CardHasLifeBurst(){
        return lifeBurst;
    }
    public List<CardData.Color> CardCost(){
        return cost;
    }
    public GameManager.Zone NowZone(){
        return zone;
    }
    public Player CardHasPlayer(){
        return player;
    }
    public Image CardImage(){
        return image;
    }
    public void ZoneSet(GameManager.Zone _zone){
        zone=_zone;
    }
    public void PlayerSet(Player player){
        this.player=player;
    }
    public void ImageObverseChange(){
        sprite = Gm.deckGenerater.ImageLoad(cardName);
        image.sprite = sprite;
    }
    public void ImageReverseChange(){
        sprite = Gm.deckGenerater.ImageLoad("WX00-000");
        image.sprite = sprite;
    }
    public void LrigImageReverseChange(){
        sprite = Gm.deckGenerater.ImageLoad("WX00-001");
        image.sprite = sprite;
    }

    public void AddEvent(){
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener( (x) => ButtonActive() );
        Trigger.triggers.Add(entry);

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerEnter;
        entry2.callback.AddListener( (x) => Gm.selectUI.sideImageChange(id,cardName) );
        Trigger.triggers.Add(entry2);
    }
    public void ButtonActive(){
        Debug.Log("ボタン作る");
        if(Gm.playerList[0].HandButtonEnable(this)){
            if(Rd.InsButton(this,this.transform)==true){
                switch(Gm.NowPhase()){
                    case GameManager.Phase.MAIN:
                        if(card_Type()==CardData.CardType.SIGNI){
                            Debug.Log("メインイベント追加");
                            Rd.AddText("場に出す");
                            Rd.AddMainEvent();
                            break;
                        }
                        if(card_Type()==CardData.CardType.SPELL){
                            Debug.Log("メインイベント追加");
                            Rd.AddText("発動");
                            Rd.AddSpellEvent(this);
                            break;
                        }
                        break;
                    case GameManager.Phase.SIGNIATTACK:
                        Rd.AddText("アタック");
                        Rd.AddSigniAttackEvent(this);
                        break;
                    case GameManager.Phase.LRIGATTACK:
                        Rd.AddText("アタック");
                        Rd.AddLrigAttackEvent(this);
                        break;
                }
            }
        }        
    }
    public void SpelLArtsEffect(){
        Gm.SpellArtsEffect(this);
    }
    public void SigniAttackEvent() {
        Debug.Log("シグニがアタックした");
        Gm.SigniBattle(this);
    }
    public void LrigAttackEvent(){
        Debug.Log("ルリグがアタックした");
        StartCoroutine(Gm.LrigAttack());
    }
    public bool Attack(Card card) {
        if (cardAt()>=card.cardAt()) {
            return true;
        }
        return false;
    }
    public IEnumerator Up(Motion motion){
        yield return StartCoroutine(motion.CardRotationMotion(this.transform,0.3f,Quaternion.Euler(0,0,0)));
    }
    public IEnumerator Down(Motion motion){
        yield return StartCoroutine(motion.CardRotationMotion(this.transform,0.3f,Quaternion.Euler(0,0,-90)));
    }
    public bool NowDown(){
        return this.transform.localRotation==Quaternion.Euler(0,0,-90);
    }
}
