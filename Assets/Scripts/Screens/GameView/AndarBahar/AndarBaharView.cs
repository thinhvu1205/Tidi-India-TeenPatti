using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Globals;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AndarBaharView : GameView
{
    [SerializeField] public GameObject dealer;
    [SerializeField] GameObject startTime;
    [SerializeField] private TextMeshProUGUI mTipChipsTMP, mTipThanksTMP;
    [SerializeField] private SkeletonGraphic mBeginGameSg;
    [SerializeField] private SkeletonGraphic mDealerSg;
    [SerializeField] public SkeletonGraphic aniClock;
    [SerializeField] private PlayerAndarBahar mDealerPlayerAndarBahar;
    [SerializeField] private Transform centerCard;
    [SerializeField] private Transform andarCard;
    [SerializeField] private Transform baharCard;
    [SerializeField] public GateBetAndarBahar gateBet;
    [SerializeField] public BoxBetAndarBahar boxBet;
    [SerializeField] public BtnBetAndarBahar buttonBet;
    [SerializeField] private Transform layerChip;
    [SerializeField] private GameObject chipBetPrefab;
    [HideInInspector] public List<int> listValue = new List<int>();
    [HideInInspector] public int matchCount = 0, chipDealLastMatch = 0;
    [HideInInspector] public long curChipBet = 0, totalBet = 0, currentTempBet = 0, limitTotalBet = 0;
    private List<ChipBetAndarBahar> listChipInTable = new List<ChipBetAndarBahar>();
    public List<GameObject> chipBetPool = new List<GameObject>();

    private bool isBet = false;

    protected override void Start()
    {
        base.Start();
        // HandleStartGame();
        // HandleStartBet();
        // HandleIsDealer();
        // HandleShowCard();
        RunFakeData();
    }
    
    private async Task RunFakeData()
    {
       await FakeDataFlow();
    }

    private async Task FakeDataFlow()
    {
        await SendFakeEvent("ctable", JObject.Parse(@"
{
    ""N"": ""LetisplaySicboBasicinstinct"",
    ""M"": 2000,
    ""ArrP"": [
        {
            ""id"": 8414398,
            ""N"": ""huyvu1"",
            ""Url"": """",
            ""AG"": 14995920,
            ""LQ"": 0,
            ""VIP"": 3,
            ""isStart"": true,
            ""IK"": 0,
            ""sIP"": ""10.148.0.2"",
            ""Av"": 10,
            ""FId"": 0,
            ""D"": 0,
            ""level"": 0,
            ""displayName"": ""huyvu1"",
            ""keyObjectInGame"": 0
        }
    ],
    ""Id"": 27678,
    ""T"": 0,
    ""V"": 0,
    ""AG"": 10000,
    ""S"": 20
}"), 1);
        await SendFakeEvent("start", "5", 3);
        
        await SendFakeEvent("jtable", JObject.Parse(@"

{
    ""id"": 7158824,
    ""N"": ""RheaMay"",
    ""Url"": """",
    ""AG"": 96000,
    ""LQ"": 0,
    ""VIP"": 3,
    ""isStart"": true,
    ""IK"": 0,
    ""Av"": 8,
    ""FId"": 0,
    ""D"": 0,
    ""level"": 0,
    ""displayName"": ""RheaMay"",
    ""keyObjectInGame"": 0
}"), 5);
        await SendFakeEvent("startdealer", Random.Range(1, 50).ToString(), 2);

        await SendFakeEvent("lc", "16000", 2);
        
       await SendFakeEvent("bet", JObject.Parse(@"
 {
     ""N"": ""RheaMay"",
     ""Num"": [[1], [8], [0], [0], [0], [0]],
     ""M"": [4000, 2000, 2000, 2000, 2000, 2000],
     ""T"": [1, 4.5, 3.5, 5.5, 15, 25]
 }"), 4);
        await SendFakeEvent("bet", JObject.Parse(@"
{
    ""N"": ""huyvu1"",
    ""Num"": [[2], [0], [20]],
    ""M"": [4000, 4000, 4000],
    ""T"": [1, 15, 4.5 ]
}"), 11);
        await SendFakeEvent("bm", "12", 12);

        await SendFakeEvent("finish", JObject.Parse(@"{""listDice"":[1,3,6],""listNumberWin"":[{""nid"":[1],""typewin"":2}]}"), 0);
        // await SendFakeEvent("jtable", JObject.Parse(@"{""id"":7980534,""N"":""AljonVallestero"",""AG"":74000,""VIP"":2}"), 7);
        // await SendFakeEvent("jtable", JObject.Parse(@"{""id"":6961765,""N"":""Andy"",""AG"":98000,""VIP"":3}"), 8);
        // await SendFakeEvent("start", "4000", 9);
        // await SendFakeEvent("lc", "15000", 10);
        // await SendFakeEvent("bet", JObject.Parse(@"{""N"":""AljonVallestero"",""M"":[10000,2000,2000],""T"":[1,10,12]}"), 11);
        // await SendFakeEvent("bet", JObject.Parse(@"{""N"":""Andy"",""M"":[10000,2000,2000],""T"":[1,8,10]}"), 12);
        // await SendFakeEvent("bet", JObject.Parse(@"{""N"":""RheaMay"",""M"":[56000,2000,2000],""T"":[1,18,1]}"), 13);
        // await SendFakeEvent("finish", JObject.Parse(@"{""listDice"":[1,3,5],""listNumberWin"":[{""nid"":[1],""typewin"":2}]}"), 14);
    }
    
    public async Task SendFakeEvent(string evt, object data, float delay)
    {

        JObject fakeEvent = new JObject
        {
            ["evt"] = evt,
            ["data"] = data.ToString()
        };

        Debug.Log($"[{DateTime.Now}] Event: {evt}  +  {fakeEvent}");
        resolveData(fakeEvent);
        await Task.Delay(TimeSpan.FromSeconds(delay));
    }
    
    private void resolveData(JObject jData)
    {
        string evt = (string)jData["evt"];
        string data = (string)jData["data"];
        switch (evt)
        {
            case "ctable":
                Debug.Log($"[{DateTime.Now}] Event: {evt}  +  {data}");
                handleCTable((string)jData["data"]);
                break;
            case "jtable":
                handleJTable((string)jData["data"]);
                break;
        }
        ProcessResponseData(jData);
    }

    public override void handleSTable(string objData)
    {
        base.handleSTable(objData);
        stateGame = STATE_GAME.VIEWING;
        JObject data = JObject.Parse(objData);
        if (thisPlayer != null && thisPlayer.playerView != null) thisPlayer.playerView.setPosThanhBarThisPlayer();

        agTable = getInt(data, "M");
        listValue = new List<int> { agTable, agTable * 5, agTable * 10, agTable * 50, agTable * 100 };
        //--------------- CREAT CHIP BETTED --------------//
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
                    int numberBet = convertBetToInteger(getListInt(dataChip, "N"), getInt(dataChip, "T"));
                    totalBet += getInt(dataChip, "M");
                    boxBet.onBet(numberBet, getInt(dataChip, "M"));
                    boxBet.creatDataBet();
                    effectBetChip(player, getInt(dataChip, "M"), numberBet);
                }
            }
            else
            {
                for (int j = 0; j < Arr.Count; j++)
                {
                    JObject dataChip = (JObject)Arr[j];
                    int numberBet = convertBetToInteger(getListInt(dataChip, "N"), getInt(dataChip, "T"));
                    effectBetChip(player, getInt(dataChip, "M"), numberBet);
                }
            }
        }
        //--------------- END CREAT CHIP BETTED --------------//

        //--------------- STABLE WHEN STARTED -----------//
        if (getInt(data, "T") > 1)
        {
            TweenCallback effectShowButtonBet = () =>
            {
                buttonBet.transform.SetSiblingIndex(transform.childCount - 2);
                buttonBet.gameObject.SetActive(true);
                for (int i = 0; i < gateBet.listButtonBet.Count; i++)
                {
                    gateBet.listButtonBet[i].interactable = true;
                }
            };

            TweenCallback effectShowClock = () =>
            {
                ShowClock(true, Mathf.FloorToInt(getInt(data, "T") / 1000) - 1);
                isBet = true;
                // aniXocDia.gameObject.SetActive(true);
            };
            DOTween.Sequence()
                .AppendCallback(effectShowButtonBet)
                .AppendInterval(1.0f)
                .AppendCallback(effectShowClock);
        }
        else
        {
            buttonBet.transform.SetSiblingIndex(transform.childCount - 2);
            buttonBet.gameObject.SetActive(false);
            for (int i = 0; i < gateBet.listButtonBet.Count; i++)
            {
                gateBet.listButtonBet[i].interactable = false;
            }

            UIManager.instance.showToast(Config.getTextConfig("txt_view_table"), transform);
        }
        //------------- END ------------//
        // if (popupHistory == null)
        // {
        //     popupHistory = Instantiate(history_prefab, layerPopup).GetComponent<HistorySicbo>();
        //
        // }
        // popupHistory.gameObject.SetActive(false);
        // JArray his = (JArray)data["History"];
        // List<List<int>> History = his.ToObject<List<List<int>>>();
        // //History = fakDataHIstory;
        // popupHistory.handleDataHistory(History);
        // //------------- Instance Rule ----------------//
        // if (popupRule == null)
        // {
        //     popupRule = Instantiate(prefab_rule, layerPopup).GetComponent<RuleSicbo>();
        //
        // }       
        //------------- CREAT HISTORY ------------//
        // popupRule.gameObject.SetActive(false);
        // nodeHistory.gameObject.SetActive(false);
        // if (History.Count > 0)
        // {
        //     List<int> dataHis = History.Last();
        //     History.RemoveAt(History.Count - 1);
        //     nodeHistory.gameObject.SetActive(true);
        //     int sc = 0;
        //     dataHis.ForEach(dice => { sc += dice; });
        //     nodeHistory.transform.Find("lb_number").GetComponent<TextMeshProUGUI>().text = sc + "";
        //     int index = 0;
        //     for (int i = 0; i < listXucXacHistory.Count; i++)
        //     {
        //         Image itemHistory = listXucXacHistory[i];
        //         if (dataHis.Count < i) return;
        //
        //         Transform itHistTf = itemHistory.transform;
        //         DOTween.Sequence().AppendInterval(0.05f * i).Append(itHistTf.DOLocalMove(new Vector2(itHistTf.localPosition.x - 40, -40), 0.2f).SetEase(Ease.InCubic))
        //             .AppendCallback(() =>
        //             {
        //                 itHistTf.localPosition = new Vector2(itHistTf.localPosition.x, 60);
        //                 itemHistory.sprite = listSpriteFrameXucXacHis[dataHis[index]];
        //                 index++;
        //             })
        //             .Append(itHistTf.DOLocalMove(new Vector2(itHistTf.localPosition.x, 0), 0.1f).SetEase(Ease.InCubic));
        //     }
        // }
        //------------- END CREAT ---------------//
    }

    public override void handleCTable(string objData)
    {
        base.handleCTable(objData);
        stateGame = STATE_GAME.WAITING;
        JObject data = JObject.Parse(objData);
        thisPlayer.playerView.setPosThanhBarThisPlayer();
        agTable = getInt(data, "M");
        listValue.AddRange(new List<int> { agTable, agTable * 5, agTable * 10, agTable * 50, agTable * 100 });
    }

    public void ProcessResponseData(JObject jData)
    {
        switch ((string)jData["evt"])
        {
            case "start":
                HandleStartGame(jData);
                break;
            case "startdealer":
                HandleIsDealer(jData);
                break;
            case "lc":
                HandleStartBet(jData);
                break;
            case "bm":
                HandleShowCard(jData);
                break;
            case "bet":
                HandleBet((string)jData["data"]);
                break;
            case "timeout":
                HandleAnyoneTimeOut(jData);
                break;
            case "finish":
                HandleFinishGame((string)jData["data"]);
                break;
            case "tip":
                HandlerTip(jData);
                break;
        }
    }

    private void HandleStartGame(JObject data = null)
    {
        int timeStart = 2;
        if (data != null)
        {
            timeStart = (int)data["data"];
        }

        startTime.SetActive(true);
        TextMeshProUGUI timeTMP = startTime.transform.Find("Time").GetComponent<TextMeshProUGUI>();
        timeTMP.text = timeStart.ToString();
        DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
        {
            timeStart--;
            checkAutoExit();
            timeTMP.text = timeStart + "";
        }).SetLoops(timeStart).OnComplete(() =>
        {
            startTime.SetActive(false);
            CallAsyncFunction(
                Awaitable.WaitForSecondsAsync(mBeginGameSg.SkeletonData.FindAnimation(ShowAnimOnBegin(true)).Duration));
        });

        playSound(SOUND_GAME.START_GAME);
        stateGame = STATE_GAME.PLAYING;
    }

    public async void HandleIsDealer(JObject data = null)
    {
        int code = 15;
        // int pid = 555;
        if (data != null)
        {
            code = (int)data["data"];
            // pid = (int)data["pid"];
        }

        RevealCard(code, centerCard, true, new Vector2(-1, 110), new Vector2(0, 0));
        await Task.Delay(1400);
        // setDealerAfterDelay(pid);
    }

    public void HandleStartBet(JObject jdata = null)
    {
        string objData = null;
        if (jdata != null)
        {
            objData = (string)jdata["data"];
        }

        int data = int.Parse(objData ?? "10000");

        DOTween.Sequence()
            .AppendCallback(EffectShowButtonBet)
            .AppendInterval(1.0f)
            .AppendCallback(EffectShowClock)
            .AppendInterval(0.5f)
            .AppendCallback(ClickButtonBet);
        return;

        void EffectShowButtonBet()
        {
            buttonBet.transform.SetSiblingIndex(transform.childCount - 2);
            buttonBet.gameObject.SetActive(true);
        }

        void ClickButtonBet()
        {
            for (int i = 0; i < gateBet.listButtonBet.Count; i++)
            {
                gateBet.listButtonBet[i].interactable = true;
            }
        }

        void EffectShowClock()
        {
            ShowClock(true, Mathf.FloorToInt(data / 1000) - 1);
            isBet = true;
        }
    }

    public void HandleBet(string objData)
    {
        playSound(SOUND_GAME.BET);
        JObject data = JObject.Parse(objData);
        Player player = getPlayer(getString(data, "N"), true);
        JArray NumArr = getJArray(data, "Num");
        List<List<int>> Num = NumArr.ToObject<List<List<int>>>();
        List<float> T = getListFLoat(data, "T");
        List<int> M = getListInt(data, "M");
        for (int i = 0; i < Num.Count; i++)
        {
            int numberBet = convertBetToInteger(Num[i], T[i]);
            player.ag -= M[i];
            player.setAg();
            if (player == thisPlayer)
            {
                stateGame = STATE_GAME.PLAYING;
                totalBet += M[i];
                boxBet.onBet(numberBet, M[i]);
                boxBet.creatDataBet();
                buttonBet.btn_Rebet.interactable = false;
                DOTween.Sequence()
                    .AppendInterval(0.5f)
                    .AppendCallback(() => { buttonBet.activeDouble(); });
                if (buttonBet != null)
                    buttonBet.setStateButtonOnBet();
            }

            effectBetChip(player, M[i], numberBet);
        }

        if (player == thisPlayer && player.ag < curChipBet)
        {
            curChipBet = player.ag;
        }
    }

    private void HandleShowCard(JObject data = null)
    {
        RevealMultipleCards(12);
    }

    private void HandleAnyoneTimeOut(JObject data)
    {
    }

    private void HandleFinishGame(string objData)
    {
        //stateGame = STATE_GAME.PLAYING;
        Debug.Log("handleFinish");
        JObject data = JObject.Parse(objData);
        //data = JObject.Parse("{\"evt\":\"finish\",\"data\":\"{\\\"listDice\\\":[1,1,3],\\\"listNumberWin\\\":[{\\\"N\\\":\\\"1 nut\\\",\\\"typewin\\\":6},{\\\"N\\\":\\\"3 nut\\\",\\\"typewin\\\":6},{\\\"N\\\":\\\"Lo\\\",\\\"typewin\\\":1},{\\\"N\\\":\\\"1-3\\\",\\\"typewin\\\":5},{\\\"N\\\":\\\"1-2-3\\\",\\\"typewin\\\":7},{\\\"N\\\":\\\"LO 1\\\",\\\"typewin\\\":2},{\\\"N\\\":\\\"LO 3\\\",\\\"typewin\\\":3}],\\\"listUser\\\":[{\\\"pid\\\":8240,\\\"ag\\\":99153,\\\"vip\\\":3,\\\"listNumWin\\\":[{\\\"M\\\":100,\\\"N\\\":\\\"3 nut\\\",\\\"T\\\":6,\\\"W\\\":100},{\\\"M\\\":300,\\\"N\\\":\\\"LO 3\\\",\\\"T\\\":3,\\\"W\\\":300},{\\\"M\\\":100,\\\"N\\\":\\\"Lo\\\",\\\"T\\\":1,\\\"W\\\":100}]}],\\\"History\\\":[[2,5,6],[1,5,6],[2,2,6],[1,5,5],[1,1,3]]}\"}");
        // dataFinish = data;
        // JArray his = (JArray)data["History"];
        // List<List<int>> History = his.ToObject<List<List<int>>>();
        //History = fakDataHIstory;
        // popupHistory.handleDataHistory(History);
        // List<int> dataHistory = History.Last();
        // History.RemoveAt(History.Count - 1);
        buttonBet.gameObject.SetActive(false);
        boxBet.creatDataBet();
        isBet = false;
        gateBet.resetOCuoc();
        gateBet.setStateButtonBet(true);
        onClickClear();
        for (int i = 0; i < gateBet.listButtonBet.Count; i++)
        {
            gateBet.listButtonBet[i].interactable = false;
        }
        //----------- START EFFECT FINISH -----------//
        
    }

    private void effectBetChip(Player player, int valueBet, int numberBet)
    {
        Vector2 pos = player.playerView.transform.localPosition;
        //---------  Gen vi trí và tổng cược mỗi ô bet ----------//
        Vector2 posGate = gateBet.getPositionGateBet(numberBet);
        int totalBetPerGate = gateBet.setValueGateBet(numberBet, valueBet);

        TweenCallback effectCreatChipTable = () => { gateBet.setValueChipForGate(numberBet, valueBet); };
        TweenCallback effectBet = () =>
        {
            ChipBetAndarBahar chipBet = createChip(numberBet, valueBet, pos);
            chipBet.pid = player.id;
            chipBet.chipMoveTo(posGate, false, effectCreatChipTable);
            listChipInTable.Add(chipBet);
            getPlayerView(player).listChipBetPl.Add(chipBet);
        };

        DOTween.Sequence()
            .AppendCallback(effectBet);
    }

    private PlayerAndarBahar getPlayerView(Player player)
    {
        if (player != null)
        {
            return (PlayerAndarBahar)player.playerView;
        }

        return null;
    }

    public ChipBetAndarBahar createChip(int numberBet, int valueChip, Vector2 posInit)
    {
        if (chipBetPool.Count == 0)
        {
            GameObject go = Instantiate(chipBetPrefab, layerChip);
            chipBetPool.Add(go);
        }

        ChipBetAndarBahar chipBet = chipBetPool[0].GetComponent<ChipBetAndarBahar>();
        chipBet.chipDeal = 0;
        chipBetPool.RemoveAt(0);
        chipBet.transform.SetSiblingIndex(transform.childCount - 2);
        chipBet.transform.localPosition = posInit;
        chipBet.setChip(valueChip, numberBet);
        chipBet.gameObject.SetActive(true);

        return chipBet;
    }

    private string convertArrayToStr(List<int> list)
    {
        string str = "";

        for (int i = 0; i < list.Count; i++)
        {
            if (i != list.Count - 1)
            {
                str += list[i] + ",";
            }
            else
            {
                str += list[i].ToString();
            }
        }

        return str;
    }

    protected virtual int convertBetToInteger(List<int> listN, float T)
    {
        string N = convertArrayToStr(listN);
        int index = 0;
        switch (T)
        {
            case 1:
                switch (N)
                {
                    case "1":
                        index = 9;
                        break;
                    case "2":
                        index = 10;
                        break;
                }
                break;
            case 4.5f:
                switch (N)
                {
                    case "8":
                        index = 2;
                        break;
                    case "20":
                        index = 4;
                        break;
                }
                break;
            case 3.5f:
                index = 1;
                break;
            case 5.5f:
                index = 3;
                break;
            case 15:
                index = 5;
                break;
            case 25:
                index = 6;
                break;
            case 50:
                index = 7;
                break;
            case 120:
                index = 8;
                break;
        }

        return index;
    }

    private JObject convertIntegerToBet(int data)
    {
        //Logging.Log("converIntegerToBet HIlo1:" + data);

        List<List<int>> N = new List<List<int>>();
        List<int> T = new List<int>();
        switch (data)
        {
            case 1:
                N.Add(new List<int> { 1 });
                T.Add(1);
                break;
            case 2:

                N.Add(new List<int> { 2 });
                T.Add(1);
                break;
            case 3:
                N.Add(new List<int> { 1 });
                T.Add(2);
                break;
            case 4:
                N.Add(new List<int> { 2 });
                T.Add(2);
                break;
            case 5:
                N.Add(new List<int> { 3 });
                T.Add(2);
                break;
            case 6:
                N.Add(new List<int> { 4 });
                T.Add(2);
                break;
            case 7:
                N.Add(new List<int> { 5 });
                T.Add(2);
                break;
            case 8:
                N.Add(new List<int> { 6 });
                T.Add(2);
                break;
            case 9:
                N.Add(new List<int> { 10 });
                T.Add(6);
                break;
            case 10:
                N.Add(new List<int> { 11 });
                T.Add(6);
                break;
            case 11:
                N.Add(new List<int> { 9 });
                T.Add(7);
                break;
            case 12:
                N.Add(new List<int> { 12 });
                T.Add(7);
                break;
            case 13:
                N.Add(new List<int> { 8 });
                T.Add(8);
                break;
            case 14:
                N.Add(new List<int> { 13 });
                T.Add(8);
                break;
            case 15:
                N.Add(new List<int> { 1, 1 });
                T.Add(10);
                break;
            case 16:
                N.Add(new List<int> { 2, 2 });
                T.Add(10);
                break;
            case 17:
                N.Add(new List<int> { 3, 3 });
                T.Add(10);
                break;
            case 18:
                N.Add(new List<int> { 4, 4 });
                T.Add(10);
                break;
            case 19:
                N.Add(new List<int> { 5, 5 });
                T.Add(10);
                break;
            case 20:
                N.Add(new List<int> { 6, 6 });
                T.Add(10);
                break;
            case 21:
                N.Add(new List<int> { 7 });
                T.Add(12);
                break;
            case 22:
                N.Add(new List<int> { 14 });
                T.Add(12);
                break;
            case 23:
                N.Add(new List<int> { 6 });
                T.Add(18);
                break;
            case 24:
                N.Add(new List<int> { 15 });
                T.Add(18);
                break;
            case 25:
                N.Add(new List<int> { 5 });
                T.Add(30);
                break;
            case 26:
                N.Add(new List<int> { 16 });
                T.Add(30);
                break;
            case 27:
                N.Add(new List<int> { 30 });
                T.Add(30);
                break;
            case 28:
                N.Add(new List<int> { 4 });
                T.Add(60);
                break;
            case 29:
                N.Add(new List<int> { 17 });
                T.Add(60);
                break;
        }

        JObject objData = new JObject();
        objData["numberBet"] = JArray.FromObject(N);
        objData["typeBet"] = JArray.FromObject(T);
        return objData;
    }

    public void sendBetAndarBahar(string data)
    {
        bool canBet = thisPlayer.ag > boxBet.totalBoxBet;
        if (!isBet || !canBet) return;
        playSound(SOUND_GAME.BET);

        if (limitTotalBet + curChipBet > agTable * 100)
        {
            string msg = Config.getTextConfig("txt_max_bet");
            UIManager.instance.showToast(msg);
            return;
        }
        limitTotalBet += curChipBet;
        currentTempBet += curChipBet;
        int index = int.Parse(data) - 1;
        boxBet.onClickBet(index, curChipBet);

        if (buttonBet != null)
            buttonBet.setStateButtonChip();
    }
    
    public void onClickRebet()
    {
        SoundManager.instance.soundClick();
        List<List<int>> arrNumBerBet = new List<List<int>>();
        List<int> arrTypeBet = new List<int>();
        List<int> arrValue = new List<int>();
        JArray arrBet = boxBet.dataBet;
        //Debug.Log("boxBet:" + arrBet.Count);
        for (int i = 0; i < arrBet.Count; i++)
        {
            JObject dataBet = (JObject)arrBet[i];
            JObject objDataChip = convertIntegerToBet(getInt(dataBet, "numberBet"));
            //Debug.Log("objDataChip=" + objDataChip.ToString());
            List<List<int>> listNumberBet = objDataChip["numberBet"].ToObject<List<List<int>>>();
            arrNumBerBet.Add(listNumberBet[0]);
            arrValue.Add((int)dataBet["value"]);
            arrTypeBet.Add((int)objDataChip["typeBet"][0]);
        }

        SocketSend.sendBetSicbo(arrNumBerBet, arrValue, arrTypeBet);
    }

    public void onClickDouble()
    {
        SoundManager.instance.soundClick();
        List<List<int>> arrNumBerBet = new List<List<int>>();
        List<int> arrTypeBet = new List<int>();
        List<int> arrValue = new List<int>();
        JArray arrBet = boxBet.dataBet;
        for (int i = 0; i < arrBet.Count; i++)
        {
            JObject dataBet = (JObject)arrBet[i];
            JObject objDataChip = convertIntegerToBet(getInt(dataBet, "numberBet"));
            Debug.Log("objDataChip=" + objDataChip.ToString());
            List<List<int>> listNumberBet = objDataChip["numberBet"].ToObject<List<List<int>>>();
            arrNumBerBet.Add(listNumberBet[0]);
            arrValue.Add((int)dataBet["value"]);
            arrTypeBet.Add((int)objDataChip["typeBet"][0]);
        }

        //require("NetworkManager").getInstance().sendBetSicbo(arrNumBerBet, arrValue, arrTypeBet);
        SocketSend.sendBetSicbo(arrNumBerBet, arrValue, arrTypeBet);
    }

    public void onClickDeal()
    {
        SoundManager.instance.soundClick();
        List<List<int>> arrNumBerBet = new List<List<int>>();
        List<int> arrTypeBet = new List<int>();
        List<int> arrValue = new List<int>();
        JArray arrBet = boxBet.dataClickBet;
        for (int i = 0; i < arrBet.Count; i++)
        {
            JObject dataBet = (JObject)arrBet[i];
            JObject objDataChip = convertIntegerToBet(getInt(dataBet, "numberBet"));
            Debug.Log("objDataChip=" + objDataChip.ToString());
            List<List<int>> listNumberBet = objDataChip["numberBet"].ToObject<List<List<int>>>();
            arrNumBerBet.Add(listNumberBet[0]);
            arrValue.Add((int)dataBet["value"]);
            arrTypeBet.Add((int)objDataChip["typeBet"][0]);
        }

        boxBet.resetDataClickBet();

        SocketSend.sendBetSicbo(arrNumBerBet, arrValue, arrTypeBet);
    }

    public void onClickClear()
    {
        SoundManager.instance.soundClick();
        limitTotalBet -= currentTempBet;
        currentTempBet = 0;
        boxBet.onClickClearBet();
    }

    public void setValueBtnBet(int data)
    {
        //data = parseInt(data);
        for (int i = 0; i < listValue.Count; i++)
        {
            if (data - 1 >= 0)
            {
                if (thisPlayer.ag < listValue[data - 1])
                    data = data - 2;
            }
        }

        if (data < 1) data = 1;
        chipDealLastMatch = data;
        switch (data)
        {
            case 1:
                curChipBet = agTable;
                if (thisPlayer.ag < curChipBet)
                    curChipBet = thisPlayer.ag;
                break;
            case 2:
                curChipBet = agTable * 5;
                break;
            case 3:
                curChipBet = agTable * 10;
                break;
            case 4:
                curChipBet = agTable * 50;
                break;
            case 5:
                curChipBet = agTable * 100;
                break;
        }
    }

    private void ShowClock(bool isShow, int timeClock)
    {
        aniClock.gameObject.SetActive(false);
        if (isShow)
        {
            aniClock.gameObject.SetActive(true);
            int del = 1;
            TweenCallback time = () =>
            {
                //timeClock -= Config.;
                //require("GameManager").getInstance().time_out_game = 0;

                aniClock.transform.Find("lb_clock").GetComponent<TextMeshProUGUI>().text = timeClock.ToString();
                if (timeClock > 0)
                {
                    if (timeClock == 3) Config.Vibration();
                    timeClock--;
                    playSound(timeClock > 5 ? SOUND_GAME.TICKTOK : SOUND_GAME.CLOCK_HURRY);
                    aniClock.Initialize(true);
                    aniClock.AnimationState.SetAnimation(0, "animation", false);
                }
                else
                {
                    //aniClock.node.stopAction();
                    aniClock.gameObject.SetActive(false);
                    buttonBet.gameObject.SetActive(false);
                }
            };
            DOTween.Sequence()
                .AppendCallback(time)
                .AppendInterval(del)
                .SetLoops(timeClock + 1);
        }
        else
        {
            timeClock = -1;
        }
    }

    private async void RevealMultipleCards(int count)
    {
        for (int i = 0; i < count; i++) 
        {
            RevealCard(Random.Range(1, 50), andarCard, false , new Vector2(210,50), new Vector2(140 - 20*i, 0)); 
            await Task.Delay(400);
            RevealCard(Random.Range(1, 50), baharCard, false , new Vector2(-210,50), new Vector2(-140 + 20*i, 0), false); 
            await Task.Delay(600);
        }
    }
    
    private void RevealCard(int code, Transform parent, bool isFirstCard , Vector2 posStart, Vector2 posEnd, bool isAndarCard = true)
    {
        var cardTemp = getCard();
        cardTemp.transform.SetParent(parent);
        if (isAndarCard)
        {
            cardTemp.transform.SetAsFirstSibling();
        }
        else
        {
            cardTemp.transform.SetAsLastSibling();
        }
        cardTemp.transform.localPosition = posStart;
        cardTemp.transform.localRotation = Quaternion.Euler(0, 0, -2);
        cardTemp.transform.localScale = new Vector2(0.32f, 0.32f);
        if (isFirstCard)
        {
            FoldCardUpFirst(cardTemp, code, 0.4f, posEnd);
        }
        else
        {
            FoldCardUp(cardTemp, code, 0.4f, posEnd, delay : 300);
        }
    }
    
    async void FoldCardUp(Card card, int code, float time, Vector2 posEnd , float scale = 0.4f, int delay = 200)
    {
        await Task.Delay(delay);
        Vector2 scaleCard = new Vector2(0.01f, scale);
        card.transform.DOScale(scaleCard, time / 2f).OnComplete(() =>
        {
            card.setTextureWithCode(code);
            card.transform.DOScale(0.4f, time / 2f).SetEase(Ease.InOutCubic);
        });
        Quaternion newRotation = Quaternion.Euler(0, 10, 0);
        card.transform.DOLocalRotate(newRotation.eulerAngles, time / 2).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            newRotation = Quaternion.Euler(0, -10, 0);
            card.transform.localRotation = newRotation;
            card.transform.DOLocalRotate(Vector3.zero, time / 2).SetEase(Ease.InOutCubic);
        });
        card.transform.DOLocalMove(posEnd, time / 2).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
           
        });
    }

    async void FoldCardUpFirst(Card card, int code, float time, Vector2 posEnd ,float scale = 0.4f, int delay = 200)
    {
        await Task.Delay(delay);
        Vector2 scaleCard = new Vector2(0.01f, scale);
        card.transform.DOScale(scaleCard, time / 2f).OnComplete(() =>
        {
            card.setTextureWithCode(code);
            card.transform.DOScale(0.4f, time / 2f).SetEase(Ease.InOutCubic);
        });
        Quaternion newRotation = Quaternion.Euler(0, 10, 0);
        card.transform.DOLocalRotate(newRotation.eulerAngles, time / 2).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            newRotation = Quaternion.Euler(0, -10, 0);
            card.transform.localRotation = newRotation;
            card.transform.DOLocalRotate(Vector3.zero, time / 2).SetEase(Ease.InOutCubic);
        });
        card.transform.DOLocalMove(new Vector2(-60, 30), time / 2).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            card.transform.SetSiblingIndex(10);
            card.transform.DOLocalMove(posEnd, time / 2).OnComplete(() =>
            {
                // cardStackNode.transform.Find("card1").GetComponent<Card>().setTextureWithCode(code);
                // removeRateCard(card);
                // putCard(card);
            });
        });
    }

    private async void CallAsyncFunction(Awaitable function)
    {
        try
        {
            await function;
        }
        catch (Exception e)
        {
            if (e.GetType() != typeof(MissingReferenceException))
                Debug.LogError("Error on calling async function: " + e.Message);
        }
    }

    private string ShowAnimOnBegin(bool isStart = true)
    {
        string animName = isStart ? "startgame" : "startgame2";
        mBeginGameSg.gameObject.SetActive(true);
        mBeginGameSg.AnimationState.SetAnimation(0, animName, false);
        return animName;
    }
}