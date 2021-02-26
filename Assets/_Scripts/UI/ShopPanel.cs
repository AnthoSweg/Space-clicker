using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShopPanel : MonoBehaviour
{
    public GameManager gm;
    public RectTransform parent;
    public RectTransform contentParent;
    private bool opened;
    public ShopEntry entryTemplate;

    private List<ShopEntry> entrys = new List<ShopEntry>();

    private int buyNumber = 1;

    public void Click()
    {
        opened = !opened;

        if (opened)
            OpenPanel();
        else
            ClosePanel();
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
        if (Time.frameCount % 10 == 0)
            UpdateUI();
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
        for (int i = 0; i < entrys.Count; i++)
        {
            entrys[i].GetCurrentCost(nbr);
        }
    }
}
