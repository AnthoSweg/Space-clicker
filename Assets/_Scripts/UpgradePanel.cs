using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class UpgradePanel : MonoBehaviour, IMenuPanel
{
    public RectTransform parent;
    public Menu menu;

    public List<UpgradeEntry> entrys = new List<UpgradeEntry>();

    public void Click()
    {
        menu.Click(this);
    }

    public void OpenPanel()
    {
        parent.DOAnchorPosY(0, .5f);
    }
    public void ClosePanel()
    {
        parent.DOAnchorPosY(-400, .5f);
    }

    private void Update()
    {
        if ((Object)menu.openedMenu == this)
            if (Time.frameCount % 10 == 0)
            {
                //UpdatePrice();
                UpdateUI();
            }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < entrys.Count; i++)
        {
            entrys[i].UpdateEntry();
        }
    }

}

public interface IMenuPanel
{
    void OpenPanel();
    void ClosePanel();
}
