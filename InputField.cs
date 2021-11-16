using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputField : MonoBehaviour
{
    public static InputField instance;

    NumberField lastField;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void ActivateInputField(NumberField field)
    {
        this.gameObject.SetActive(true);
        lastField = field;
    }

    public void ClickedInput(int number)
    {
        lastField.ReceiveInput(number);
        //DEACTIVATE PANEL
        this.gameObject.SetActive(false);
    }
}
