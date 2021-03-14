using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JaysUnityUtils
{
    public static class JUtils
    {
        // courtesy of http://answers.unity.com/answers/893984/view.html
        public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
        {
            Transform t = parent.transform;
            foreach (Transform tr in t)
            {
                if (tr.tag == tag)
                {
                    return tr.GetComponent<T>();
                }
            }
            return null;
        }
    }
}
