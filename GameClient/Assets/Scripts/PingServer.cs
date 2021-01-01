using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine;

public class PingServer : MonoBehaviour
{
    public static int serverMilliseconds;
    public static int serverSeconds;

    public static void CalculatePing()
    {
        //dear god i need to fix this at some point

        int clientSeconds = DateTime.Now.Second;
        int clientMilliseconds = DateTime.Now.Millisecond;

        if (clientMilliseconds > serverMilliseconds)
        {
            if (clientSeconds > serverSeconds)
            {
                //Debug.Log($"ping: {clientSeconds - serverSeconds}");
            }
            else
            {
                //Debug.Log("ping: 0");
            }
        }
        else
        {
           // Debug.Log("test");
            
            //Debug.Log($"ping: {clientMilliseconds - serverMilliseconds}");
        }
    }
}
