using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelsManager : MonoBehaviour
{
    [Header("Profile panels")]
    public GameObject myProfilePanel;
    public GameObject myInfosPanel;
    public GameObject modifyIdentityPanel;
    public GameObject modifyCredentialsPanel;

    [Header("Fragments panels")]
    public GameObject myFragmentsPanel;
    public GameObject selectedFragmentPanel;
    public GameObject fragmentSettingsPanel;

    [HideInInspector] public GameObject activePanel;
    private List<GameObject> allPanels;
    private Stack<GameObject> previousPanels;

    [Header("Other")]
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
        ActivatePanel(activePanel);
        backButton.SetActive(false);
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
}