using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;

public class DeckGenerater : MonoBehaviour{
    public GameObject cardPrefab;
    

    public void Generate(List<CardData> _cardDataList, List<CardData> _lrigCardDataList, Deck _deck, LrigDeck _lrigDeck) {
        for (int i = 0; i < _cardDataList.Count; i++) {
            GameObject cardObj = Instantiate(cardPrefab);
            cardObj.name = _cardDataList[i].name;
            Card card = cardObj.GetComponent<Card>();
            card.Load(_cardDataList[i]);
            _deck.GenerateAdd(card);
        }
        _deck.cardList=_deck.cardList.Shuffle().ToList();
        for(int i=0;i<_lrigCardDataList.Count;i++){
            GameObject cardObj = Instantiate(cardPrefab);
            cardObj.name = _lrigCardDataList[i].name;
            Card card = cardObj.GetComponent<Card>();
            card.Load(_lrigCardDataList[i]);
            _lrigDeck.Add(card);
        }
    }
    public List<string> DeckTectLoad(string name){

        List<string> txtDatas = new List<string>();

        TextAsset txt = Resources.Load (name) as TextAsset;
        StringReader reader = new StringReader (txt.text);
        while (reader.Peek () > -1) {
			// ','ごとに区切って配列へ格納
			string line = reader.ReadLine ();
			txtDatas.Add(line);
		}
        return txtDatas;
    }

    /*CardData(int _id, string _name, int? _at, int? _lv ,int? _limit,
                        CardType _cardType, LrigType? _lrigType, RaceType? _raceType, LrigType? _lrigLimit,
                        Color _color, List<Color> _cost, bool _effect, bool _lifeBurst)*/

    public List<CardData> CardDataGenerate(List<string> deckTxt){
        List<CardData> cardDatas = new List<CardData>();
        for(int i=0;i<deckTxt.Count;i++){
            int id;
            string name;
            int? at;
            int? lv;
            int? limit;
            CardData.CardType cardType;
            CardData.LrigType? lrigType;
            CardData.RaceType? raceType;
            CardData.LrigType? lriglimit;
            CardData.Color color;
            List<CardData.Color> cost=new List<CardData.Color>();
            bool effect;
            bool lifeBurst;

            SqliteDatabase sqlDB=new SqliteDatabase("CardDate.db");
            string cardQuery = string.Format("select * from CardDate where id = '{0}'", deckTxt[i]);
            DataTable dataTable = sqlDB.ExecuteQuery(cardQuery);

            foreach (DataRow dr in dataTable.Rows){
            
            id=Int32.Parse(deckTxt[i]);
            name = (string)dr["name"];
            at=(int?)dr["at"];
            lv=(int?)dr["lv"];
            limit=(int?)dr["Limit"];
            cardType=(CardData.CardType)dr["CardType"];
            lrigType=(CardData.LrigType?)dr["LrigType"];
            raceType=(CardData.RaceType?)dr["RaceType"];
            lriglimit=(CardData.LrigType?)dr["LrigLimit"];
            color=(CardData.Color)dr["Color"];
            if((int)dr["Cost"]==0){
                cost=null;
            }else{
                string costQuery=string.Format("select * from CardCost where id = '{0}'", deckTxt[i]);
                DataTable costDataTable=sqlDB.ExecuteQuery(costQuery);

                int? Cost1;
                int? CostCount1;
                int? Cost2;
                int? CostCount2;

                foreach(DataRow costDr in costDataTable.Rows){
                    
                    Cost1=(int?)costDr["Cost1"];
                    CostCount1=(int?)costDr["CostCount1"];
                    Cost2=(int?)costDr["Cost2"];
                    CostCount2=(int?)costDr["CostCount2"];
                
                    if(Cost1!=null){
                        for(int j=0;j<CostCount1;j++){
                            cost.Add((CardData.Color)Cost1);
                        }
                    }
                    if(Cost2!=null){
                        for(int j=0;j<CostCount2;j++){
                            cost.Add((CardData.Color)Cost2);
                        }
                    }

                }
            }
            if((int)dr["effect"]==0){
                effect=false;
            }else{
                effect=true;
            }
            if((int)dr["LifeBurst"]==0){
                lifeBurst=false;
            }else{
                lifeBurst=true;
            }
            CardData cardData=new CardData(id,name,at,lv,limit,cardType,lrigType,raceType,lriglimit,color,cost,effect,lifeBurst);
            cardDatas.Add(cardData);
        }

        }
        return cardDatas;
    }
    public string cardText(int? id){
        SqliteDatabase sqlDB=new SqliteDatabase("CardDate.db");
        string cardQuery = string.Format("select * from TextDate where id = '{0}'", id);
        DataTable dataTable = sqlDB.ExecuteQuery(cardQuery);

        string Text="";

        foreach (DataRow dr in dataTable.Rows){
            Text=(string)dr["Text"];
        }
        return Text;
    }
    public string cardName(int? id){
        SqliteDatabase sqlDB=new SqliteDatabase("CardDate.db");
        string cardQuery = string.Format("select * from TextDate where id = '{0}'", id);
        DataTable dataTable = sqlDB.ExecuteQuery(cardQuery);

        string Text="";

        foreach (DataRow dr in dataTable.Rows){
            Text=(string)dr["Name"];
        }
        return Text;
    }
    public Sprite ImageLoad(string name){
        string path = Application.streamingAssetsPath + "/"+name+".jpg";
        if(File.Exists(path)){
            return readByBinary(readPngFile(path));
        }else{
            return Resources.Load<Sprite>("Noimage");
        }
    }
    public byte[] readPngFile(string path) {
        using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
            BinaryReader bin = new BinaryReader(fileStream);
            byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);
            bin.Close();
            return values;
        }
    }
    public Sprite readByBinary(byte[] bytes) {
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);
        Sprite sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(0.5f,0.5f));
        return sprite;
    }
}
public static class IEnumerableExtension
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
    {
        return collection.OrderBy(i => Guid.NewGuid());
    }
}
