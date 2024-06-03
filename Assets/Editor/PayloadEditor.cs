using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(Payload))]
public class PayloadEditor : Editor
{
    private Payload payloadTarget;
    
    private void OnSceneGUI()
    {
        payloadTarget = (Payload)target;
        
        Handles.color = Color.black;

        var positions = payloadTarget.displayPayloadPath;

        //Handles.DrawLine(payloadTarget.transform.position, positions[0], 15f);

        for (int i = 1; i < positions.Count; i++)
        {
            
            var previousPosition = positions[i - 1];
            var currentPosition = positions[i];

            Handles.DrawLine(previousPosition, currentPosition, 15f);
        }

        //Handles.DrawPolyLine(positions.ToArray());

        for (int i = 0; i < positions.Count; i++)
        {
            positions[i] = Handles.PositionHandle(positions[i], Quaternion.identity);
        }
    }
}
