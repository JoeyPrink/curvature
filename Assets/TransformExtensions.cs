using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class TransformExtensions
{
    public static Transform FindDeep(this Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindDeep(aName);
            if (result != null)
                return result;
        }
        return null;
    }

    public static C FindDeepComponent<C>(this Transform t, string name) where C : Component
    {
        return t.FindDeep(name).GetComponent<C>();
    }
}
