using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reset : MonoBehaviour
{
    public GameObject resetObj;
    public Vector3 resetPos;
    public Gomoku gomoku;

    public void Reset(){
      resetObj.transform.position = resetPos;
    }

    public void realPut(){
      gomoku.realPut();
    }
}
