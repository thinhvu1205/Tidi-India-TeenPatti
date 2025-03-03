using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class TeenPattiView : GameView
{
    public static TeenPattiView instance = null;
    private ShowNumbOfCard _CardNumberSNOC;
    
    private List<ShowGroupBanner> _GroupBannerSGB = new();
     private List<List<int>> _CodeGroups = new();
    protected override void Awake()
    {
        instance = this;
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }

    //TODO: Override HandleGame
     public override void setGameInfo(int m, int id = 0, int maxBet = 0)
    {
        base.setGameInfo(m, id, maxBet);
        int countMode = 0, gameId = Globals.Config.curGameId;
    }
    protected override void updatePositionPlayerView()
    {
        thisPlayer.playerView.setPosThanhBarThisPlayer();
        int? currentDimondIndex = null;
        string currentDimonHolder = null;
        if (currentDimondIndex != null)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i]._indexDynamic == currentDimondIndex)
                {
                    currentDimonHolder = players[i].displayName;
                }
            }
        }
        base.updatePositionPlayerView();
        for (int i = 0; i < players.Count; i++)
        {
            int index = getDynamicIndex(getIndexOf(players[i]));
            players[i].playerView.transform.localPosition = listPosView[index];
            players[i]._indexDynamic = index;
        }
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].displayName == currentDimonHolder)
            {
                // m_DiamondIcons[players[i]._indexDynamic].SetActive(true);
            }
        }
    }
    public override void handleCTable(string data)
    {
        base.handleCTable(data);
    }
    public override void handleCCTable(JObject data)
    {
        base.handleCCTable(data);
    }
    public override void handleSTable(string objData)
    {
        base.handleSTable(objData);
        JObject data = JObject.Parse(objData);
        stateGame = Globals.STATE_GAME.VIEWING;
        JObject hitPot = (JObject)data["HitPot"];
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.START_GAME);
        JArray ArrP = getJArray(data, "ArrP");
        for (int i = 0; i < ArrP.Count; i++)
        {
            JObject dataPlayer = (JObject)ArrP[i];
            Player player = getPlayerWithID(getInt(dataPlayer, "id"));
            JArray Arr = getJArray(dataPlayer, "Arr");

            if (player == thisPlayer)
            {
                for (int j = 0; j < Arr.Count; j++)
                {
                    JObject dataChip = (JObject)Arr[j];
                    // int numberBet = convertBetToInteger(getListInt(dataChip, "N"), getInt(dataChip, "T"));
                    // totalBet += getInt(dataChip, "M");
                    // boxBet.onBet(numberBet, getInt(dataChip, "M"));
                    // boxBet.creatDataBet();
                    // effectDatCuocChip(player, getInt(dataChip, "M"), numberBet);
                }
            }
            else
            {
                for (int j = 0; j < Arr.Count; j++)
                {
                    JObject dataChip = (JObject)Arr[j];
                    // int numberBet = convertBetToInteger(getListInt(dataChip, "N"), getInt(dataChip, "T"));
                    // effectDatCuocChip(player, getInt(dataChip, "M"), numberBet);
                }
            }
        }
    }
    public override void handleLTable(JObject data)
    {
        string name = getString(data, "Name");
        Player player = getPlayer(name);
        if (player == null)
            return;
        base.handleLTable(data);
        cleanTable();
        clearNumbOfCard();
    }

    public override void handleVTable(string objData)
    {
        base.handleVTable(objData);
        JObject data = JObject.Parse(objData);
        JObject hitPot = (JObject)data["HitPot"];

        if ((int)data["Finish"] != 1 || data["Finish"] == null)
        {
            viewTable(data);
            JArray ArrP = getJArray(data, "ArrP");
            if ((int)data["T"] > 0)
            {
                for (int i = 0; i < ArrP.Count; i++)
                {
                    if ((bool)ArrP[i]["isStart"])
                    {
                        setPlayerTurn((int)ArrP[i]["id"], (int)data["T"]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < ArrP.Count; i++)
                {
                    if ((bool)ArrP[i]["isStart"])
                    {
                        setPlayerTurn((int)ArrP[i]["id"], 0);
                    }
                }
            }
            // viewCardOnDeck(ArrP);
        }
    }

    public async override void handleRJTable(string objData)
    {
        base.handleRJTable(objData);
        JObject data = JObject.Parse(objData);
        cleanTable(false);
        JObject hitPot = (JObject)data["HitPot"];
        // potView((int)hitPot["AG"], (int)hitPot["rounds"], false);
        // var dimon = m_HitPotSG.transform.Find("dimond");
        // if ((int)hitPot["pid"] != 0)
        // {
        //     var player = getPlayerWithID((int)hitPot["pid"]);
        //     showDimond(player._indexDynamic);
        //     if (dimon != null)
        //         dimon.gameObject.SetActive(true);
        // }
        if ((int)data["T"] > 0) setPlayerTurn((int)data["CN"], (int)data["T"], (bool)data["isDaBoc"]);
        else setPlayerTurn((int)data["CN"], 0);
        viewTable(data);
        JArray ArrP = getJArray(data, "ArrP");
        // viewCardOnDeck(ArrP);
        updateGroupBanner();
        // _IsDraw = (bool)data["isDaBoc"];
        // if (!_IsDraw && _CurrentPlayerP == thisPlayer)
        // {
        //     await Task.Delay(200);
        //     showArrow();
        //     showEatHint();
        // }

        // var status = getString(data, "statusGame");
        // if (status == "WAIT_FOR_START" || status == "DECLEARING" || status == "FINISH")
        // {
        //     activeAllButton(false);
        //     hideArrow();
        // }
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.START_GAME);
    }
    public void handleTimeToStart(JObject data)
    {
        //{ "evt":"timeToStart","time":5}
        stateGame = Globals.STATE_GAME.WAITING;
        cleanTable();
    }
    
    void updateGroupBanner()
    {
        foreach (ShowGroupBanner sgb in _GroupBannerSGB)
        {
            if (sgb == null) continue;
            if (sgb.gameObject != null) Destroy(sgb.gameObject);
        }
        _GroupBannerSGB.Clear();
        for (int i = 0; i < _CodeGroups.Count; i++) showGroupBanner(i);
    }
    public void showGroupBanner(int groupId)
    {
        // ShowGroupBanner banner = Instantiate(m_GroupBannerSGB, m_Cards.transform);
        // banner.transform.SetAsLastSibling();
        // _GroupBannerSGB.Add(banner);
        // setInfoGroupBanner(groupId, banner);
    }
    async void viewTable(JObject data)
    {
        JArray ArrP = getJArray(data, "ArrP");
        foreach (JObject playerData in ArrP)
        {
            if ((int)playerData["id"] == thisPlayer.id)
            {
                JArray jarr = (JArray)playerData["arr"];
                List<List<int>> arr = jarr.ToObject<List<List<int>>>();
                // viewHand(arr);
            }
        }
        // initCardStack((int)data["cardNoc"], false);
        List<int> ArrD = getListInt(data, "ArrD");
        // for (int i = 0; i < ArrD.Count; i++)
        // {
        //     Card cardTemp = spawnCard();
        //     cardTemp.setTextureWithCode(ArrD[i]);
        //     cardTemp.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
        //     _DumpedCardCs.Add(cardTemp);
        //     var pos = getDumpedCardsPosition(i);
        //     cardTemp.transform.localPosition = pos;
        //     if (i == ArrD.Count - 1)
        //     {
        //         cardTemp.turnBorderBlue(true);
        //     }
        //     else
        //     {
        //         cardTemp.setDark(true);
        //     }
        //     if (i == ArrD.Count - 1)
        //         setCardTouch(cardTemp);
        // }
        List<int> otherPlayerCard = new();
        for (int i = 0; i < players.Count - 1; i++)
        {
            otherPlayerCard.Add(12);
        }
        for (var i = 0; i < ArrP.Count; i++)
        {
            JObject playerData = (JObject)ArrP[i];
            var player = getPlayerWithID((int)playerData["id"]);
            if (player != thisPlayer)
            {
                int count = 0;
                JArray jarr = (JArray)playerData["arr"];
                List<List<int>> arr = new();
                for (var j = 0; j < jarr.Count; j++)
                {
                    JArray arrItem = (JArray)jarr[j];
                    // count += arrItem.Count();
                }
                otherPlayerCard[player._indexDynamic - 1] = count;
            }
        }
        // showNumbOfCard((int)data["cardNoc"], otherPlayerCard);
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.START_GAME);
        await Task.Delay(600);
    }

    void setPlayerTurn(int playerID, float time, bool isDaBoc = false)
    {
        // hideArrow();
        // Player player = getPlayerWithID(playerID);
        // foreach (Player p in players)
        // {
        //     p.setTurn(false);
        // }
        // if (player == null) return;
        // if (player == thisPlayer)
        // {
        //     if (thisPlayer.vectorCard.Count > 12 || isDaBoc)
        //     {
        //         _IsDraw = true;
        //         hideArrow();
        //     }
        //     else if (!isDaBoc)
        //     {
        //         StartCoroutine(showArrowWithDelay(0.7f));
        //     }
        //     if (checkIfCanFight())
        //     {
        //         enableFight(true);
        //     }
        // }
        // if (player != null)
        // {
        //     _CurrentPlayerP = player;
        //     player.setTurn(true, time);
        //     highLightPlayerArea(player._indexDynamic);
        // }
        // else
        // {
        //     foreach (Player p in players)
        //     {
        //         p.setTurn(false);
        //     }
        //     highLightPlayerArea(3);
        // }
    }

    public void cleanTable(bool updateBanner = false)
    {
        checkAutoExit();
        if (Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS11)
        {
            Transform other = transform.Find("playerScoreOther");
            if (other != null) other.GetChild(0).gameObject.SetActive(false);
            Transform title = transform.Find("cardOnHand");
            if (title != null) title.gameObject.SetActive(false);
        }
    }
    void clearNumbOfCard()
    {
        if (_CardNumberSNOC == null)
        {
            return;
        }
        else
        {
            try
            {
                Destroy(_CardNumberSNOC.gameObject);
            }
            catch (System.Exception err)
            {
                Debug.Log(err);
            }
            _CardNumberSNOC = null;
        }
    }

    //TODO:Event Game
    public void HandleStart(){

    }
    public void HandleStartBet(string objData){

    }
    public void HandleBet(string objData){

    }
    public void HandleFinish(string objData){

    }
}
