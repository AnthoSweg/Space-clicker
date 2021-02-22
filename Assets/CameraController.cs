using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class CameraController : MonoBehaviour
{
    public Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void ZoomIn(Transform target)
    {
        DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, GameParams.Main.zoomInLevel, .2f);
        cam.transform.DOMove(new Vector3(target.position.x, target.position.y, -10), .2f);
    }

    public void ZoomOut()
    {
        DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, GameParams.Main.zoomOutLevel, .2f);
        cam.transform.DOMove(new Vector3(0, 0, -10), .2f);
    }
}
