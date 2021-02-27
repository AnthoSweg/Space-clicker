using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class background : MonoBehaviour
{
    public List<Sprite> images = new List<Sprite>();
    public SpriteRenderer spriteR;

    private void Start()
    {
        spriteR.color = new Color(1, 1, 1, 0);
        GetRandomBG();
    }

    void GetRandomBG()
    {
        spriteR.sprite = images[Random.Range(0, images.Count)];
        spriteR.DOColor(new Color(1, 1, 1, .75f), 10).SetEase(Ease.InOutQuint).OnComplete(()=>
        {
            spriteR.DOColor(new Color(1, 1, 1, 0), 10).SetEase(Ease.InOutQuint).OnComplete(() =>
            {
                GetRandomBG();
            });
        });
    }
}
