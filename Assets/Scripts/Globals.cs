using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public static string host="localhost:8058";
    public static string username="zjs";
    public static string protocol = "ws";
    public const string clientVersion = "4";
    public static string toJoin;

    public static bool checkValid(string s)
    {
        if (s.Length == 0 || s.Length > 30)
        {
            return false;
        }

        return !s.Contains('#');
    }
}
