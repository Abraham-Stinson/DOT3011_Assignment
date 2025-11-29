using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class LevelManager : MonoBehaviour
{
    [Header("Tree Settings")]
    [SerializeField] private GameObject treeGO;
    [SerializeField] private Transform treeSpawnTransform;

    private GameObject tree;
    public void InstantiateTree()
    {
        if (tree != null) return;

        Debug.Log("InstantiateTree(): Agac olusturuluyor");
        tree = Instantiate(treeGO, treeSpawnTransform);
    }
}

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelManager script = (LevelManager)target;

        GUILayout.Space(10);

        GUI.enabled = Application.isPlaying;



        if (GUILayout.Button("Create Tree"))
        {
            script.InstantiateTree();
        }
    }
}
