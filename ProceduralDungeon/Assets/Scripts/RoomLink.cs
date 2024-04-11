using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Room Links are used to keep track of connections between rooms that will be used in conjunction with A* to create hallways
 */
public class RoomLink
{
    private FloorNode firstCenter;
    private FloorNode secondCenter;

    public RoomLink(FloorNode _firstCenter, FloorNode _secondCenter)
    {
        firstCenter = _firstCenter;
        secondCenter = _secondCenter;
    }

    public FloorNode GetFirstCenter()
    {
        return firstCenter;
    }

    public FloorNode GetSecondCenter()
    {
        return secondCenter;
    }
}
