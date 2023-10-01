using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using ServerUtils;
using UnityEngine.Networking;

public class MyAccountManager : MonoBehaviour
{

    [Header("Inputs")]
    public GameObject nameInput;
    public GameObject surnameInput;
    public GameObject oldPwdInput;
    public GameObject newPwdInput;

    [Header("Other")]
    public GameObject badPwdText;

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
