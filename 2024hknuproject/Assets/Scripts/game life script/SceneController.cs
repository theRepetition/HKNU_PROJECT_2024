using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("main");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "main")
        {
            InitializeMainScene();
        }
    }

    private void InitializeMainScene()
    {
        Debug.Log("메인 씬 초기화 중...");

        // 예: 스테이지 초기화 및 NPC 배치 등
        CycleManager.currentStage = 1;
        CycleManager.Instance.SpawnNPCs();

        // 기타 필요한 초기화 작업 수행
        ResetPlayerState();
        ResetUI();
    }

    private void ResetPlayerState()
    {
        // 플레이어 상태 초기화 코드
    }

    private void ResetUI()
    {
        // UI 초기화 코드
    }
}
