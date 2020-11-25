using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public int index;
    public int hp = 100;
    public int strength = 10;
    public float radius = 5;
    public float recovery = 1;
    
    public Vector3 GetPosition_2D() => new Vector3(transform.position.x,0,transform.position.z);
}
