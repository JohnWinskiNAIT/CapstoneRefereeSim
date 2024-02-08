using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.VisualScripting;

[CustomEditor(typeof(FollowPath))]

//Script is designed to make features available in the editor so we can easily make paths for our hockey players to follow.
public class FollowPathEditor : Editor
{
    //script follows the path 
    private FollowPath targetComponent;

    private void OnSceneGUI()
    {
        targetComponent = (FollowPath)target;

        Handles.color = Color.red;

        var positions = targetComponent.path;

        //for each item in the list, draw a dotted line between them.
        for (int i = 1; i < positions.Length + 1; ++i)
        {
            //first point
            var previousPoint = positions[i - 1].waypointPosition;
            //second point
            var currentPoint = positions[i % positions.Length].waypointPosition;
            //drawing the line between the points
            Handles.DrawDottedLine(p1: previousPoint, p2: currentPoint, screenSpaceSize: 4f);
        }

        //create position handles on the points so you can move them
        for (int i = 0; i < positions.Length; ++i)
        {
            
            positions[i].waypointPosition = Handles.PositionHandle(positions[i].waypointPosition, Quaternion.identity);
        }
        //when you move a point, make unity saveable
        if (GUI.changed)
        {
            EditorUtility.SetDirty(targetComponent);
            EditorSceneManager.MarkSceneDirty(targetComponent.gameObject.scene);
        }
    }
}
