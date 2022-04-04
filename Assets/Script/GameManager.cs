using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class GameManager : MonoBehaviour{

    [Flags]public enum Zone{
        NULL=0x0000,
        DECK=0x0001,
        LRIGDECK=0x0002,
        HAND=0x0004,
        LIFECLOTE=0x0008,
        ENERZONE=0x0010,
        GRAVEYARDZONE=0x0020,
        LRIGGRAVEYARDZONE=0x0040,
        CHECKZONE=0x0080,
        LRIGZONE=0x0100,
        KEYZONE=0x0200,
        CENTERZONE=0x0400,
        LEFTZONE=0x0800,
        RIGHTZONE=0x1000,
        CHEERZONE=0x2000,
        EXCLUSIONZONE=0x4000,
    };
    
    public EventSystem eventSystem;
    public Player[] playerList;
    public Text Text;
    public SelectUI selectUI;
    public RaycastDetector raycastDetector;
    public DeckGenerater deckGenerater;
    public Motion motion;
    bool action = false;
    private int Trun=1;
    private bool Phasetime=false;

    [Flags]public enum Phase {
        NULL=0,
        INIT=0x0001,
        UP=0x0002,
        DRAW=0x0004,
        ENA=0x0008,
        GLOW=0x0010,
        MAIN=0x0020,
        ATTACK=0x0040,
        CURRENTEFFECTSTEP=0x0080,
        ENEMYEFFECTSTEP=0x0100,
        SIGNIATTACK=0x0200,
        LRIGATTACK=0x0400,
        END=0x0800,
    };

    Phase phase;
    void Start(){
        deckGenerater = GetComponent<DeckGenerater>();
        StartCoroutine(StartPhase());
    }

    void Update(){
        switch (phase) {
            case Phase.INIT:
                break;
            case Phase.UP:
                if(!Phasetime){
                    Phasetime=true;
                    StartCoroutine(UpPhase());
                }
                break;
            case Phase.DRAW:
                if(!Phasetime){
                    Phasetime=true;
                    StartCoroutine(DrawPhase());
                }
                break;
            case Phase.ENA:
                if(!Phasetime){
                    Phasetime=true;
                    StartCoroutine(EnerPhase());
                }
                break;
            case Phase.GLOW:
                if(!Phasetime){
                    Phasetime=true;
                    StartCoroutine(GlowPhase());
                }
                break;
            case Phase.MAIN:
                if(!Phasetime){
                    Phasetime=true;
                    MainPhase();
                }
                break;
            case Phase.SIGNIATTACK:
            if(!Phasetime){
                    Phasetime=true;
                    SigniBattlePhase();
                }
                break;
            case Phase.LRIGATTACK:
                if(!Phasetime){
                    Phasetime=true;
                    StartCoroutine(LrigBattlePhase());
                }
                break;
            case Phase.END:
                if(!Phasetime){
                    Phasetime=true;
                    EndPhase();
                }
                break;
        }
    }

    Player currentPlayer;
    Player waitPlayer;

    void InitPhase() {
        Debug.Log("InitPhase");
        List<string> deckTxt= deckGenerater.DeckTectLoad("Deck");
        List<string> lrigDeckTxt= deckGenerater.DeckTectLoad("LrigDeck");
        List<CardData> deckCard=deckGenerater.CardDataGenerate(deckTxt);
        List<CardData> lrigDeckCard=deckGenerater.CardDataGenerate(lrigDeckTxt);

        /*deckGenerater.Generate(
            player1CardDataList, player1CardLrigCardDataList,playerList[0].deck,playerList[0].lrigDeck
        );
        deckGenerater.Generate(
            player1CardDataList, player1CardLrigCardDataList,playerList[1].deck,playerList[1].lrigDeck
        );*/
        
        deckGenerater.Generate(
            deckCard, lrigDeckCard,playerList[0].deck,playerList[0].lrigDeck
        );
        deckGenerater.Generate(
            deckCard, lrigDeckCard,playerList[1].deck,playerList[1].lrigDeck
        );
    
        currentPlayer = playerList[1];
        waitPlayer = playerList[0];

        currentPlayer.LrigSet();
        waitPlayer.LrigSet();

        currentPlayer.LifeCloteCharge(7);
        waitPlayer.LifeCloteCharge(7);

        
    }
    void MainPhase() {
        if(currentPlayer.AI){
            currentPlayer.MainPhaseAction();
            Phasechange();
        }
    }
    void SigniBattlePhase() {
        Debug.Log("SigniBattlePhase");
        if(currentPlayer.AI){
            Phasechange();
        }
    }
    IEnumerator LrigBattlePhase(){
        Debug.Log("LrigBattlePhase");
        if(currentPlayer.AI){
            if(!currentPlayer.lrigZone.Get(currentPlayer.lrigZone.cards().Count-1).NowDown()){
                yield return StartCoroutine(LrigAttack());
                Phasechange();
            }
        }
    }
    void EndPhase() {
        Debug.Log("EndPhase");
        if(currentPlayer.AI){
            Phasechange();
        }
    }

    public void Phasechange() {
        StartCoroutine(PhaseMotion());
    }
    public void PhasechangeUI(){
        if(!NowPlayer().AI){
            StartCoroutine(PhaseMotion());
        }
    }
    public void ButtonOn(){
        action = true;
    }
    public Phase NowPhase() {
        Debug.Log(phase.ToString());
        return phase;
    }
    public Player NowPlayer(){
        return currentPlayer;
    }
    public void SpellArtsEffect(Card card){
        //spellとarts
        StartCoroutine(card.CardHasPlayer().SpellArtsEffect(0.3f,card));
    }
    public void SigniBattle(Card card) {
        //currentPlayer.SigniBattleAction(waitPlayer,card);
        StartCoroutine(currentPlayer.SigniBattleAction(waitPlayer,card));
    }
    public IEnumerator LrigAttack(){
        yield return StartCoroutine(currentPlayer.LrigAttack(waitPlayer,currentPlayer.lrigZone.Get(currentPlayer.lrigZone.cards().Count-1)));
    }
    public void TrunCount(){
        Trun++;
    }
    public int NowTrun(){
        return Trun;
    }
    public void GameFinish(Player winPlayer){
        if(winPlayer==playerList[0]){
            Text.text="あなたの勝利です";
            eventSystem.enabled=false;
            Text.gameObject.SetActive(true);
        }else{
            Text.text="あなたの敗北です";
            eventSystem.enabled=false;
            Text.gameObject.SetActive(true);
        }
    }
    IEnumerator StartPhase() {
        eventSystem.enabled=false;
        Text.text = "バトル開始";
        Text.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        Text.gameObject.SetActive(false);
        InitPhase();        
        yield return new WaitForSeconds(2.5f);
        currentPlayer.Draw(5);
        waitPlayer.Draw(5);
        yield return new WaitForSeconds(5*0.35f);
        eventSystem.enabled=true;
        //selectUI.DisplaySelectUI(currentPlayer.hand.cardList,currentPlayer.hand.cardList.Count);
        if(!currentPlayer.AI){
            yield return StartCoroutine(selectUI.DisplaySelectUI(currentPlayer.hand.cardList,0,currentPlayer.hand.cardList.Count,"引き直すカードを選択してください"));
            while (!selectUI.SelectConfirm()) {
                yield return null;
            }
            int i=0; //=currentPlayer.DecktoAdd(selectUI.SelectResult(),"HAND");
            List<Card> tmpCards=selectUI.SelectResult();
            //selectUI.DisplayEnd();
            yield return StartCoroutine(selectUI.DisplayEnd());
            yield return StartCoroutine(currentPlayer.DecktoAdd(tmpCards,GameManager.Zone.HAND,r=>i=r));
            eventSystem.enabled=false;

            currentPlayer.Draw(i);
            yield return new WaitForSeconds(i*0.35f);
        }else{
            yield return StartCoroutine(selectUI.DisplaySelectUI(waitPlayer.hand.cardList,0,waitPlayer.hand.cardList.Count,"引き直すカードを選択してください"));
            while (!selectUI.SelectConfirm()) {
                yield return null;
            }
            int i=0; //=currentPlayer.DecktoAdd(selectUI.SelectResult(),"HAND");
            List<Card> tmpCards=selectUI.SelectResult();
            //selectUI.DisplayEnd();
            yield return StartCoroutine(selectUI.DisplayEnd());
            yield return StartCoroutine(waitPlayer.DecktoAdd(tmpCards,GameManager.Zone.HAND,r=>i=r));
            eventSystem.enabled=false;

            waitPlayer.Draw(i);
            yield return new WaitForSeconds(i*0.35f);
        }
        Text.text = "オープン！";
        Text.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        Text.gameObject.SetActive(false);

        StartCoroutine(currentPlayer.lrigZone.Open());
        yield return StartCoroutine(waitPlayer.lrigZone.Open());
        //waitPlayer.lrigZone.Open();
        
        //Phasechange();
        Text.text = "ドローフェイズ";
        Text.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        Text.gameObject.SetActive(false);
        phase = Phase.DRAW;
        eventSystem.enabled=true;
    }
    IEnumerator UpPhase(){
        yield return StartCoroutine(currentPlayer.Up(currentPlayer.field.FieldAllSigni()));
        yield return StartCoroutine(currentPlayer.LrigUp());
        Phasechange();
    }
    IEnumerator DrawPhase(){
        Debug.Log("DrawPhase");
        int drawCount;
        if(NowTrun()==1){
            drawCount=1;
        }else{
            drawCount=2;
        }
        if (currentPlayer.deck.cardList.Count >=drawCount) {
            currentPlayer.Draw(drawCount);
            
        }
        yield return new WaitForSeconds(drawCount*0.35f);
        Phasechange();
    }
    IEnumerator EnerPhase(){
        Debug.Log("EnerPhase");
        if(!currentPlayer.AI){
            //yesnoui呼び出し
            yield return StartCoroutine(selectUI.YesNoUIOnDisplay("手札からエナゾーンにカードを置きますか？"));
            //待つ間ループ
            while(selectUI.yesOrNoResult()==null){
                yield return null;
            }
            yield return StartCoroutine(selectUI.YesNoUIOnDisplay(null));
            if(selectUI.yesOrNoResult()==true){
                //手札セレクトui呼び出し
                //ループ
                //selectUI.DisplaySelectUI(currentPlayer.hand.cardList,1);
                yield return StartCoroutine(selectUI.DisplaySelectUI(currentPlayer.hand.cardList,1,1,"エナゾーンに置くカードを選択してください"));
                while (!selectUI.SelectConfirm()) {
                    yield return null;
                }
                //返値
                //エナチャージ
                //currentPlayer.EnerCharge(selectUI.SelectResult());
                List<Card> tmpCards=selectUI.SelectResult();
                //selectUI.DisplayEnd();
                yield return StartCoroutine(selectUI.DisplayEnd());
                yield return StartCoroutine(currentPlayer.EnerCharge(tmpCards));
                yield return new WaitForSeconds(0.3f);
            }else if(currentPlayer.field.FieldAllSigni().Count>0){
                yield return StartCoroutine(selectUI.YesNoUIOnDisplay("場からエナゾーンにカードを置きますか？"));
                while(selectUI.yesOrNoResult()==null){
                    yield return null;
                }
                yield return StartCoroutine(selectUI.YesNoUIOnDisplay(null));
                if(selectUI.yesOrNoResult()==true){
                    //selectUI.DisplaySelectUI(currentPlayer.field.FieldAllSigni(),1);
                    yield return StartCoroutine(selectUI.DisplaySelectUI(currentPlayer.field.FieldAllSigni(),1,1,"エナゾーンに置くカードを選択してください"));
                    while (!selectUI.SelectConfirm()) {
                        yield return null;
                    }
                    //返値
                    //エナチャージ
                    List<Card> cards=selectUI.SelectResult();
                    //currentPlayer.EnerCharge(cards);
                    yield return StartCoroutine(selectUI.DisplayEnd());
                    yield return StartCoroutine(currentPlayer.EnerCharge(cards));
                    //selectUI.DisplayEnd();
                    yield return new WaitForSeconds(0.3f);
                }
            }
        }else{
            if(currentPlayer.hand.cardList.Count>0){
                List<Card> cards=new List<Card>();
                cards.Add(currentPlayer.hand.cardList[0]);
                yield return StartCoroutine(currentPlayer.EnerCharge(cards));
                //currentPlayer.EnerCharge(cards);
                yield return new WaitForSeconds(0.3f);
            }
        }
        Phasechange();        
    }
    IEnumerator GlowPhase(){
        if(!currentPlayer.AI){
            Debug.Log("GlowPhase");
            //yesnoui
            //ループ
            //セレクトui
            //ループ
            //グロウ
            bool costHas=false;
            for(int i=0;i<currentPlayer.lrigDeck.LevelCards(currentPlayer.lrigZone.LrigNowLevel()+1).Count;i++){
                if(currentPlayer.hasCost(currentPlayer.lrigDeck.LevelCards(currentPlayer.lrigZone.LrigNowLevel()+1)[i].CardCost())){
                    costHas=true;
                    break;
                }
            }
            if(costHas){
                yield return StartCoroutine(selectUI.YesNoUIOnDisplay("グロウしますか？"));
                while(selectUI.yesOrNoResult()==null){
                    yield return null;
                }
                yield return StartCoroutine(selectUI.YesNoUIOnDisplay(null));
                if(selectUI.yesOrNoResult()==true){
                    /*selectUI.DisplaySelectUI(
                        currentPlayer.lrigDeck.LevelCards(currentPlayer.lrigZone.LrigNowLevel()+1),
                    1);*/
                    yield return StartCoroutine(selectUI.DisplaySelectUI(
                        currentPlayer.lrigDeck.LevelCards(currentPlayer.lrigZone.LrigNowLevel()+1),1,1,"グロウするカードを選択してください"
                    ));
                    while (!selectUI.SelectConfirm()) {
                        yield return null;
                    }
                    //Displayend先!!!
                    List<Card> tmpCard=selectUI.SelectResult();
                    //selectUI.DisplayEnd();
                    yield return StartCoroutine(selectUI.DisplayEnd());
                    yield return StartCoroutine(currentPlayer.Grow(tmpCard));
                }
            }
        }else{
            bool costHas=false;
            if(currentPlayer.hasCost(currentPlayer.lrigDeck.LevelCards(currentPlayer.lrigZone.LrigNowLevel()+1)[0].CardCost())){
                costHas=true;
            }
            if(costHas){
                yield return StartCoroutine(currentPlayer.Grow(currentPlayer.lrigDeck.LevelCards(currentPlayer.lrigZone.LrigNowLevel()+1)));
            }
        }
        Phasechange();
    }
    IEnumerator PhaseMotion() {
        Debug.Log("button");
        eventSystem.enabled=false;
        raycastDetector.ButtonDestroy();
        switch (phase) {
            case Phase.UP:
                Text.text = "ドローフェイズ";
                Text.gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                Text.gameObject.SetActive(false);
                phase = Phase.DRAW;
                Phasetime=false;
                break;
            case Phase.DRAW:
                Text.text = "エナフェイズ";
                Text.gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                Text.gameObject.SetActive(false);
                phase = Phase.ENA;
                Phasetime=false;
                break;
            case Phase.ENA:
                Text.text = "グロウフェイズ";
                Text.gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                Text.gameObject.SetActive(false);
                phase = Phase.GLOW;
                Phasetime=false;
                break;
            case Phase.GLOW:
                Text.text = "メインフェイズ";
                Text.gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                Text.gameObject.SetActive(false);
                phase = Phase.MAIN;
                Phasetime=false;
                break;
            case Phase.MAIN:
                if(NowTrun()==1){
                    Text.text = "エンドフェイズ";
                    Text.gameObject.SetActive(true);
                    yield return new WaitForSeconds(1);
                    Text.gameObject.SetActive(false);
                    phase = Phase.END;
                    Phasetime=false;
                }else{
                    Text.text = "アタックフェイズ";
                    Text.gameObject.SetActive(true);
                    yield return new WaitForSeconds(1);
                    Text.gameObject.SetActive(false);
                    Text.text="シグニアタックステップ";
                    Text.gameObject.SetActive(true);
                    yield return new WaitForSeconds(1);
                    Text.gameObject.SetActive(false);
                    phase = Phase.SIGNIATTACK;
                    Phasetime=false;
                }
                break;
            case Phase.SIGNIATTACK:
                Text.text="ルリグアタックステップ";
                Text.gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                Text.gameObject.SetActive(false);
                phase = Phase.LRIGATTACK;
                Phasetime=false;
                break;
            case Phase.LRIGATTACK:
                Text.text = "エンドフェイズ";
                Text.gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                Text.gameObject.SetActive(false);
                phase = Phase.END;
                Phasetime=false;
                break;
            case Phase.END:
                Player tmpPlayer = currentPlayer;
                currentPlayer = waitPlayer;
                waitPlayer = tmpPlayer;
                Text.text = "アップフェイズ";
                Text.gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                Text.gameObject.SetActive(false);
                phase = Phase.UP;
                TrunCount();
                Phasetime=false;
                break;
        }
        eventSystem.enabled=true;
    }
}
