using UnityEngine;
using UnityEditor;


public class ResetGuards : EditorWindow
{
    private string GuardTag = "Guard";

    [MenuItem("Tools/Reset Guards")]
    public static void ShowWindow()
    {
        GetWindow<ResetGuards>("Reset Guards");
    }
    private void OnGUI()
    {
        GUILayout.Label("Reset All Guards", EditorStyles.boldLabel);
        GuardTag = EditorGUILayout.TextField("Guard Tag", GuardTag);
        GUILayout.Space(10);

        if (GUILayout.Button("Reset Guards"))
        {
            ResetAllGuards();
        }
    }
    private void ResetAllGuards()
    {
        GameObject[] TaggedObjects = GameObject.FindGameObjectsWithTag(GuardTag); //Any guard tagged with "guard" will be automatically added to this array
        
        foreach (GameObject obj in TaggedObjects)
        {
            EnemyAI Guard = obj.GetComponent<EnemyAI>();
            if ( Guard != null )
            {
                Guard.ResetGuard();//Calling the reset function
            }
        }
        Debug.Log("Guards tagged with Guard have been reset");
    }

}