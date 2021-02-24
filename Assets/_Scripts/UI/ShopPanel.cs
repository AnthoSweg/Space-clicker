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
        UpdateUI();
    }

    public void ClosePanel()
    {
        parent.DOAnchorPosY(-400, .5f);
    }

    private void UpdateUI()
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
}
