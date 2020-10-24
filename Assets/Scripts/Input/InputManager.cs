using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private IInputStrategy inputStrategy = new InputStrategyMouse();

    void Update()
    {
        inputStrategy.CheckInput();
    }
}



internal interface IInputStrategy
{
    void CheckInput();
}

internal class InputStrategyMouse : IInputStrategy
{
    private bool isMouseDown = false;

    public void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                interactable?.OnTouchBegin();
            }
            else
            {
                GameManager.DeselectAll();
            }
            return;
        }

        if (isMouseDown && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                interactable?.OnTouchMoved();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                interactable?.OnTouchEnd();
            }
            else
            {
                GameManager.DeselectAll();
            }
        }
    }
}

internal class InputStrategyTouch : IInputStrategy
{
    public void CheckInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.touches[0];

        {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (touch.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                    interactable?.OnTouchBegin();
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                    interactable?.OnTouchMoved();
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {

                if (Physics.Raycast(ray, out hit))
                {
                    IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                    interactable?.OnTouchEnd();
                }
                GameManager.DeselectAll();
            }
        }
    }
}