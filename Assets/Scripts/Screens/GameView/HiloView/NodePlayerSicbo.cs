using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Globals;

public class NodePlayerSicbo : BaseView
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject item_player;
    [SerializeField]
    ScrollRect list_player;
    //HiloView sicboGameView;
    SicboView sicboGameView;


    protected override void OnEnable()
    {
        base.OnEnable();
        sicboGameView = (SicboView)UIManager.instance.gameView;
        loadListPlayer();
    }
    public void loadListPlayer()
    {
        List<Player> list_data_player = sicboGameView.listPlayerSicbo;
        Globals.Logging.Log("list_data_player:" + list_data_player.Count);
        UIManager.instance.destroyAllChildren(list_player.content);
        for (int i = 0; i < list_data_player.Count; i++)
        {
            Player objData = list_data_player[i];
            ItemPlayerSicbo item = Instantiate(item_player, list_player.content).GetComponent<ItemPlayerSicbo>();
            item.setInfo(objData);
            item.gameObject.SetActive(true);

            //item.GetComponent<Image>().enabled = i % 2 == 0;
        }

    }
    public void onClose()
    {
        onClickClose();
    }
}
