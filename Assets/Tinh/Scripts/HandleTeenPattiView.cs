using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HandleTeenPattiView
{
    public static void ProcessData(JObject jData)
    {
        var gameview = (TeenPattiView)UIManager.instance.gameView;
        if (gameview == null) return;
        string evt = (string)jData["evt"];
        //TODO:Check lai event
        switch (evt)
        {
            case "start":
                gameview.HandleStart();
                break;
            case "check":
                gameview.HandleStartBet((string)jData["data"]);
                break;
            case "check1":
                gameview.HandleBet((string)jData["data"]);
                break;
            case "finish":
                gameview.HandleFinish((string)jData["data"]);
                break;
            default:
                break;
        }
    }
}
