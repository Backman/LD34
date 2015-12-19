using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public Action<TreeOfLife, int, int> OnNextTreeSegment;
    public Action OnTreeStartGrowth;
    public Action OnTreeStopGrowth;
    public Action<float, float> OnTreeDamage;
    public Action OnTreeDeath;
    public Action<int, int> OnAddAmmo;
    public Action<int, int> OnRemoveAmmo;
    public Action<int> OnSpawnWater;
    public Action<QuestObjective> OnObjectiveStart;
    public Action<QuestObjective> OnObjectiveEnd;
    public Action OnWaterTree;

    public BotSpawnSettings StartSpawnSettings;
    public bool SpawnBotsOnStart;
    public BotSpawning BotSpawning;
    public WaterSpawing WaterSpawing;
    public DialogSystem Dialog = new DialogSystem();

    public QuestObjectives Objectives;

    private HUD _hud;

    private TreeOfLife _tree;

    public QuestObjective CurrentObjective { get { return Objectives.CurrentObjective; } }

    private const int _tryToSpawnWaterAmount = 4;

    void Awake()
    {
        BotSpawning.Awake();
        WaterSpawing.Awake();
    }

    private void Start()
    {
        _tree = FindObjectOfType<TreeOfLife>();
        _hud = FindObjectOfType<HUD>();
        OnNextTreeSegment += NextTreeSegment;
        OnTreeDamage += TreeDamage;
        OnTreeDeath += TreeDeath;
        OnSpawnWater += SpawnWater;
        OnAddAmmo += AddAmmo;
        OnObjectiveStart += ObjectiveStart;
        OnObjectiveEnd += ObjectiveEnd;

        if (SpawnBotsOnStart && StartSpawnSettings)
        {
            BotSpawning.StartSpawn(StartSpawnSettings);
        }

        StartCoroutine(BigAssQuest());
    }

    private void Update()
    {
        BotSpawning.Update();
        Dialog.Update();
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.O))
                Time.timeScale += 1.2f;
            if (Input.GetKeyDown(KeyCode.P))
                Time.timeScale *= 0.8f;
            if (Input.GetKeyDown(KeyCode.I))
                Time.timeScale = 1f;
        }
    }

    public void SpawnWater(int amount)
    {
        WaterSpawing.SpawnWater(amount);
    }

    public void TrySpawnWater()
    {
        WaterSpawing.TrySpawnWater(_tryToSpawnWaterAmount);
    }

    private void ObjectiveStart(QuestObjective objective)
    {
        
    }

    private void ObjectiveEnd(QuestObjective objective)
    {
    }

    private void NextTreeSegment(TreeOfLife tree, int previousSegment, int nextSegment)
    {
        
    }

    private void TreeDamage(float amount, float curHealth)
    {
        
    }

    private void TreeDeath()
    {
        MenuScreenController.Instance.LoseScreen();
    }

    private void AddAmmo(int amount, int curAmount)
    {
        if (CurrentObjective != null && CurrentObjective is WaterObjective)
        {
            var waterObjective = (WaterObjective)CurrentObjective;
            waterObjective.CurrentWaterAmount += amount;
        }
    }

    private IEnumerator BigAssQuest()
    {
        Debug.Log("Start BIG ASS QUEST!");
        Objectives.Initialize();
        while (!Objectives.IsDone())
        {
            Objectives.Update();
            yield return null;
        }
        Debug.Log("Big ass quest is done!");
        MenuScreenController.Instance.WinScreen();
    }
}