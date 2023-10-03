using System;
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
    private Vector2 panelVisiblePosition;
    private Vector2 panelAwayPosition;

    [Header("Superpanels")]
    public GameObject deleteAccountSuperPanel;

    private Color superpanelsBackgroundsColor;

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
        
        panelVisiblePosition = myProfilePanel.GetComponent<RectTransform>().anchoredPosition;
        panelAwayPosition = new Vector2(panelVisiblePosition.x + Screen.width,
                                        panelVisiblePosition.y);
        superpanelsBackgroundsColor = deleteAccountSuperPanel.GetComponent<Image>().color;


        foreach (GameObject p in allPanels){
            p.SetActive(false);
            p.GetComponent<RectTransform>().anchoredPosition = panelAwayPosition;
        }

        activePanel = myProfilePanel;

        previousPanels = new Stack<GameObject>();
        activePanel.GetComponent<RectTransform>().anchoredPosition = panelVisiblePosition;
        activePanel.SetActive(true);
        backButton.SetActive(false);
    }

    IEnumerator FadeAnimation(Image img, Color initialColor, Color targetColor, Action callback = null)
    {
        float speed = 8f;
        for (float progress = 0f; progress < 1; progress += speed*Time.deltaTime)
        {
            speed -= 0.4f*Time.deltaTime;
            img.color = new Color(
                Mathf.Lerp(initialColor.r, targetColor.r, progress),
                Mathf.Lerp(initialColor.g, targetColor.g, progress),
                Mathf.Lerp(initialColor.b, targetColor.b, progress),
                Mathf.Lerp(initialColor.a, targetColor.a, progress)
            );
            yield return null;
        }
        img.color = targetColor;
        if (callback != null){
            callback();
        }
    }

    public void ActivateSuperpanel(GameObject panel)
    {
        Image background = deleteAccountSuperPanel.GetComponent<Image>();
        GameObject window = deleteAccountSuperPanel.transform.GetChild(0).gameObject;
        RectTransform windowPos = window.GetComponent<RectTransform>();
        backButton.GetComponent<Button>().interactable = false;
        panel.SetActive(true);
        StartCoroutine(FadeAnimation(background, new Color(0,0,0,0), superpanelsBackgroundsColor, null));
        StartCoroutine(PanelAnimation(window, windowPos.anchoredPosition, windowPos.anchoredPosition + new Vector2(0, windowPos.sizeDelta.y) , null));
    }

    public void DeactivateSuperpanel(GameObject panel)
    {
        Image background = deleteAccountSuperPanel.GetComponent<Image>();
        GameObject window = deleteAccountSuperPanel.transform.GetChild(0).gameObject;
        RectTransform windowPos = window.GetComponent<RectTransform>();
        Action switchPanel = () => {
            panel.SetActive(false);
            backButton.GetComponent<Button>().interactable = true;
        };
        StartCoroutine(FadeAnimation(background, superpanelsBackgroundsColor, new Color(0,0,0,0), null));
        StartCoroutine(PanelAnimation(window, windowPos.anchoredPosition, windowPos.anchoredPosition - new Vector2(0, windowPos.sizeDelta.y), switchPanel));
    }

    IEnumerator PanelAnimation(GameObject panel, Vector2 panelInitialPosition, Vector2 panelTargetPosition, Action callback = null)
    {
        float speed = 8f;
        for (float progress = 0f; progress < 1; progress += speed*Time.deltaTime)
        {
            speed -= 0.4f*Time.deltaTime;
            panel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(panelInitialPosition, panelTargetPosition, progress);
            yield return null;
        }
        panel.GetComponent<RectTransform>().anchoredPosition = panelTargetPosition;
        if (callback != null){
            callback();
        }
    }

    public void ActivatePanel(GameObject panel)
    {
        previousPanels.Push(activePanel);
        backButton.SetActive(true);
        panel.SetActive(true);
        GameObject oldPanel = activePanel;
        activePanel = panel;
        Action switchPanel = () =>
        {
            oldPanel.SetActive(false);
        };
        StartCoroutine(PanelAnimation(activePanel, panelAwayPosition, panelVisiblePosition, switchPanel));
    }

    public void BackToPreviousPanel()
    {
        GameObject prev = previousPanels.Pop();
        prev.SetActive(true);
        Action switchPanel = () =>
        {
            activePanel.SetActive(false);
            prev.SetActive(true);
            activePanel = prev;
            if (previousPanels.Count == 0){
                backButton.SetActive(false);
            }
        };
        StartCoroutine(PanelAnimation(activePanel, panelVisiblePosition, panelAwayPosition, switchPanel));
    }
}