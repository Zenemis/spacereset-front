using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using ServerUtils;
using UnityEngine.Networking;

public class MyAccountManager : MonoBehaviour
{
    [Tooltip("Panels")]
    public GameObject myProfilePanel;
    public GameObject myInfosPanel;
    public GameObject myFragmentsPanel;
    public GameObject modifyIdentityPanel;
    public GameObject modifyCredentialsPanel;

    private List<GameObject> allPanels;

    [Tooltip("Inputs")]
    public GameObject nameInput;
    public GameObject surnameInput;

    // Start is called before the first frame update
    void Start()
    {
        allPanels = new List<GameObject>();
        allPanels.Add(myProfilePanel);
        allPanels.Add(myInfosPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateSuperpanel(GameObject panel){
        panel.SetActive(true);
    }

    public void DeactivateSuperpanel(GameObject panel){
        panel.SetActive(false);
    }

    public void ActivatePanel(GameObject panel){
        foreach (GameObject p in allPanels){
            p.SetActive(false);
            if (p == panel){
                p.SetActive(true);
            }
        }
    }

    async public void OnModifyAccountName(){
        TMP_InputField nameInputField = nameInput.GetComponent<TMP_InputField>();
        TMP_InputField surnameInputField = surnameInput.GetComponent<TMP_InputField>();
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("nom", nameInputField.text);
        form.Add("prenom", surnameInputField.text);
        WWWForm toSend = SRServer.FormFromDict(form);
        await SRServer.Upload(toSend, "new_client");
    }

    public void OnModifyAccountCredentials(){
        TMP_InputField nameInputField = nameInput.GetComponent<TMP_InputField>();
        TMP_InputField surnameInputField = surnameInput.GetComponent<TMP_InputField>();
        
    }

    public void OnDeleteAccount(){

    }

}
