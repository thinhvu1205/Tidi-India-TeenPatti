using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Globals;

public class SettingView : BaseView
{
    public static SettingView instance;

    [SerializeField] private Transform deletion;
    [SerializeField] private Button btnMusic, btnSound, btnVibration, btnGroup, btnFanpage, btnLogout, btnDeleteAc;
    [SerializeField] private TMP_Dropdown m_ChangeLanguageD;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        SettingView.instance = this;

        Globals.CURRENT_VIEW.setCurView(Globals.CURRENT_VIEW.SETTING_VIEW);
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
    private new void Start()
    {
        base.Start();
        //tglMusic.SetIsOnWithoutNotify(Globals.Config.isMusic);
        //tglSound.SetIsOnWithoutNotify(Globals.Config.isSound);
        SetStatusBtn(btnMusic.transform, Globals.Config.isMusic);
        SetStatusBtn(btnSound.transform, Globals.Config.isSound);
        SetStatusBtn(btnVibration.transform, Globals.Config.isVibration);


        btnGroup.gameObject.SetActive(Globals.Config.is_bl_fb);
        btnFanpage.gameObject.SetActive(Globals.Config.is_bl_fb);
        btnLogout.gameObject.SetActive(false);
        if (UIManager.instance.gameView != null && UIManager.instance.gameView.gameObject.activeSelf)
        {
            btnDeleteAc.interactable = false;
            deletion.Find("iconDelAc").GetComponent<Image>().color = Color.gray;
            deletion.Find("lbDelAc").GetComponent<TextMeshProUGUI>().color = Color.gray;
            deletion.Find("btnDeleAc").GetComponent<Image>().color = Color.gray;
        }
        else
        {
            btnDeleteAc.interactable = true;
            deletion.Find("iconDelAc").GetComponent<Image>().color = Color.white;
            deletion.Find("lbDelAc").GetComponent<TextMeshProUGUI>().color = Color.white;
            deletion.Find("btnDeleAc").GetComponent<Image>().color = Color.white;
        }
        // m_ChangeLanguageD.onValueChanged.AddListener((index) =>
        // {
        //     Config.loadTextConfig(index == 0 ? "THAI" : "EN");
        //     UIManager.instance.refreshUIFromConfig();
        //     foreach (var item in FindObjectsByType<CCFS>(FindObjectsSortMode.None)) item.RefreshUI();
        // });
        // m_ChangeLanguageD.value = Config.language.Equals("EN") ? 1 : 0;
    }

    private void SetStatusBtn(Transform btn, bool status)
    {
        if (status)
        {
            btn.GetChild(0).gameObject.SetActive(true);
            btn.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            btn.GetChild(0).gameObject.SetActive(false);
            btn.GetChild(1).gameObject.SetActive(true);
        }
    }
    public void onClickFanpage()
    {
        SoundManager.instance.soundClick();
        string linkPage = "https://www.facebook.com/profile.php?id=" + Globals.Config.fanpageID;
        Application.OpenURL(linkPage);

    }
    public void onClickSupport()
    {

        if (!Config.fanpageID.Equals("") && Config.is_bl_fb)
        {
            SoundManager.instance.soundClick();
            Application.OpenURL("https://" + Config.u_chat_fb);
        }
        else
        {
            UIManager.instance.openFeedback();
        }
    }
    public void onClickPolicy()
    {
        Application.OpenURL(Config.url_privacy_policy); //Config.url_privacy_policy
    }
    public void onClickGroup()
    {
        SoundManager.instance.soundClick();
        string linkPage = "https://www.facebook.com/groups/" + Globals.Config.groupID;
        Application.OpenURL(linkPage);

    }
    public void onClickSound()
    {
        Globals.Config.isSound = !Globals.Config.isSound;
        if (Globals.Config.isSound)
        {
            SoundManager.instance.soundClick();
        }
        Globals.Config.updateConfigSetting();
        SetStatusBtn(btnSound.transform, Globals.Config.isSound);
    }
    public void onClickMusic()
    {
        Globals.Config.isMusic = !Globals.Config.isMusic;
        SoundManager.instance.soundClick();
        Globals.Config.updateConfigSetting();
        SoundManager.instance.playMusic();
        SetStatusBtn(btnMusic.transform, Globals.Config.isMusic);
    }
    public void onClickVibration()
    {
        SoundManager.instance.soundClick();
        Globals.Config.isVibration = !Globals.Config.isVibration;
        Globals.Config.updateConfigSetting();
        SetStatusBtn(btnVibration.transform, Globals.Config.isVibration);
        Globals.Config.Vibration();
    }
    public void onClickChangeLanguage()
    {
        SoundManager.instance.soundClick();
    }

    public void onClickHelp()
    {
        SoundManager.instance.soundClick();
        var url_h = Globals.Config.url_help.Replace("%language%", "thai");

        UIManager.instance.showWebView(url_h);
    }

    public void onClickLogout()
    {
        SoundManager.instance.soundClick();
        SocketSend.sendLogOut();
        Config.typeLogin = LOGIN_TYPE.NONE;
        PlayerPrefs.SetInt("type_login", (int)LOGIN_TYPE.NONE);
        PlayerPrefs.Save();
        UIManager.instance.showLoginScreen(false);
        SocketIOManager.getInstance().emitSIOCCCNew("ClickLogOut");
    }
    public void handleDeleteAcount()
    {
        onClickLogout();
        UIManager.instance.loginView.accPlayNow = "";//= PlayerPrefs.GetString("USER_PLAYNOW", "");
        UIManager.instance.loginView.passPlayNow = "";// PlayerPrefs.GetString("PASS_PLAYNOW", "");
        PlayerPrefs.DeleteKey("USER_PLAYNOW");
        PlayerPrefs.DeleteKey("PASS_PLAYNOW");
    }
    public void onClickAccountDeletion()
    {
        UIManager.instance.showDialog(Globals.Config.getTextConfig("txt_confirm_account_deletion"),
            Globals.Config.getTextConfig("txt_ok")
            , (() =>
            {
                SocketSend.sendRequestAccountDeletion();
            }),
              Globals.Config.getTextConfig("txt_cancel"));
    }
}
