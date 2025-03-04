using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChipBetTeenPatti : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprChips = new List<Sprite>();
    [SerializeField] private Image imgChip;
    [SerializeField] private TextMeshProUGUI textChipValue;
    private int chipValue;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void SetChipValue(int value)
    {
        chipValue = value;
        if (chipValue >= 1 && chipValue <= sprChips.Count)
        {
            imgChip.sprite = sprChips[chipValue - 1];
        }
        else
        {
            Debug.LogWarning("Giá trị chip không hợp lệ: " + chipValue);
        }
        textChipValue.text = chipValue.ToString();
    }
    public int GetChipValue()
    {
        return chipValue;
    }
}
