using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayUtils 
{
    public static string ArrayToString<T>(T[] array)
    {
        string line = "[";
        foreach(T elem in array)
        {
            line += elem.ToString() + ", ";
        }
        line += "]";
        return line;
    }
}
