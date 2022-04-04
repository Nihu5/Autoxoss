using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectUI : MonoBehaviour{
    int pointer = 0,lastPointer=0;
    bool selectConfirm = false;
    [SerializeField] GameManager gameManager;
    [SerializeField] Motion motion;
    [SerializeField] Text selectUItext;
    [SerializeField] Image[] image = new Image[5];
    [SerializeField] Image[] imageTarget = new Image[5];
    List<bool> selectTarget = new List<bool>(){false,false,false,false,false};
    private int maxSelectLimit,minSelectLimit;
    [SerializeField] Image button;
    List<Card> cardList=new List<Card>();
    [SerializeField]Canvas gameField;
    [SerializeField] Image zoneTarget;
    [SerializeField] Field field;
    [SerializeField] RaycastDetector raycastDetector;
    [SerializeField] Image yesNoUI;
    [SerializeField] Text yesNoUItext;
    [SerializeField] Image selectUIDisplay;
    [SerializeField] Image sideDisplay;
    [SerializeField] Text sideDisplayName;
    [SerializeField] Text sideDisplayText;
    private bool? yesOrNo=null;
    Card tmpcard=null;
    Image insbutton=null;
    public float feedValue=0.1f;
    
    public IEnumerator DisplaySelectUI(List<Card> cards,int minLimit,int maxLimit,string text){
        selectConfirm = false;
        pointer=0;
        cardList = cards;
        maxSelectLimit=maxLimit;
        minSelectLimit=minLimit;
        selectUItext.text=text;
        if(cardList.Count>=6){
            for(int i=5;i<cardList.Count;i++){
                selectTarget.Add(false);
            }
        }
        Target(cardList);
        selectUIDisplay.gameObject.gameObject.SetActive(true);
        yield return StartCoroutine(motion.DisplayFeedIn(selectUIDisplay.gameObject.transform));
    }
    
    public IEnumerator DisplayEnd(){
        selectTarget = new List<bool>(){false,false,false,false,false};
        Debug.Log(selectTarget[2]);
        
        yield return StartCoroutine(motion.DisplayFeedOut(selectUIDisplay.gameObject.transform));
        selectUIDisplay.gameObject.gameObject.SetActive(false);
    }

    public void Target(List<Card> cards) {
        Debug.Log("Target");
        for (int i = 0; i < 5; i++) {
            if (i<cards.Count) {
                //image[i].sprite = Resources.Load<Sprite>(cards[pointer + i].CardName());//cards[pointer + i].sprite;
                image[i].sprite = gameManager.deckGenerater.ImageLoad(cards[pointer + i].CardName());
                selectchange(i);
            } else {
                image[i].sprite = Resources.Load<Sprite>("Noimage");
                selectchange(i);
            }
        }
    }
    public void selectchange(int i) {
        if (selectTarget[pointer + i] == true) {
            imageTarget[i].color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
        } else {
            imageTarget[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        }
    }
    public int NowSelectCount(){
        int count=0;
        for(int i=0;i<selectTarget.Count;i++){
            if(selectTarget[i]){
                count++;
            }
        }
        return count;
    }
    public void Leftpointer() {
        if (pointer == 0) {
            return;
        }
        pointer--;
        Target(cardList);
    }
    public void Rightpointer() {
        if (pointer >= cardList.Count - 5) {
            return;
        }
        pointer++;
        Target(cardList);
    }
    public void SelectOk() {
        int count=0;
        foreach(bool Target in selectTarget){
            if(Target){
                count++;
            }
        }
        if(count>=minSelectLimit){
            selectConfirm=true;
        }
    }
    public bool SelectConfirm() {
        return selectConfirm;
    }
    public void Select0() {
        if(NowSelectCount()<=maxSelectLimit){
            if (selectTarget[pointer] == false&&NowSelectCount()<maxSelectLimit) {
                selectTarget[pointer] = true;
            } else {
                selectTarget[pointer] = false;
            }
            selectchange(0);
        }
    }
    public void Select1() {
        if(NowSelectCount()<=maxSelectLimit){
            if (selectTarget[pointer + 1] == false&&NowSelectCount()<maxSelectLimit&&cardList.Count>pointer+1) {
                selectTarget[pointer + 1] = true;
            } else {
                selectTarget[pointer + 1] = false;
            }
            selectchange(1);
        }
    }
    public void Select2() {
        if(NowSelectCount()<=maxSelectLimit){
            if (selectTarget[pointer + 2] == false&&NowSelectCount()<maxSelectLimit&&cardList.Count>pointer+2) {
                selectTarget[pointer + 2] = true;
            } else {
                selectTarget[pointer + 2] = false;
            }
            selectchange(2);
        }
    }
    public void Select3() {
        if(NowSelectCount()<=maxSelectLimit){
            if (selectTarget[pointer + 3] == false&&NowSelectCount()<maxSelectLimit&&cardList.Count>pointer+3) {
                selectTarget[pointer + 3] = true;
            } else {
                selectTarget[pointer + 3] = false;
            }
            selectchange(3);
        }
    }
    public void Select4() {
        if(NowSelectCount()<=maxSelectLimit){
            if (selectTarget[pointer + 4] == false&&NowSelectCount()<maxSelectLimit&&cardList.Count>pointer+4) {
                selectTarget[pointer + 4] = true;
            } else {
                selectTarget[pointer + 4] = false;
            }
            selectchange(4);
        }
    }
    public List<Card> SelectResult(){
        List<Card> selectCards=new List<Card>();
        for (int i = 0; i < cardList.Count; i++)
        {
            if(selectTarget[i]==true){
                selectCards.Add(cardList[i]);
            }
        }
        return selectCards;
    }
    public void zoneUIactive(Card card){
        tmpcard =card;
        zoneTarget.gameObject.SetActive(true);
    }
    public void CenterSelect(){
        raycastDetector.ButtonDestroy();
        if(tmpcard!=null){
            if(!field.AddfromHandtoZone(tmpcard,GameManager.Zone.CENTERZONE)){
                return;
            }
            tmpcard=null;
        }
        zoneTarget.gameObject.SetActive(false);
    }
    public void LeftSelect(){
        raycastDetector.ButtonDestroy();
        if(tmpcard!=null){
            if(!field.AddfromHandtoZone(tmpcard,GameManager.Zone.LEFTZONE)){
                return;
            }
            tmpcard=null;
        }
        zoneTarget.gameObject.SetActive(false);
    }
    public void RightSelect(){
        raycastDetector.ButtonDestroy();
        if(tmpcard!=null){
            if(!field.AddfromHandtoZone(tmpcard,GameManager.Zone.RIGHTZONE)){
                return;
            }
            tmpcard=null;
        }
        zoneTarget.gameObject.SetActive(false);
    }
    public IEnumerator YesNoUIOnDisplay(string text){
        if(yesNoUI.gameObject.activeSelf==false){
            yesNoUItext.text=text;
            yesOrNo=null;
            yesNoUI.gameObject.SetActive(true);
            yield return StartCoroutine(motion.DisplayFeedIn(yesNoUI.gameObject.transform));
        }else{
            yield return StartCoroutine(motion.DisplayFeedOut(yesNoUI.gameObject.transform));
            yesNoUI.gameObject.SetActive(false);
        }
    }
    public void YesButton(){
        yesOrNo=true;
    }
    public void NoButton(){
        yesOrNo=false;
    }
    public bool? yesOrNoResult(){
        return yesOrNo;
    }
    public void sideImageChange(int? id, string name){
        Sprite sprite = gameManager.deckGenerater.ImageLoad(name);
        sideDisplay.sprite = sprite;
        sideDisplayName.text=gameManager.deckGenerater.cardName(id);
        sideDisplayText.text=gameManager.deckGenerater.cardText(id);
    }
    public void selectUISideImgaeChange(int i){
        if(i<cardList.Count){
            sideImageChange(cardList[pointer+i].cardId(),cardList[pointer+i].CardName());
        }
    }
}
