using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueIDGenerator
{
    private int currMaxID = -1;

    public UniqueIDGenerator()
    {
        currMaxID = -1;
    }

    public UniqueIDGenerator(int firstID)
    {
        currMaxID = firstID - 1;
    }

    public int createNewID()
    {
        currMaxID += 1;
        return currMaxID;
    }
}
