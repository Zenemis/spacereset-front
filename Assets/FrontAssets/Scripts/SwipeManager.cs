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
    }

    [Tooltip("Swipe menus")]
    public GameObject swipeRightGroup;
    private RectTransform rightRectTransform;
    [SerializeField] private State state = State.Gyro;

    [Tooltip("Swipe variables")]
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool isSwiping = false;
    public float minSwipeDistance = 50f;
    public float screenWidth;

    private float generalOffset = 0f;
    private bool canInteract = true;
    private float lerpTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rightRectTransform = swipeRightGroup.GetComponent<RectTransform>();
        rightRectTransform.offsetMin = new Vector2(screenWidth, rightRectTransform.offsetMin.y);
        rightRectTransform.offsetMax = new Vector2(screenWidth, rightRectTransform.offsetMax.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract) 
        {
            if (Input.touchCount > 0)
            {
                float swipe = DetectSwipe();
                SwipePanels(swipe);
            }
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
        generalOffset = -screenWidth;
        canInteract = false;
        lerpTime = 0f;
    }

    private void ResetToSameState(){
        canInteract = false;
        lerpTime = 0f;
    }

    private void InterpolateToStatePosition(){
        lerpTime += 2f * Time.deltaTime;
        float posForProfileMin = Mathf.Lerp(rightRectTransform.offsetMin.x, generalOffset + screenWidth, lerpTime);
        float posForProfileMax = Mathf.Lerp(rightRectTransform.offsetMax.x, generalOffset + screenWidth, lerpTime);
        rightRectTransform.offsetMin = new Vector2(posForProfileMin, rightRectTransform.offsetMin.y);
        rightRectTransform.offsetMax = new Vector2(posForProfileMax, rightRectTransform.offsetMax.y);
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
                    ResetToSameState();
                    break;
                case State.Profile :
                    SetStateToGyro();
                    break;
            }
        } else if (swipe == Mathf.NegativeInfinity) {
            switch (state) {
                case State.Explore :
                    break;
                case State.Gyro :
                    SetStateToProfile();
                    break;
                case State.Profile :
                    ResetToSameState();
                    break;
            }
        } else if (swipe == Mathf.Epsilon) {
            ResetToSameState();
        } else {
            float amount = screenWidth*2*swipe/Screen.width;
            RectTransform rightRectTransform = swipeRightGroup.GetComponent<RectTransform>();
            rightRectTransform.offsetMin = new Vector2(generalOffset + screenWidth + amount, rightRectTransform.offsetMin.y);
            rightRectTransform.offsetMax = new Vector2(generalOffset + screenWidth + amount, rightRectTransform.offsetMax.y);
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
        return Mathf.Epsilon;
    }

    private float DetectSwipe()
    {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                startTouchPosition = touch.position;
                isSwiping = true;
                return 0;

            case TouchPhase.Stationary:
                endTouchPosition = touch.position;
                return (endTouchPosition - startTouchPosition).x;

            case TouchPhase.Moved:
                endTouchPosition = touch.position;
                return (endTouchPosition - startTouchPosition).x;

            case TouchPhase.Ended:
                if (isSwiping)
                {
                    isSwiping = false;
                    return ComputeSwipe();
                }
                return Mathf.Epsilon;

            default :
                return Mathf.Epsilon;
        }
    }
}
