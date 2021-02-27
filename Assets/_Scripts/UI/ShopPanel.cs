using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShopPanel : MonoBehaviour, IMenuPanel
{
    public GameManager gm;
    public Menu menu;
    public RectTransform parent;
    public RectTransform contentParent;
    public ShopEntry entryTemplate;

    private List<ShopEntry> entrys = new List<ShopEntry>();

    private int buyNumber = 1;

    public void Click()
    {
        menu.Click(this);
    }

    public void OpenPanel()
    {
        parent.DOAnchorPosY(0, .5f);
        SetupUI();
    }

    public void ClosePanel()
    {
        parent.DOAnchorPosY(-400, .5f);
    }

    private void SetupUI()
    {
        if (entrys.Count == 0)
            for (int i = 0; i < gm.allPlanets.Count; i++)
            {
                ShopEntry entry = Instantiate(entryTemplate, contentParent);
                entrys.Add(entry);
                entry.gameObject.SetActive(true);
                entrys[i].SetupEntry(gm.allPlanets[i]);
            }
    }

    private void Update()
    {
        if ((Object)menu.openedMenu == this)
            if (Time.frameCount % 10 == 0)
            {
                UpdatePrice();
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

    public void SetBuyNumber(int nbr)
    {
        buyNumber = nbr;
        UpdatePrice();
    }

    private void UpdatePrice()
    {
        for (int i = 0; i < entrys.Count; i++)
        {
            entrys[i].GetCurrentCost(buyNumber);
        }
    }
}
