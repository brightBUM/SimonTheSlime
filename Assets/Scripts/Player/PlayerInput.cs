using System;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Action PoundAbility;
    public Action PoundReleased;
    //public Action BulletTimeAbility;  
    public Action DashAbility;
    public Action GrappleAbility;
    public Action mouseReleased;
    public Action RespawnToCheckPoint;
    public Action DoubleTapAbility;
    //public Action<Vector2> mouseClicked;
    public Action<Vector2> mouseDragging;

    Camera camRef;
    Vector2 mousePos;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float swipeThreshold = 50f; // Minimum swipe distance in pixels
    public float doubleTapMaxTime = 0.3f; // Maximum time between taps in seconds
    private float lastTapTime = 0f;
    private int tapCount = 0;
    public float minHoldingTime = 0.4f;
    private float holdtimer;
    private bool isSwipeDown;
    //private bool leftControl;
    // Start is called before the first frame update
    void Start()
    {
        camRef = Camera.main;
        //this.leftControl = SaveLoadManager.Instance.playerProfile.leftControls;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManger.Instance.IsPaused)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        MouseInput();
#elif UNITY_ANDROID
        TouchInput();
#endif

    }

    private void TouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            //detect whether left or right side of the screen
            Vector2 touchPos = touch.position;


            HandleAimTouch(touch);


        }
    }

    private void HandleAimTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:

                startTouchPosition = touch.position;
                break;

            case TouchPhase.Moved:

                endTouchPosition = touch.position;
                mousePos = camRef.ScreenToWorldPoint(endTouchPosition);
                mouseDragging.Invoke(mousePos);
                break;

            case TouchPhase.Ended:

                endTouchPosition = touch.position;

                // Swipe Down Detection
                //if (IsSwipeDown(startTouchPosition, endTouchPosition))
                //{
                //    OnSwipeDown();
                //}

                mouseReleased.Invoke();
                break;
        }
    }
    private void HandleAbilitiesTouch(Touch touch)
    {

        switch (touch.phase)
        {
            case TouchPhase.Began:

                startTouchPosition = touch.position;
                // Double Tap Detection
                if (Time.time - lastTapTime < doubleTapMaxTime)
                {
                    tapCount++;
                }
                else
                {
                    tapCount = 1;
                }

                lastTapTime = Time.time;

                if (tapCount == 2)
                {
                    //implement dash / sling
                    OnDoubleTap();

                }

                break;

            case TouchPhase.Moved:
                // Check swipe direction
                Vector2 direction = touch.position - startTouchPosition;
                if (direction.y < -swipeThreshold && !isSwipeDown) // Swipe down threshold
                {
                    isSwipeDown = true;
                    PoundAbility.Invoke();
                }
                else
                {
                    isSwipeDown = false;
                }
                break;

            case TouchPhase.Stationary:

                //implement slam held down
                if (isSwipeDown)
                {
                    PoundAbility.Invoke();
                }
                break;

            case TouchPhase.Ended:
                endTouchPosition = touch.position;
                PoundReleased.Invoke();
                holdtimer = 0;
                break;
        }

    }
    private void MouseInput()
    {
        //mouse and keyboard input for windows/editor
        if (Input.GetMouseButton(0))
        {
            mousePos = camRef.ScreenToWorldPoint(Input.mousePosition);
            mouseDragging.Invoke(mousePos);
        }
        if (Input.GetMouseButtonUp(0) || (Input.GetKeyUp(KeyCode.Space)))
        {
            mouseReleased.Invoke();
        }
        if (Input.GetMouseButtonDown(1))
        {
            PoundAbility.Invoke();
        }
        if (Input.GetMouseButtonUp(1))
        {
            PoundReleased.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            DashAbility.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GrappleAbility.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnToCheckPoint.Invoke();
        }
    }

    private bool IsSwipeDown(Vector2 start, Vector2 end)
    {
        float verticalDistance = start.y - end.y;
        float horizontalDistance = Mathf.Abs(start.x - end.x);

        return verticalDistance > swipeThreshold && verticalDistance > horizontalDistance;
    }

    private void OnSwipeDown()
    {
        Debug.Log("Swipe Down Detected!");
        PoundAbility.Invoke();
        // Add your swipe down handling logic here
    }

    private void OnDoubleTap()
    {
        DoubleTapAbility.Invoke();
        // Add your double tap handling logic here
    }
}
