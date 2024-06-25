using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    IDLE,
    AIMING,
    LAUNCHED,
    POUND
}
public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnEnable()
    {
        playerInput.mouseClicked += LeftClicked;
        playerInput.mouseReleased += LeftReleased;
        playerInput.mouseDragging += LeftDragging;
        playerInput.rightClicked += RightClicked;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void LeftClicked()
    {
        Debug.Log("left mouse clicked");
    }
    private void LeftReleased()
    {
        Debug.Log("left mouse released");
    }
    private void LeftDragging(Vector2 mousePos)
    {
        Debug.Log("mouse dragging pos -"+mousePos);
    }
    private void RightClicked()
    {
        Debug.Log("right mouse clicked");
    }
    private void OnDisable()
    {
        playerInput.mouseClicked -= LeftClicked;
        playerInput.mouseReleased -= LeftReleased;
        playerInput.mouseDragging -= LeftDragging;
        playerInput.rightClicked -= RightClicked;
    }
}
