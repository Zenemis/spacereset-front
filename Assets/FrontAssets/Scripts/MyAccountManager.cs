using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using ServerUtils;
using UnityEngine.Networking;

public class MyAccountManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject myProfilePanel;
    public GameObject myInfosPanel;
    public GameObject modifyIdentityPanel;
    public GameObject modifyCredentialsPanel;

    public GameObject myFragmentsPanel;

    [HideInInspector] public GameObject activePanel;
    private List<GameObject> allPanels;
    private Stack<GameObject> previousPanels;

    [Header("Inputs")]
    public GameObject nameInput;
    public GameObject surnameInput;
    public GameObject oldPwdInput;
    public GameObject newPwdInput;

    [Header("Other")]
    public GameObject badPwdText;
    public GameObject backButton;

    // Start is called before the first frame update
    void Start()
    {
        allPanels = new List<GameObject>();
        allPanels.Add(myProfilePanel);
        allPanels.Add(myInfosPanel);
        allPanels.Add(modifyIdentityPanel);
        allPanels.Add(modifyCredentialsPanel);
        previousPanels = new Stack<GameObject>();
        activePanel = myProfilePanel;
        backButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateSuperpanel(GameObject panel){
        backButton.GetComponent<Button>().interactable = false;
        panel.SetActive(true);
    }

    public void DeactivateSuperpanel(GameObject panel){
        backButton.GetComponent<Button>().interactable = true;
        panel.SetActive(false);
    }

    public void ActivatePanel(GameObject panel){
        previousPanels.Push(activePanel);
        backButton.SetActive(true);
        foreach (GameObject p in allPanels){
            p.SetActive(false);
            if (p == panel){
                p.SetActive(true);
                activePanel = p;
            }
        }
    }

    public void BackToPreviousPanel(){
        GameObject prev = previousPanels.Pop();
        activePanel.SetActive(false);
        prev.SetActive(true);
        activePanel = prev;
        if (previousPanels.Count == 0){
            backButton.SetActive(false);
        }
    }

    async public void OnModifyAccountName(){
        TMP_InputField nameInputField = nameInput.GetComponent<TMP_InputField>();
        TMP_InputField surnameInputField = surnameInput.GetComponent<TMP_InputField>();

        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("nom", nameInputField.text);
        form.Add("prenom", surnameInputField.text);

        WWWForm toSend = SRServer.FormFromDict(form);
        await SRServer.Upload("modify_client/", toSend);
    }

    public async void OnModifyAccountCredentials(){
        TMP_InputField oldPwdInputField = oldPwdInput.GetComponent<TMP_InputField>();
        TMP_InputField newPwdInputField = newPwdInput.GetComponent<TMP_InputField>();

        string salt = await SRServer.Download("get_salt/");
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("salted_hash", SRServer.Sha256(salt+oldPwdInputField.text));
        WWWForm toSend = SRServer.FormFromDict(form);
        bool can_modify = ((await SRServer.Upload("check_pwd/", toSend)) == "Ok");

        if (can_modify){
            string new_salt = SRServer.NewSalt(32);
            form = new Dictionary<string, string>();
            form.Add("salt", new_salt);
            form.Add("salted_hash", SRServer.Sha256(salt+newPwdInputField.text));
            toSend = SRServer.FormFromDict(form);
            await SRServer.Upload("modify_client/", toSend);
            badPwdText.SetActive(false);
        } else {
            badPwdText.SetActive(true);
        }
    }

    public void OnDeleteAccount(){

    }

}
