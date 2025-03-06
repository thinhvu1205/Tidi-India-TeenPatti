using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnBetAndarBahar : MonoBehaviour
{
    [SerializeField] public Button btn_Rebet, btn_Double;

    [SerializeField] List<Button> listBtnBetChip;

    [SerializeField] public GameObject ske_rebet, ske_double;

    [SerializeField] public TextMeshProUGUI label_totalBet;

    [SerializeField] public TextMeshProUGUI label_clear;

    [SerializeField] TMP_FontAsset fontChip;

    [SerializeField] AndarBaharView _andarBaharView;
    public List<int> listValue = new List<int>();

    private void Awake()
    {
        SetSprChipBet();
    }

    private void OnEnable()
    {
        StartCoroutine(ShowButtons());
        return;

        IEnumerator ShowButtons()
        {
            int valueBet = 0;
            for (int i = 0; i < _andarBaharView.boxBet.dataBet.Count; i++)
            {
                JObject data = (JObject)_andarBaharView.boxBet.dataBet[i];
                valueBet += (int)data["value"];
            }

            bool enableRebet = _andarBaharView.matchCount != 0 &&
                               !(_andarBaharView.thisPlayer.ag < valueBet || _andarBaharView.boxBet.dataBet.Count <= 0);
            btn_Rebet.interactable = enableRebet;
            btn_Double.interactable = false;
            CanvasGroup canvasGr = GetComponent<CanvasGroup>();
            canvasGr.alpha = 0;
            for (int i = 0; i < listBtnBetChip.Count; i++) listBtnBetChip[i].gameObject.SetActive(false);
            float fadingTime = .05f;
            canvasGr.DOComplete();
            canvasGr.DOFade(1, fadingTime);
            yield return new WaitForSecondsRealtime(fadingTime);
            foreach (Button betBtn in listBtnBetChip)
            {
                yield return new WaitForSecondsRealtime(.1f);
                betBtn.gameObject.SetActive(true);
                betBtn.interactable = true;
                betBtn.transform.DOComplete();
                betBtn.transform.DOScale(new Vector2(1.2f, 1.2f), .05f).OnComplete(() =>
                {
                    betBtn.transform.DOScale(Vector2.one, .05f);
                });
            }

            yield return new WaitForSecondsRealtime(.6f);
            onClickChip(_andarBaharView.chipDealLastMatch.ToString());
            _andarBaharView.gateBet.setStateButtonBet(false);
        }
    }

    public void onClickDouble()
    {
        Debug.Log("onclick button double!!!!");
        _andarBaharView.onClickDouble();
        btn_Double.interactable = false;
    }

    public void onClickRebet()
    {
        Debug.Log("onclick button rebet!!!!");
        _andarBaharView.onClickRebet();
        btn_Rebet.interactable = false;
    }

    public void onClickDeal()
    {
        _andarBaharView.onClickDeal();
    }

    public void onClickClear()
    {
        _andarBaharView.onClickClear();
    }

    public void onClickChip(string dataChip)
    {
        for (int i = 0; i < listBtnBetChip.Count; i++)
        {
            listBtnBetChip[i].transform.Find("border").gameObject.SetActive(false);
        }

        SoundManager.instance.soundClick();
        int data = int.Parse(dataChip);
        _andarBaharView.setValueBtnBet(data);
        setStateButtonChip();
    }

    public void activeDouble()
    {
        bool isDisable = _andarBaharView.thisPlayer.ag < _andarBaharView.totalBet;
        btn_Double.interactable = !isDisable;
    }

    public void setStateButtonChip()
    {
        var agClickBet = _andarBaharView.thisPlayer.ag - _andarBaharView.boxBet.totalBoxBet;

        if (agClickBet < _andarBaharView.curChipBet) _andarBaharView.curChipBet = agClickBet;

        for (int i = 0; i < listBtnBetChip.Count; i++)
        {
            if (agClickBet < listValue[i])
            {
                listBtnBetChip[i].interactable = false;
            }
            else
            {
                listBtnBetChip[i].interactable = true;
            }
        }

        var index = _andarBaharView.chipDealLastMatch - 1;
        if (index < 1)
            index = 0;
        listBtnBetChip[index].transform.Find("border").gameObject.SetActive(true);
    }

    public void setStateButtonOnBet()
    {
        for (int i = 0; i < listBtnBetChip.Count; i++)
        {
            if (_andarBaharView.thisPlayer.ag < listValue[i])
            {
                listBtnBetChip[i].interactable = false;
            }
            else
            {
                listBtnBetChip[i].interactable = true;
            }
        }

        int index = _andarBaharView.chipDealLastMatch - 1;
        if (index < 1)
            index = 0;
        listBtnBetChip[index].transform.Find("border").gameObject.SetActive(true);
    }

    private void SetSprChipBet()
    {
        listValue = _andarBaharView.listValue;
        for (int i = 0; i < listValue.Count; i++)
        {
            TextMeshProUGUI nodeText = listBtnBetChip[i].transform.GetComponentInChildren<TextMeshProUGUI>();
            nodeText.font = fontChip;
            nodeText.text = Globals.Config.FormatMoney2(listValue[i], true);
            nodeText.transform.localScale = new Vector2(1, 1);
            nodeText.color = Color.black;
        }
    }
}