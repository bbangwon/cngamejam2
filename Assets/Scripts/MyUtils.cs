using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyUtils
{
    public static Vector3 GetBottomPos(this Collider2D coll)
    {
        return new Vector3(coll.bounds.extents.x, coll.bounds.min.y, 0);
    }
}