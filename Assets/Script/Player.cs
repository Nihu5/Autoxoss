using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Player : MonoBehaviour{
    public bool AI;
    public GameManager gameManager;
    [SerializeField] Motion motion;
    public Deck deck;
    public LrigDeck lrigDeck;
    public Hand hand;
    public CenterZone centerZone;
    public LeftZone leftZone;
    public RightZone rightZone;
    public Field field;
    public LrigZone lrigZone;
    public Graveyard Graveyard;
    public LrigGraveyard lrigGraveyard;
    public EnerZone enerZone;
    public LifeClote lifeClote;
    public CheckZone checkZone;
    public Player enemyPlayer;

    public void LrigSet(){
        for(int i=0;i<lrigDeck.cardList.Count;i++){
            if(lrigDeck.cardList[i].cardLevel()==0){
                Card card=lrigDeck.Pull(i);
                lrigZone.Add(card);
                card.image.raycastTarget=false;
                card.LrigImageReverseChange();
            }
        }
    }
    public IEnumerator Up(List<Card> cards){
        for(int i=0;i<cards.Count;i++){
            if(cards[i].NowDown()){
                yield return StartCoroutine(cards[i].Up(motion));
            }
        }
    }
    public IEnumerator LrigUp(){
        if(lrigZone.cards()[lrigZone.cards().Count-1].NowDown()){
            yield return StartCoroutine(lrigZone.cards()[lrigZone.cards().Count-1].Up(motion));
        }
    }
    public void Draw(int count) {
        StartCoroutine(CardMotionCount(0.25f, hand.transform.position,count,GameManager.Zone.HAND));
    }
    public void EnerCharge(int count){
        StartCoroutine(CardMotionCount(0.2f, enerZone.transform.position,count,GameManager.Zone.ENERZONE));
    }
    public void LifeCloteCharge(int count){
        StartCoroutine(CardMotionCount(0.25f, lifeClote.transform.position,count,GameManager.Zone.LIFECLOTE));
    }
    public IEnumerator EnerCharge(List<Card> cards){
        Card card=null;
        for(int i=0;i<cards.Count;i++){
            switch(cards[i].NowZone()){
                case GameManager.Zone.HAND:
                    card = hand.Pull(cards[i]);
                    break;
                case GameManager.Zone.CHECKZONE:
                    card=checkZone.Pull(cards[i]);
                    break;
                case GameManager.Zone.CENTERZONE:
                    card=centerZone.Pull(cards[i]);
                    break;
                case GameManager.Zone.LEFTZONE:
                    card=leftZone.Pull(cards[i]);
                    break;
                case GameManager.Zone.RIGHTZONE:
                    card=rightZone.Pull(cards[i]);
                    break;
            }
            Debug.Log(cards.Count);
            yield return StartCoroutine(CardMotionWait(0.28f,enerZone.transform.position,card,GameManager.Zone.ENERZONE));
        }
    }
    //aiのやつ
    public void MainPhaseAction() {
        if (centerZone.cards().Count == 0) {
            //Card card=null;
            for(int i=0;i<hand.cardList.Count;i++){
                if(summonEnable(hand.cardList[i])){
                    hand.cardList[i].ImageObverseChange();
                    field.AddfromHandtoZone(hand.cardList[i],GameManager.Zone.CENTERZONE);
                    return;
                }
            }
        }
    }
    public IEnumerator SigniBattleAction(Player enemyplayer,Card card) {
        Debug.Log("BattleAction");
        gameManager.raycastDetector.ButtonDestroy();
        switch(card.NowZone()){
            case GameManager.Zone.CENTERZONE:
                if(enemyplayer.centerZone.cards().Count > 0) {
                    Card enemycard = SelectTarget(enemyplayer.centerZone);
                    yield return StartCoroutine(card.Down(motion));
                    if (card.Attack(enemycard)) {
                        //敵モーション修正
                        //enemyplayer.SendEnerZone(enemycard);
                        List<Card> tmpCard=new List<Card>();
                        tmpCard.Add(enemycard);
                        yield return StartCoroutine(enemyplayer.EnerCharge(tmpCard));
                    }
                }else{
                    yield return StartCoroutine(Attack(enemyplayer,card));
                }
                break;
            case GameManager.Zone.LEFTZONE:
                if(enemyplayer.leftZone.cards().Count > 0) {
                    Card enemycard = SelectTarget(enemyplayer.leftZone);
                    yield return StartCoroutine(card.Down(motion));
                    if (card.Attack(enemycard)) {
                        List<Card> tmpCard=new List<Card>();
                        tmpCard.Add(enemycard);
                        yield return StartCoroutine(enemyplayer.EnerCharge(tmpCard));
                    }
                }else{
                    yield return StartCoroutine(Attack(enemyplayer,card));
                }
                break;
            case GameManager.Zone.RIGHTZONE:
                if(enemyplayer.rightZone.cards().Count > 0) {
                    Card enemycard = SelectTarget(enemyplayer.rightZone);
                    yield return StartCoroutine(card.Down(motion));
                    if (card.Attack(enemycard)) {
                        List<Card> tmpCard=new List<Card>();
                        tmpCard.Add(enemycard);
                        yield return StartCoroutine(enemyplayer.EnerCharge(tmpCard));
                    }
                }else{
                    yield return StartCoroutine(Attack(enemyplayer,card));
                }
                break;
        }
    }
    public IEnumerator Attack(Player enemyPlayer,Card card){
        gameManager.raycastDetector.ButtonDestroy();
        yield return StartCoroutine(card.Down(motion));
        if(enemyPlayer.lifeClote.cards().Count>0){
            yield return StartCoroutine(enemyPlayer.LifeCloteVanish(0.3f));
        }else{
            gameManager.GameFinish(this);
        }
    }
    public int DecktoAdd(List<Card> cards,string zone){
        Card card=null;
        int i;
        for (i = 0; i < cards.Count; i++){
            if(zone=="HAND"){
                card = hand.Pull(cards[i]);
            }
            Debug.Log(cards.Count);
            deck.Add(card);
        }
        deck.cardList=deck.cardList.Shuffle().ToList();
        return i;
    }
    public IEnumerator DecktoAdd(List<Card> cards,GameManager.Zone zone,Action<int> callback){
        Card card=null;
        int i;
        for(i=0;i<cards.Count;i++){
            switch(zone){
                case GameManager.Zone.HAND:
                    card=hand.Pull(cards[i]);
                    break;
            }
            yield return StartCoroutine(CardMotionWait(0.2f,deck.transform.position,card,GameManager.Zone.DECK));
        }
        DeckShuffle();
        callback(i);
    }
    public void DeckShuffle(){
        deck.cardList=deck.cardList.Shuffle().ToList();
    }
    //count統合
    IEnumerator CardMotionCount(float time, Vector3? position,int count,GameManager.Zone zone){
        for(int i=0;i<count;i++){
            Card card=deck.Pull(0);
            yield return StartCoroutine(CardMotionWait(time,position,card,zone));
        }
    }
    //wait統合
    public IEnumerator CardMotionWait(float time, Vector3? position, Card card,GameManager.Zone zone){
        yield return StartCoroutine(motion.CardPotionMotion(card.transform, time, position));
        switch(zone){
            case GameManager.Zone.HAND:
                if(AI){
                    card.ImageReverseChange();
                }else{
                    card.ImageObverseChange();
                }
                hand.Add(card);
                break;
            case GameManager.Zone.ENERZONE:
                card.ImageObverseChange();
                enerZone.Add(card);
                break;
            case GameManager.Zone.GRAVEYARDZONE:
                card.ImageObverseChange();
                Graveyard.Add(card);
                break;
            case GameManager.Zone.LRIGZONE:
                card.ImageObverseChange();
                lrigZone.Add(card);
                break;
            case GameManager.Zone.CHECKZONE:
                card.ImageObverseChange();
                checkZone.Add(card);
                break;
            case GameManager.Zone.DECK:
                card.ImageReverseChange();
                deck.Add(card);
                break;
            case GameManager.Zone.LIFECLOTE:
                lifeClote.Add(card);
                break;
            case GameManager.Zone.CENTERZONE:
                card.ImageObverseChange();
                centerZone.Add(card);
                break;
            case GameManager.Zone.LEFTZONE:
                card.ImageObverseChange();
                leftZone.Add(card);
                break;
            case GameManager.Zone.RIGHTZONE:
                card.ImageObverseChange();
                rightZone.Add(card);
                break;
            default:
                break;
        }
    }
    public IEnumerator LifeCloteVanish(float time){
        Card card = lifeClote.Pull(lifeClote.cards().Count-1);
        yield return StartCoroutine(CardMotionWait(time,checkZone.transform.position,card,GameManager.Zone.CHECKZONE));
        yield return StartCoroutine(LifeBurstCheck(card));
        yield return StartCoroutine(CardMotionWait(time,enerZone.transform.position,card,GameManager.Zone.ENERZONE));
    }
    public IEnumerator LifeBurstCheck(Card card){
        if(card.CardHasLifeBurst()){
            if(!AI){
                yield return StartCoroutine(gameManager.selectUI.YesNoUIOnDisplay("ライフバーストを使用しますか？"));
                while(gameManager.selectUI.yesOrNoResult()==null){
                    yield return null;
                }
                yield return StartCoroutine(gameManager.selectUI.YesNoUIOnDisplay(null));
                if(gameManager.selectUI.yesOrNoResult()==true){
                    card.CardEffect().LifeBurst(this);
                    while(!card.CardEffect().effectProcessFinish()){
                        yield return null;
                    }
                }
            }
        }
    }
    public IEnumerator SpellArtsEffect(float time, Card card){
        gameManager.raycastDetector.ButtonDestroy();
        yield return StartCoroutine(PayCost(card.CardCost()));
        Card tmpcard=null;
        switch(card.NowZone()){
            case GameManager.Zone.HAND:
                tmpcard=hand.Pull(card);
                break;
            case GameManager.Zone.LRIGDECK:
                tmpcard=lrigDeck.Pull(card);
                break;
        }
        yield return StartCoroutine(CardMotionWait(time,checkZone.transform.position,tmpcard,GameManager.Zone.CHECKZONE));
        card.CardEffect().Effect1(this,enemyPlayer);
        while(!card.CardEffect().effectProcessFinish()){
            yield return null;
        }
        switch(tmpcard.card_Type()){
            case CardData.CardType.SPELL:
                yield return StartCoroutine(CardMotionWait(time,Graveyard.transform.position,tmpcard,GameManager.Zone.GRAVEYARDZONE));
                break;
            case CardData.CardType.ARTS:
                yield return StartCoroutine(CardMotionWait(time,lrigGraveyard.transform.position,tmpcard,GameManager.Zone.LRIGGRAVEYARDZONE));
                break;
        }
    }
    public IEnumerator LrigAttack(Player enemyPlayer,Card card){
        yield return StartCoroutine(card.Down(motion));
        gameManager.raycastDetector.ButtonDestroy();
        List<Card> tmpCards=new List<Card>();
        tmpCards=enemyPlayer.HasServant();
        if(tmpCards.Count>0){
            if(!enemyPlayer.AI){
                yield return StartCoroutine(gameManager.selectUI.YesNoUIOnDisplay("ガードしますか？"));
                while(gameManager.selectUI.yesOrNoResult()==null){
                    yield return null;
                }
                yield return StartCoroutine(gameManager.selectUI.YesNoUIOnDisplay(null));
                if(gameManager.selectUI.yesOrNoResult()==true){
                    //gameManager.selectUI.DisplaySelectUI(tmpCards,1);
                    yield return StartCoroutine(gameManager.selectUI.DisplaySelectUI(tmpCards,1,1,"捨てるカードを選択してください"));
                    while (!gameManager.selectUI.SelectConfirm()) {
                        yield return null;
                    }
                    //モーションいれる
                    Debug.Log(enemyPlayer.hand.cardList.Count);
                    Card tmpcard=enemyPlayer.hand.Pull(gameManager.selectUI.SelectResult()[0]);
                    Debug.Log(enemyPlayer.hand.cardList.Count);
                    //gameManager.selectUI.DisplayEnd();
                    yield return StartCoroutine(gameManager.selectUI.DisplayEnd());
                    yield return StartCoroutine(enemyPlayer.CardMotionWait(0.2f,enemyPlayer.Graveyard.transform.position,tmpcard,GameManager.Zone.GRAVEYARDZONE));
                }else{
                    if(enemyPlayer.lifeClote.cards().Count>0){
                        yield return StartCoroutine(enemyPlayer.LifeCloteVanish(0.2f));
                    }else{
                        gameManager.GameFinish(this);
                    }
                }
            }else{
                //ここも
                Card tmpCard=enemyPlayer.hand.Pull(tmpCards[0]);
                yield return StartCoroutine(enemyPlayer.CardMotionWait(0.2f,enemyPlayer.Graveyard.transform.position,tmpCard,GameManager.Zone.GRAVEYARDZONE));
            }
        }else{
            if(enemyPlayer.lifeClote.cards().Count>0){
                yield return StartCoroutine(enemyPlayer.LifeCloteVanish(0.2f));
            }else{
                gameManager.GameFinish(this);
            }
        }
    }
    public IEnumerator PayCost(List<CardData.Color> cardCost){
        if(!AI){
            Dictionary<CardData.Color,int> costCount=costDictionary(cardCost);
            List<Card> costCards=new List<Card>();
            foreach(CardData.Color key in costCount.Keys){
                List<Card> tmpCard=new List<Card>();
                for(int i=0;i<enerZone.cardList.Count;i++){ 
                    //レベル4グロウ時UIでないエラー 
                    //効果判定？？？
                    if(enerZone.cardList[i].cardColor()==key||
                    enerZone.cardList[i].CardEffect().hasEffectType(EffectDate.EffectType.MULTIENER).Count>0){
                        tmpCard.Add(enerZone.cardList[i]);
                    }
                }
                //gameManager.selectUI.DisplaySelectUI(tmpCard,costCount[key]);
                yield return StartCoroutine(gameManager.selectUI.DisplaySelectUI(tmpCard,0,costCount[key],"コストを支払うカードを選択してください"));
                while (!gameManager.selectUI.SelectConfirm()) {
                    yield return null;
                }
                tmpCard=gameManager.selectUI.SelectResult();
                for(int i=0;i<tmpCard.Count;i++){
                    costCards.Add(tmpCard[i]);
                }
                //gameManager.selectUI.DisplayEnd();
                yield return StartCoroutine(gameManager.selectUI.DisplayEnd());
            }
            for(int i=0;i<costCards.Count;i++){
                Card tmpCard=enerZone.Pull(costCards[i]);
                yield return StartCoroutine(CardMotionWait(0.2f,Graveyard.transform.position,tmpCard,GameManager.Zone.GRAVEYARDZONE));
            }
        }
    }
    public IEnumerator Grow(List<Card> cards){
        if(!AI){
            Dictionary<CardData.Color,int> costCount=costDictionary(cards[0].CardCost());
            List<Card> costCards=new List<Card>();
            foreach(CardData.Color key in costCount.Keys){
                List<Card> tmpCard=new List<Card>();
                for(int i=0;i<enerZone.cardList.Count;i++){ 
                    //レベル4グロウ時UIでないエラー 
                    //効果判定？？？
                    if(enerZone.cardList[i].cardColor()==key||
                    enerZone.cardList[i].CardEffect().hasEffectType(EffectDate.EffectType.MULTIENER).Count>0){
                        tmpCard.Add(enerZone.cardList[i]);
                    }
                }
                //gameManager.selectUI.DisplaySelectUI(tmpCard,costCount[key]);
                yield return StartCoroutine(gameManager.selectUI.DisplaySelectUI(tmpCard,1,costCount[key],"コストを支払うカードを選択してください"));
                while (!gameManager.selectUI.SelectConfirm()) {
                    yield return null;
                }
                tmpCard=gameManager.selectUI.SelectResult();
                for(int i=0;i<tmpCard.Count;i++){
                    costCards.Add(tmpCard[i]);
                }
                //gameManager.selectUI.DisplayEnd();
                yield return StartCoroutine(gameManager.selectUI.DisplayEnd());
            }
            for(int i=0;i<costCards.Count;i++){
                Card tmpCard=enerZone.Pull(costCards[i]);
                yield return StartCoroutine(CardMotionWait(0.2f,Graveyard.transform.position,tmpCard,GameManager.Zone.GRAVEYARDZONE));
            }
            Card card=lrigDeck.Pull(cards[0]);
            yield return StartCoroutine(CardMotionWait(0.2f,lrigZone.transform.position,card,GameManager.Zone.LRIGZONE));
        }else{
            Dictionary<CardData.Color,int> costCount=costDictionary(cards[0].CardCost());
            List<Card> costCards=new List<Card>();
            foreach(CardData.Color key in costCount.Keys){
                List<Card> tmpCard=new List<Card>();
                for(int i=0;i<enerZone.cardList.Count;i++){ 
                    //レベル4グロウ時UIでないエラー 
                    //効果判定？？？
                    if(enerZone.cardList[i].cardColor()==key||
                    enerZone.cardList[i].CardEffect().hasEffectType(EffectDate.EffectType.MULTIENER).Count>0){
                        tmpCard.Add(enerZone.cardList[i]);
                    }
                }
                for(int i=0;i<costCount[key];i++){
                    costCards.Add(tmpCard[i]);
                }
            }
            for(int i=0;i<costCards.Count;i++){
                Card tmpCard=enerZone.Pull(costCards[i]);
                yield return StartCoroutine(CardMotionWait(0.2f,Graveyard.transform.position,tmpCard,GameManager.Zone.GRAVEYARDZONE));
            }
            Card card=lrigDeck.Pull(cards[0]);
            yield return StartCoroutine(CardMotionWait(0.2f,lrigZone.transform.position,card,GameManager.Zone.LRIGZONE));
        }
    }
    Card SelectTarget(CenterZone centerZone) {
        return centerZone.Get(centerZone.cards().Count-1);
    }
    Card SelectTarget(LeftZone leftZone) {
        return leftZone.Get(leftZone.cards().Count-1);
    }
    Card SelectTarget(RightZone rightZone) {
        return rightZone.Get(rightZone.cards().Count-1);
    }
    public bool HandButtonEnable(Card card){
        GameObject gOj=card.transform.parent.gameObject;
        switch(gameManager.NowPhase()){
            case GameManager.Phase.MAIN:
                if(gOj==hand.gameObject){
                    switch(card.card_Type()){
                        case CardData.CardType.SIGNI:
                            if(summonEnable(card)){
                                return true;
                            }
                            break;
                        case CardData.CardType.SPELL:
                            if(hasCost(card.CardCost())){
                                return true;
                            }
                            break;
                    }
                }
                break;
            case GameManager.Phase.SIGNIATTACK:
                if(gOj==centerZone.gameObject||gOj==leftZone.gameObject||gOj==rightZone.gameObject){
                    if(!card.NowDown()&&NowMyTrun()){
                        return true;
                    }
                }
                break;
            case GameManager.Phase.LRIGATTACK:
                if(gOj==lrigZone.gameObject){
                    if(!card.NowDown()&&NowMyTrun()){
                        return true;
                    }
                }
                break;
        }
        return false;
    }
    public List<Card> LrigDeckEffectEnable(){
        List<Card> tmpCards=new List<Card>();
        switch(gameManager.NowPhase()){
            case GameManager.Phase.MAIN:
                foreach(Card target in lrigDeck.cardList){
                    if(target.card_Type()==CardData.CardType.ARTS&&hasCost(target.CardCost())==true&&target.CardEffect().hasEffectTiming(GameManager.Phase.MAIN).Count>0){
                        Debug.Log(hasCost(target.CardCost()));
                        tmpCards.Add(target);
                    }
                }
                break;
            case GameManager.Phase.CURRENTEFFECTSTEP:
            case GameManager.Phase.ENEMYEFFECTSTEP:
                foreach(Card target in lrigDeck.cardList){
                    if(hasCost(target.CardCost())==true&&target.CardEffect().hasEffectTiming(GameManager.Phase.ATTACK).Count>0){
                        tmpCards.Add(target);
                    }
                }
                break;
        }
        return tmpCards;
    }
    public bool summonEnable(Card card){
        Debug.Log("シグニのレベルは"+card.cardLevel());
        if(lrigZone.LrigNowLevel()>=card.cardLevel()){
            if(lrigZone.LrigNowLimit()>=(field.fieldSigniLevelSum()??0)+card.cardLevel()){
                if(field.FieldAllSigni().Count<3){
                    return true;
                }
            }
        }
        Debug.Log("これでたらおかしい");
        return false;
    }
    public bool NowMyTrun(){
        if(gameManager.NowPlayer()==this){
            return true;
        }
        return false;
    }
    public List<Card> HasServant(){
        List<Card> tmpCards=new List<Card>();
        for(int i=0;i<hand.cardList.Count;i++){
            if(hand.cardList[i].CardEffect()==true&&hand.cardList[i].CardEffect().hasEffectType(EffectDate.EffectType.GUARD).Count>0){
                tmpCards.Add(hand.cardList[i]);
            }
        }
        return tmpCards;
    }
    public bool hasCost(List<CardData.Color> cost){
        
        Dictionary<CardData.Color,int> costCount = new Dictionary<CardData.Color,int>();
        Dictionary<CardData.Color,int> enerCount = new Dictionary<CardData.Color,int>();
        int multiEnerCount=0;

        //dictionaryに変換
        for(int i=0;i<enerZone.cardList.Count;i++){
            if(enerZone.cardList[i].CardEffect()==true){
                if(enerZone.cardList[i].CardEffect().hasEffectType(EffectDate.EffectType.MULTIENER).Count>0){
                    multiEnerCount++;
                    Debug.Log(multiEnerCount);
                    continue;
                }
            }
            if(enerCount.ContainsKey(enerZone.cardList[i].cardColor())){
                enerCount[enerZone.cardList[i].cardColor()]++;
            }else{
                enerCount.Add(enerZone.cardList[i].cardColor(),1);
            }
            Debug.Log(enerCount[enerZone.cardList[i].cardColor()]);
        }
        for(int i=0;i<cost.Count;i++){
            if(costCount.ContainsKey(cost[i])){
                costCount[cost[i]]++;
            }else{
                costCount.Add(cost[i],1);
            }
        }
        
        //操作用dictionary作る
        Dictionary<CardData.Color,int> enerCountDummys = new Dictionary<CardData.Color,int>(enerCount);

        //コスト支払う
        foreach(CardData.Color key in enerCountDummys.Keys){
            //色コストあるとき
            if(costCount.ContainsKey(key)){
                //エナゾーンの色の方が多いとき
                if(enerCount[key]>=costCount[key]){
                    enerCount[key]=enerCount[key]-costCount[key];
                    costCount.Remove(key);
                }else{
                    if(enerCount[key]+multiEnerCount>=costCount[key]){
                        costCount[key]-=enerCount[key];
                        multiEnerCount-=costCount[key];
                        costCount.Remove(key);
                    }else{
                        return false;
                    }
                }
            }
            //無色コスト
            if(costCount.ContainsKey(CardData.Color.COLORLESS)){
                if(enerCount[key]>=costCount[CardData.Color.COLORLESS]){
                    enerCount[key]=enerCount[key]-costCount[CardData.Color.COLORLESS];
                    costCount.Remove(CardData.Color.COLORLESS);
                }else{
                    costCount[CardData.Color.COLORLESS]=costCount[CardData.Color.COLORLESS]-enerCount[key];
                }
            }
        }

        Dictionary<CardData.Color,int> costCountDummys = new Dictionary<CardData.Color,int>(costCount);

        foreach(CardData.Color key in costCountDummys.Keys){
            if(multiEnerCount>=costCount[key]){
                multiEnerCount-=costCount[key];
                costCount.Remove(key);
            }else{
                return false;
            }
        }
        if(costCount.Count>0){
            return false;
        }
        return true;
    }
    public Dictionary<CardData.Color,int> costDictionary(List<CardData.Color> cost){
        Dictionary<CardData.Color,int> costCount = new Dictionary<CardData.Color,int>();
        for(int i=0;i<cost.Count;i++){
            if(costCount.ContainsKey(cost[i])){
                costCount[cost[i]]++;
            }else{
                costCount.Add(cost[i],1);
            }
        }
        return costCount;
    }
}
