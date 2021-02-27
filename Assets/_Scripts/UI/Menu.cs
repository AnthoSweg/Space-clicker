using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    public IMenuPanel openedMenu;

    public void Click(IMenuPanel panel)
    {
        if(openedMenu != null)
            openedMenu.ClosePanel();

        if (panel == openedMenu)
        {
            openedMenu = null;
        }
        else
        {
            openedMenu = panel;
            openedMenu.OpenPanel();
        }
    }
}