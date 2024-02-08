using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(FollowPath))]
public class FollowPathEditor : Editor
{
    private FollowPath targetComponent;

    private void OnSceneGUI()
    {
        targetComponent = (FollowPath)target;

        Handles.color = Color.cyan;

        var positions = targetComponent.path;

        for (int i = 1; i < positions.Length + 1; ++i)
        {
            var previousPoint = positions[i - 1];
            var currentPoint = positions[i % positions.Length];

            Handles.DrawDottedLine(p1: previousPoint, p2: currentPoint, screenSpaceSize: 4f);
        }


        for (int i = 0; i < positions.Length; ++i)
        {
            positions[i] = Handles.PositionHandle(positions[i], Quaternion.identity);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(targetComponent);
            EditorSceneManager.MarkSceneDirty(targetComponent.gameObject.scene);
        }
    }
}
