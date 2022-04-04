using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastDetector : Graphic {

    [SerializeField] Button button;
    [SerializeField] SelectUI ui;
    Button insButton;
    Text insButtonText;
    Button zoneInsButton;
    Text zoneInsButtonText;
    Card lastCard=null;
    Transform lastTransform;

    public void TouchAble(){
        this.gameObject.SetActive(true);
    }
    public void TouchEnable(){
        this.gameObject.SetActive(false);
    }
    public void ButtonDestroy(){
        Debug.Log("ボタン消すよ");
        if(insButton!=null){
            Destroy(insButton.gameObject,0f);
        }
        insButton=null;
    }
    public bool InsButton(Card card,Transform transform){
        bool insResult;
        Debug.Log("ボタン押された");
        if(insButton==null){
            insButton = Instantiate(button);
            insButtonText=insButton.transform.Find("Text").gameObject.GetComponent<Text>();
            insButton.transform.SetParent(canvas.transform,false);
            insButton.transform.position=transform.position+new Vector3(40f,0f,40f);
            insResult=true;
        }else{
            //Destroy(insButton.gameObject,0f);
            ButtonDestroy();
            //Debug.Log(insButton.transform.position-cardposition);
            if(lastTransform!=transform){
                insButton = Instantiate(button);
                insButtonText=insButton.transform.Find("Text").gameObject.GetComponent<Text>();
                insButton.transform.SetParent(canvas.transform,false);
                insButton.transform.position=transform.position+new Vector3(40f,0f,40f);
                insResult=true;   
            }else{
                insButton=null;
                insResult=false;
            }
        }
        lastCard=card;
        lastTransform=transform;
        return insResult;
    }
    public void AddText(string text){
        insButtonText.text=text;
    }
    public void AddMainEvent(){
        Debug.Log("追加前");
        insButton.onClick.AddListener (AddOnClick);
        Debug.Log("追加した");
    }
    public void AddSpellEvent(Card card){
        insButton.onClick.AddListener(card.SpelLArtsEffect);
    }
    public void AddArtsEvent(LrigDeck lrigDeck){
        insButton.onClick.AddListener(lrigDeck.AddSelectArts);
    }
    public void AddSigniAttackEvent(Card card){
        insButton.onClick.AddListener(card.SigniAttackEvent);
    }
    public void AddLrigAttackEvent(Card card){
        insButton.onClick.AddListener(card.LrigAttackEvent);
    }
    public void AddOnClick(){
        Debug.Log("押された");
        ui.zoneUIactive(lastCard);
    }
    public void SetText(string name, string text) {
        foreach(Transform child in insButton.transform) {
            // 子の要素をたどる
            if(child.name == name) {
                // 名前が一致
                foreach(Transform child2 in child.transform) {
                // 孫要素をたどる
                    if(child2.name == "Text") {
                        // テキストを見つけた
                        Text t = child2.GetComponent<Text>();
                        // テキスト変更
                        t.text = text;

                        // おしまい
                        return;
                    }
                }
            }
        }
        // 指定したオブジェクト名が見つからなかった
        Debug.LogWarning("Not found objname:"+name);
    }
}