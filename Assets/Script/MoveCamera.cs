using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using JetBrains.Annotations;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private GameObject targetCam;
    [SerializeField] private GameObject[] positions;
    CinemachineVirtualCamera myCinemachine;

    private void Start()
    {
        GameObject[] tempGameObject = new GameObject[targetCam.transform.childCount];
        positions = tempGameObject;
        myCinemachine = gameObject.GetComponent<CinemachineVirtualCamera>();
        for(int i = 0; i < positions.Length; i++)
        {
            positions[i] = targetCam.transform.GetChild(i).gameObject;
        }
    }

    public void Up()
    {
        float cameraHeight = 2 * Camera.main.orthographicSize;
        positions[1].gameObject.transform.position = positions[0].gameObject.transform.position + new Vector3(0f, cameraHeight, 0f);
        positions[0].gameObject.transform.position = positions[1].gameObject.transform.position;
    }

    public void Down()
    {
        float cameraHeight = 2 * Camera.main.orthographicSize;
        positions[1].gameObject.transform.position = positions[0].gameObject.transform.position - new Vector3(0f, cameraHeight, 0f);
        positions[0].gameObject.transform.position = positions[1].gameObject.transform.position;
    }

    public void Right()
    {
        float cameraWidth = Camera.main.aspect * 2 * Camera.main.orthographicSize;
        positions[1].gameObject.transform.position = positions[0].gameObject.transform.position + new Vector3(cameraWidth, 0f, 0f);
        positions[0].gameObject.transform.position = positions[1].gameObject.transform.position;
    }
    public void Left()
    {
        float cameraWidth = Camera.main.aspect * 2 * Camera.main.orthographicSize;
        positions[1].gameObject.transform.position = positions[0].gameObject.transform.position - new Vector3(cameraWidth, 0f, 0f);
        positions[0].gameObject.transform.position = positions[1].gameObject.transform.position;
    }
    /*   public void CheckClosestDown()
       {
           indexClosest = 0;

           int k = index;
           distance = -Mathf.Infinity;
           for (int i = 0; i < positions.Length; i++)
           {
               if (i == k)
                   continue;

               if (distance <= positions[i].gameObject.transform.position.y - positions[k].gameObject.transform.position.y)
               {
                   if (positions[i].gameObject.transform.position.y - positions[k].gameObject.transform.position.y < 0)
                   {
                       indexClosest = i;
                       index = indexClosest;
                       distance = positions[i].gameObject.transform.position.y - positions[k].gameObject.transform.position.y;
                   }
               }
           }
           Debug.Log(indexClosest + " : " + distance);
           MoveCameraToward(indexClosest);
       }

       public void CheckClosestUp()
       {
           indexClosest = 0;

           int k = index;
           distance = Mathf.Infinity;
           for (int i = 0; i < positions.Length; i++)
           {
               if (i == k)
                   continue;

               if (distance >= positions[i].gameObject.transform.position.y - positions[k].gameObject.transform.position.y)
               {
                   if (positions[i].gameObject.transform.position.y - positions[k].gameObject.transform.position.y > 0)
                   {
                       indexClosest = i;
                       index = indexClosest;
                       distance = positions[i].gameObject.transform.position.y - positions[k].gameObject.transform.position.y;
                   }
               }
           }
           Debug.Log(indexClosest + " : " + distance);
           MoveCameraToward(indexClosest);
       }

       public void CheckClosestLeft()
       {
           indexClosest = 0;

           int k = index;
           distance = -Mathf.Infinity;
           for (int i = 0; i < positions.Length; i++)
           {
               if (i == k)
                   continue;

               if (distance <= positions[i].gameObject.transform.position.x - positions[k].gameObject.transform.position.x)
               {
                   if (positions[i].gameObject.transform.position.x - positions[k].gameObject.transform.position.x < 0)
                   {
                       indexClosest = i;
                       index = indexClosest;
                       distance = positions[i].gameObject.transform.position.x - positions[k].gameObject.transform.position.x;
                   }
               }
           }
           Debug.Log(indexClosest + " : " + distance);
           MoveCameraToward(indexClosest);
       }

       public void CheckClosestRight()
       {
           indexClosest = 0;

           int k = index;
           distance = Mathf.Infinity;
           for (int i = 0; i < positions.Length; i++)
           {
               if(i == k)
                   continue;

               if (distance >= positions[i].gameObject.transform.position.x - positions[k].gameObject.transform.position.x)
               {
                   if(positions[i].gameObject.transform.position.x - positions[k].gameObject.transform.position.x > 0)
                   {
                       indexClosest = i;
                       index = indexClosest;
                       distance = positions[i].gameObject.transform.position.x - positions[k].gameObject.transform.position.x;
                   }
               }
           }
           Debug.Log(indexClosest + " : " + distance);
           MoveCameraToward(indexClosest);
       }*/
}
