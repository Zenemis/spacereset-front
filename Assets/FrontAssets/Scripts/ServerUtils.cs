using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace ServerUtils {

    public static class SRServer
    {
        private static string serverUrl = "http://votre-serveur-ip-ou-domaine:3000/";

        public static string NewSalt(int maxLength)
        {
            var random = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] salt = new byte[maxLength];
            random.GetNonZeroBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public static string Sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        static public WWWForm FormFromDict(Dictionary<string,string> formInJson){
            WWWForm form = new WWWForm();
            foreach(KeyValuePair<string,string> formEntry in formInJson)
            {
                form.AddField(formEntry.Key, formEntry.Value);
            }
            return form;
        }

        static public async Task<string> Upload(string uri, WWWForm form)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(serverUrl + uri, form))
            {
                await www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                    return "error code " + www.responseCode + " : " + www.error;
                }
                else
                {
                    Debug.Log("Form upload complete!");
                    return www.downloadHandler.text;
                }
            }
        }

        static public async Task<string> Download(string uri)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(serverUrl + uri))
            {
                await www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                    return "error code " + www.responseCode + " : " + www.error;
                }
                else
                {
                    Debug.Log("Form download complete!");
                    return www.downloadHandler.text;
                }
            }
        }

    }
}