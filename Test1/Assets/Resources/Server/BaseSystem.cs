using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSystem : MonoBehaviour
{
    protected virtual void Start()
    {
    }
    protected void OnDisable()
    {
        GameManager.instance.RemoveSystem(this);
    }
    protected void OnEnable()
    {
        GameManager.instance.AddSystem(this);
    }
}
