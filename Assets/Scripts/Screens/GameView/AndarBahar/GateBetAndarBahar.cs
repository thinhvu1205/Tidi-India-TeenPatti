using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GateBetAndarBahar : MonoBehaviour
{
    [SerializeField] public List<Button> listButtonBet;

    [SerializeField] public List<TextMeshProUGUI> listLabelGateBet;

    [SerializeField] public GameObject chip_bet_prefab;

    [SerializeField] public GameObject mask;

    [SerializeField] public Sprite noImg;

    [SerializeField] private AndarBaharView _andarBaharView;
    public List<JObject> dataWin = new List<JObject>();
    public List<int> listWinResult = new List<int>();
    private int t1 = 0, t2 = 0, t3 = 0, t4 = 0, t5 = 0, t6 = 0, t7 = 0, t8 = 0, t9 = 0, t10 = 0;
 

    private List<List<ChipBetAndarBahar>> listChipBetOnGate = new List<List<ChipBetAndarBahar>>();
    private List<ChipBetAndarBahar> chipPool = new List<ChipBetAndarBahar>();
    private List<int> listValueGateWin = new List<int>();
    private List<ChipBetAndarBahar> listChipPerGate = new List<ChipBetAndarBahar>();
    private List<ChipBetAndarBahar> listChipWinPerGate = new List<ChipBetAndarBahar>();
    private List<int> listGateTotalValue = new List<int>();

    private void Awake()
    {
        resetGateBet();
        setStateButtonBet(true);
    }
    
    void Start()
    {
        listButtonBet.ForEach(btn => { btn.GetComponent<Image>().sprite = noImg; });
        for (int i = 0; i <= 15; i++)
        {
            ChipBetAndarBahar initChip = new ChipBetAndarBahar();
            listChipPerGate.Add(initChip);
            listChipWinPerGate.Add(initChip);
        }
    }

    private void OnDisable()
    {
        chipPool.ForEach(chip => { Destroy(chip.gameObject); });
    }

    public void removeChipInTable()
    {
        listChipPerGate.ForEach(objChip =>
        {
            if (objChip != null)
            {
                objChip.gameObject.SetActive(false);
            }
        });
        listChipWinPerGate.ForEach(objChip =>
        {
            if (objChip != null)
            {
                objChip.gameObject.SetActive(false);
            }
        });
    }

    public void resetOCuoc()
    {
        for (int i = 0; i < listButtonBet.Count; i++)
        {
            Button objButton = listButtonBet[i];
            Sprite spr = objButton.GetComponent<Image>().sprite;
            GameObject btnPress = objButton.transform.Find("press").gameObject;
            if (btnPress != null)
            {
                //btnPress.GetComponent<Image>().sprite = spr;
                btnPress.SetActive(true);
                btnPress.GetComponent<Image>().enabled = false;
            }
        }
    }
    
    public void setStateButtonBet(bool isActive = false)
    {
        mask.SetActive(isActive);
    }

    public void resetGateBet()
    {
        resetOCuoc();
        listWinResult.Clear();
        dataWin.Clear();

        t1 = 0;
        t2 = 0;
        t3 = 0;
        t4 = 0;
        t5 = 0;
        t6 = 0;
        t7 = 0;
        t8 = 0;
        t9 = 0;
        t10 = 0;

        listLabelGateBet.ForEach(lb =>
        {
            lb.gameObject.SetActive(false);
            lb.text = "0";
        });
        listGateTotalValue.Clear();
        listValueGateWin.Clear();
        for (int i = 0; i <= 15; i++)
        {
            List<ChipBetAndarBahar> tempList = new List<ChipBetAndarBahar>();
            listChipBetOnGate.Add(tempList);
            listGateTotalValue.Add(0);
            listValueGateWin.Add(0);
        }

        listGateTotalValue.Add(0);
        listValueGateWin.Add(0);
    }

    public Vector2 getPositionGateBet(int index)
    {
        if (index < 1 || index > listButtonBet.Count) return Vector2.zero;
        
        for (int i = 0; i < listButtonBet.Count; i++)
        {
            Button objButton = listButtonBet[i];
            if (i == index - 1)
            {
                return objButton.transform.localPosition;
            }
        }

        return Vector2.zero;
    }

    private ChipBetAndarBahar creatChipBet(int valueChip, int numberBet)
    {
        ChipBetAndarBahar chipBet;

        Transform parent = transform.Find(numberBet + "");
        if (numberBet < 9)
        {
            parent = transform.Find("MiniBet/" + numberBet);
        }
        
        if (chipPool.Count == 0)
        {
            ChipBetAndarBahar cbs = Instantiate(chip_bet_prefab, parent).GetComponent<ChipBetAndarBahar>();
            chipPool.Add(cbs);
        }

        chipBet = chipPool[0];
        chipPool.RemoveAt(0);
        chipBet.transform.SetParent(parent);
        chipBet.transform.localScale = Vector2.one;
        chipBet.transform.SetAsLastSibling();
        chipBet.gameObject.SetActive(true);
        chipBet.setChip(valueChip, numberBet);
        DOTween.Kill(chipBet.transform, true);
        return chipBet;
    }

    public int setValueGateBet(int index, int value)
    {
        int valueBet = 0;
        switch (index)
        {
            case 1:
                t1 += value;
                valueBet = t1;
                break;
            case 2:
                t2 += value;
                valueBet = t2;
                break;
            case 3:
                t3 += value;
                valueBet = t3;
                break;
            case 4:
                t4 += value;
                valueBet = t4;
                break;
            case 5:
                t5 += value;
                valueBet = t5;
                break;
            case 6:
                t6 += value;
                valueBet = t6;
                break;
            case 7:
                t7 += value;
                valueBet = t7;
                break;
            case 8:
                t8 += value;
                valueBet = t8;
                break;
            case 9:
                t9 += value;
                valueBet = t9;
                break;
            case 10:
                t10 += value;
                valueBet = t10;
                break;
        }

        setValueLabelBet(index, valueBet);
        return valueBet;
    }

    private void setValueLabelBet(int numberBet, int value)
    {
        for (int i = 0; i < listLabelGateBet.Count; i++)
        {
            if (i == numberBet - 1)
            {
                TextMeshProUGUI labelBet = listLabelGateBet[i];
                labelBet.gameObject.SetActive(true);
                labelBet.text = Globals.Config.FormatMoney(value, true);
            }
        }
    }

    public void setValueChipForGate(int gateID, int value)
    {
        listGateTotalValue[gateID] += value;
        if (listChipPerGate[gateID] == null)
        {
            listChipPerGate[gateID] = creatChipBet(value, gateID);
        }

        listChipPerGate[gateID].gameObject.SetActive(true);
        listChipPerGate[gateID].gameObject.transform.localPosition = Vector2.zero;
        listChipPerGate[gateID].setChip(listGateTotalValue[gateID], gateID);
        listLabelGateBet[gateID - 1].gameObject.SetActive(true);
        listLabelGateBet[gateID - 1].text = Globals.Config.FormatMoney(listGateTotalValue[gateID], true);
    }

    public void effectWinGate(int index)
    {
        int resultWin = index - 1;
        listWinResult.Add(resultWin);

        for (int i = 0; i < listButtonBet.Count; i++)
        {
            if (i == resultWin)
            {
                Button objButton = listButtonBet[i];
                Sprite spr = objButton.spriteState.pressedSprite;
                GameObject btnPress = objButton.transform.Find("press").gameObject;
                if (btnPress != null)
                {
                    Image btnImgPress = btnPress.GetComponent<Image>();
                    btnImgPress.sprite = spr;
                    btnPress.gameObject.SetActive(true);
                    btnImgPress.enabled = true;
                    Color normalColor = btnImgPress.color;
                    Color noOpacity = new Color(1, 1, 1, 0);
                    DOTween.Sequence()
                        .Append(btnImgPress.DOColor(noOpacity, 0.1f))
                        .Append(btnImgPress.DOColor(normalColor, 0.1f))
                        .SetLoops(10)
                        .OnComplete(() =>
                        {
                            btnPress.gameObject.SetActive(false);
                            btnImgPress.enabled = false;
                        });
                }
            }
        }
    }

    public void getChipLose()
    {
        for (int i = 0; i < listChipPerGate.Count; i++)
        {
            //Globals.Logging.Log(i + "======>" + listChipPerGate[i] + "--value:" + listGateTotalValue[i] + "----listWinResult=");
            if (!listWinResult.Contains(i - 1) && listChipPerGate[i] != null &&
                listGateTotalValue[i] != 0) //check xem i cos tồn tại trong mảng win ko va gate do co dat tien ko;
            {
                ChipBetAndarBahar chipLoseEff =
                    Instantiate(listChipPerGate[i].gameObject, listChipPerGate[i].transform.parent)
                        .GetComponent<ChipBetAndarBahar>();

                Vector2 pos = _andarBaharView.dealer.transform.localPosition;
                Vector2 orgPos = _andarBaharView.transform.TransformPoint(pos);
                Vector2 posReal = listChipPerGate[i].transform.parent.InverseTransformPoint(orgPos);
                TweenCallback cb = () => { Destroy(chipLoseEff.gameObject); };
                chipLoseEff.chipMoveTo(posReal, false, cb);
                listChipPerGate[i].gameObject.SetActive(false);
            }
        }
    }

    public void createChipWin(int gateID) // hien dong chip tong win ben canh dong chip tong cuoc cua gate do
    {
        int value = listValueGateWin[gateID];
        if (listValueGateWin[gateID] > 0)
        {
            if (listChipWinPerGate[gateID] == null)
            {
                listChipWinPerGate[gateID] = creatChipBet(value, gateID);
            }
            else
            {
            }

            listChipWinPerGate[gateID].setChip(value, gateID);
            Vector2 posGate = getPositionGateBet(gateID);
            posGate = new Vector2(posGate.x + (posGate.x > 0 ? -35 : 35), posGate.y);
            listChipWinPerGate[gateID].transform.localPosition = listChipWinPerGate[gateID].transform.parent
                .InverseTransformPoint(transform.TransformPoint(posGate));
            listChipWinPerGate[gateID].gameObject.SetActive(true);
        }
    }

    public int setValueGateWin(int index, int value)
    {
        int valueWin = 0;

        for (int i = 0; i < listValueGateWin.Count; i++)
        {
            if (index == i)
            {
                listValueGateWin[i] += value;
                valueWin = listValueGateWin[i];
            }
        }

        return valueWin;
    }

    public void creatDataChipWin()
    {
        dataWin.Clear();
        for (int i = 0; i < listValueGateWin.Count; i++)
        {
            if (listValueGateWin[i] > 0)
            {
                initData(i, listValueGateWin[i]);
            }
        }
    }

    private void initData(int numberWin, int value)
    {
        JObject data = new JObject();
        data["numberWin"] = numberWin;
        data["value"] = value;
        dataWin.Add(data);
    }
}