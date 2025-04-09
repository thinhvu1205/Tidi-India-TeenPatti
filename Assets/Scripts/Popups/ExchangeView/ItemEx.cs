using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemEx : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI txtChip, txtPrize;

    [SerializeField] private GameObject _imgBackGround;
    System.Action callback;
    public void setInfo(JObject dt, System.Action _callback, bool isActiveBg = false)
    {
        //      {
        //          "ag": 1000000,
        //  "m": 50
        //},
        _imgBackGround.SetActive(isActiveBg);
        callback = _callback;
        txtChip.text = Globals.Config.FormatNumber((int)dt["ag"]);
        txtPrize.text = Globals.Config.FormatNumber((int)dt["m"]);
    }

    public void onClickConfirm()
    {
        SoundManager.instance.soundClick();
        callback.Invoke();
    }
}
