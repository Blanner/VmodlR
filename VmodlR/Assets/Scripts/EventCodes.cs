using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventCodes
{
    public const byte synchronizeClass = 1;
    public const byte updateTargetEndAttachState = 2;
    public const byte updateOriginEndAttachState = 3;

    public static bool IsUpdateTargetAttachmentEvent(byte eventCode)
    {
        return eventCode == updateTargetEndAttachState;
    }

    public static bool IsUpdateOriginAttachmentEvent(byte eventCode)
    {
        return eventCode == updateOriginEndAttachState;
    }
}
