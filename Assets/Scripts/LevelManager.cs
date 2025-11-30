using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    [Header("Tree Settings")]
    [SerializeField] private GameObject treeGO;
    [SerializeField] private Transform treeSpawnTransform;
    private GameObject tree;
    [Header("Museum Artifacts")]
    [SerializeField] private GameObject[] museumArtifacts;
    [SerializeField] private Material cursedMaterial;
    private bool isMuseumArtifactsCursed = false;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void InstantiateTree()
    {
        if (tree != null) return;

        //Debug.Log("InstantiateTree(): Agac olusturuluyor");
        tree = Instantiate(treeGO, treeSpawnTransform);
    }
    public void ArtifectsInMuseum()
    {
        isMuseumArtifactsCursed = true;
        Debug.LogWarning("eserlerde bir gariplik var");
        foreach (GameObject parents in museumArtifacts)
        {
            Debug.Log(parents.transform.childCount);
            int childCount = parents.transform.childCount;
            int selectedArtifact = Random.Range(0, childCount);
            //Debug.Log($" {parents.name} iteminin secilen {parents.transform.GetChild(selectedArtifact).name}");
            Debug.Log($"Random result: {selectedArtifact} name: {parents.transform.GetChild(selectedArtifact).name}");
            parents.transform.GetChild(selectedArtifact).AddComponent<LevelObjectInteraction>();
            parents.transform.GetChild(selectedArtifact).GetComponent<LevelObjectInteraction>().ChangeMaterialOfArtifact(cursedMaterial);

            switch (parents.name)
            {
                case "SculpParent":
                    parents.transform.GetChild(selectedArtifact).GetComponent<LevelObjectInteraction>().AppendLevelName("sculptLevel");
                    break;
                case "PaintParent":
                    parents.transform.GetChild(selectedArtifact).GetComponent<LevelObjectInteraction>().AppendLevelName("paintLevel");
                    break;
                default:
                    Debug.LogWarning("Spesifik bir item belirltmedi");
                    break;
            }

        }
    }


    public bool isMuseumArtifactsCursedCheck()
    {
        return isMuseumArtifactsCursed;
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
