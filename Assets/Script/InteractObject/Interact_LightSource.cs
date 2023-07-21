using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_LightSource : MonoBehaviour
{
    [SerializeField]private float startLightSourceSize;
    public void GetLightSource(PlayerLight playerLightReceive)
    {
        //main animasi di sini kalo mau kalo tidak yasuda
        playerLightReceive.restartFireSize(startLightSourceSize);
        Destroy(this.gameObject);
    }
}
