using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public LayerMask layerToRaycastTo;
    private IInputStrategy inputStrategy;

    void Start()
    {
        inputStrategy = new InputStrategyMouse(layerToRaycastTo);
    }

    void Update()
    {
        if (!GameSpeed.IsPaused) inputStrategy.CheckInput();
    }
}



internal interface IInputStrategy
{
    void CheckInput();
}

internal class InputStrategyMouse : IInputStrategy
{
    private bool isMouseDown = false;
    private LayerMask layerToRaycastTo;

    public InputStrategyMouse(LayerMask layerToRaycastTo)
    {
        this.layerToRaycastTo = layerToRaycastTo;
    }

    public void CheckInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool rayHit = Physics.Raycast(ray, out hit, Mathf.Infinity, layerToRaycastTo);
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
            if (rayHit)
            {
                IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                interactable?.OnTouchBegin();
            }
            else
            {
                GameManager.Instance.DeselectAll();
            }
            return;
        }

        if (isMouseDown && Input.GetMouseButton(0))
        {
            if (rayHit)
            {
                IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                interactable?.OnTouchMoved();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            if (rayHit)
            {
                IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                interactable?.OnTouchEnd();
            }
            else
            {
                GameManager.Instance.DeselectAll();
            }
        }
    }
}

internal class InputStrategyTouch : IInputStrategy
{
    private LayerMask layerToRaycastTo;

    public InputStrategyTouch(LayerMask layerToRaycastTo)
    {
        this.layerToRaycastTo = layerToRaycastTo;
    }

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
                GameManager.Instance.DeselectAll();
            }
        }
    }
}