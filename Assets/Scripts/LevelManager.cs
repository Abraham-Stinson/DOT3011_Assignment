using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private string[] museumArtifactsLevelName;
    [SerializeField] private Material cursedMaterial;
    private bool isMuseumArtifactsCursed = false;
    [Header("Game Mechanic")]
    private Vector3 savedPlayerReturnPosition;
    [SerializeField] public bool isPlayerGetFirstWin = false;

    [SerializeField] private string currentLevelName;
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
                    parents.transform.GetChild(selectedArtifact).GetComponent<LevelObjectInteraction>().AppendLevelName(museumArtifactsLevelName[0]);
                    break;
                case "PaintParent":
                    parents.transform.GetChild(selectedArtifact).GetComponent<LevelObjectInteraction>().AppendLevelName(museumArtifactsLevelName[1]);
                    break;
                default:
                    Debug.LogWarning("Spesifik bir item belirltmedi");
                    break;
            }

        }
    }
    public void ModifyFormerPositionForReturn(Vector3 playersPosition)
    {
        savedPlayerReturnPosition = playersPosition;
        Debug.Log($"Player's former postion saved: {savedPlayerReturnPosition}");

    }

    public void ReturnFromLevel()
    {
        var player = ThirdPersonController.instance;
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        player.transform.position = savedPlayerReturnPosition;

        if (cc != null)
        {
            cc.enabled = true;
        }
    }
    public void ModifyCurrentLevelName(string loadToScene)
    {
        currentLevelName = loadToScene;
    }

    public void DestroyCurrentLevel()
    {
        SceneManager.UnloadSceneAsync(currentLevelName);
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
