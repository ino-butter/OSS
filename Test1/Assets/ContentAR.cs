using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentAR : MonoBehaviour
{
    public GameObject contentAR;

    public void OnEnable()
    {
        contentAR.SetActive(true);
    }
    public void OnLoadTile()
    {
        contentAR.SetActive(false);
    }
}
