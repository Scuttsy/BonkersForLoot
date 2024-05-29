using UnityEngine;
using UnityEngine.UI;

public class ControllerInputHandler : MonoBehaviour
{
    public Button button1;
    public Button button2;

    private Button currentButton;

    void Start()
    {
        currentButton = button1;
        currentButton.Select();
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            currentButton = (currentButton == button1) ? button2 : button1;

            if (currentButton && currentButton.IsInteractable())
            {
                currentButton.Select();
            }
        }
    }
}
