using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedVariables : MonoBehaviour
{
    public static Dictionary<int, Vector3> movementStore = new Dictionary<int, Vector3>();

    //no touchie!!!
    public static bool doCheck;
    public static Vector3 clientPos;
    public static Vector3 serverPos;
}
