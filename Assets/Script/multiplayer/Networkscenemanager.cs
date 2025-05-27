using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class networkscenemanager : NetworkBehaviour
{
#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
    private void OnValidate()
    {
        if (SceneAsset != null)
        {
            m_SceneName = SceneAsset.name;
        }
    }
#endif

    [SerializeField]
    private string m_SceneName;

    [Header("Scene Control")]
    [SerializeField] private bool randomizeScenes = true;
    [SerializeField] private string creditSceneName = "Credits";

    [Header("UI Control")]
    public GameObject controlUI;
    public bool destroyUIOnStart = true;

    private List<string> sceneOrder = new List<string>();
    private int currentSceneIndex = 0;

    // Optional singleton access
    public static networkscenemanager Instance;

    private void Awake()
    {
        if (IsServer)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (IsServer)
        {
            PrepareScenes();
            // LoadNextScene();
        }
    }

    private void PrepareScenes()
    {
        sceneOrder.Clear();

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        List<string> gameScenes = new List<string>();
        bool creditExists = false;

        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            if (name.ToLower().Contains("game") && name != creditSceneName)
            {
                gameScenes.Add(name);
            }

            if (name == creditSceneName)
            {
                creditExists = true;
            }
        }

        if (randomizeScenes)
        {
            for (int i = 0; i < gameScenes.Count; i++)
            {
                int randIndex = Random.Range(i, gameScenes.Count);
                (gameScenes[i], gameScenes[randIndex]) = (gameScenes[randIndex], gameScenes[i]);
            }
        }

        sceneOrder.AddRange(gameScenes);

        if (creditExists)
        {
            sceneOrder.Add(creditSceneName);
        }

        currentSceneIndex = 0;

        if (controlUI != null)
        {
            if (destroyUIOnStart)
                Destroy(controlUI);
            else
                controlUI.SetActive(false);
        }
    }

    public void TryLoadNextSceneFromTrigger()
    {
        if (!IsServer) return;
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        if (!IsServer) return;

        if (currentSceneIndex < sceneOrder.Count)
        {
            string nextSceneName = sceneOrder[currentSceneIndex];
            currentSceneIndex++;
            NetworkManager.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
    }

    public void LoadRandomScene()
    {
        if (!IsServer) return;

        PrepareScenes();
        LoadNextScene();
    }
}
