using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.black;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.catchRange);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngel / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngel / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.catchRange);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.catchRange);
    }

}
