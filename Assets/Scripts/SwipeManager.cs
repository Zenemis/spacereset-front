using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SwipeManager : MonoBehaviour
{
    enum State 
    {
        Explore,
        Gyro,
        Profile,
        Sliding
    }

    [Tooltip("Swipe menus")]
    public GameObject myProfilePanel;
    private RectTransform panelRectTransform;
    [SerializeField] private State state = State.Gyro;

    [Tooltip("Swipe variables")]
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool isSwiping = false;
    public float minSwipeDistance = 50f;
    public float screenWidth = 1080f;

    private float generalOffset = 0f;
    private bool canInteract = true;
    private float lerpTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        panelRectTransform = myProfilePanel.GetComponent<RectTransform>();
        panelRectTransform.offsetMin = new Vector2(screenWidth, panelRectTransform.offsetMin.y);
        panelRectTransform.offsetMax = new Vector2(screenWidth, panelRectTransform.offsetMax.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract) 
        {
            float swipe = DetectSwipe();
            SwipePanels(swipe);
        } else {
            InterpolateToStatePosition();
        }
    }

    private void SetStateToGyro()
    {
        state = State.Gyro;
        generalOffset = 0f;
        canInteract = false;
        lerpTime = 0f;
    }

    private void SetStateToProfile()
    {
        state = State.Profile;
        generalOffset = 1080f;
        canInteract = false;
        lerpTime = 0f;
    }

    private void InterpolateToStatePosition(){
        lerpTime += 2f * Time.deltaTime;
        float posForProfileMin = Mathf.Lerp(panelRectTransform.offsetMin.x, generalOffset + screenWidth, lerpTime);
        float posForProfileMax = Mathf.Lerp(panelRectTransform.offsetMax.x, generalOffset + screenWidth, lerpTime);
        panelRectTransform.offsetMin = new Vector2(posForProfileMin, panelRectTransform.offsetMin.y);
        panelRectTransform.offsetMax = new Vector2(posForProfileMax, panelRectTransform.offsetMax.y);
        if (lerpTime >= 1f){
            lerpTime = 0f;
            canInteract = true;
        }
    }

    private void SwipePanels(float swipe)
    {
        if (swipe == Mathf.Infinity){
            switch (state) {
                case State.Explore :
                    break;
                case State.Gyro :
                    SetStateToProfile();
                    break;
                case State.Profile :
                    break;
            }
        } else if (swipe == Mathf.NegativeInfinity) {
            switch (state) {
                case State.Explore :
                    break;
                case State.Gyro :
                    break;
                case State.Profile :
                    SetStateToGyro();
                    break;
            }
        } else {
            float amount = screenWidth*2*swipe/Screen.width;
            RectTransform panelRectTransform = myProfilePanel.GetComponent<RectTransform>();
            panelRectTransform.offsetMin = new Vector2(generalOffset + amount, panelRectTransform.offsetMin.y);
            panelRectTransform.offsetMax = new Vector2(generalOffset + amount, panelRectTransform.offsetMax.y);
        }

    }

    private float ComputeSwipe(){
        float swipeDistance = (endTouchPosition - startTouchPosition).x;
        if (Mathf.Abs(swipeDistance) >= minSwipeDistance)
        {
            if (swipeDistance > 0)
            {
                return Mathf.Infinity;
            }
            else
            {
                return Mathf.NegativeInfinity;
            }
        }
        return (endTouchPosition - startTouchPosition).x;
    }

    private float DetectSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    isSwiping = true;
                    return 0;

                case TouchPhase.Moved:
                    endTouchPosition = touch.position;
                    return (endTouchPosition - startTouchPosition).x;

                case TouchPhase.Ended:
                    if (isSwiping)
                    {
                        isSwiping = false;
                        return ComputeSwipe();
                    }
                    return (endTouchPosition - startTouchPosition).x;
                
                default :
                    return (endTouchPosition - startTouchPosition).x;
            }
        }
        return 0;
    }
}
