using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleConsole : MonoBehaviour
{
    public GameObject Weapons;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote)) // the tilde key is called 'BackQuote' in Unity's keycode system
        {
            bool isActive = Weapons.activeSelf;

            Weapons.SetActive(!isActive);

            // Set cursor visibility and lock state based on the new state of Weapons
            if (!isActive)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}