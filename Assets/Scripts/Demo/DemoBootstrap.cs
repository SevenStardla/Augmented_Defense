using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-1000)]
public sealed class DemoBootstrap : MonoBehaviour
{
    private const int EnemyLayer = 8;
    private const int BlockedLayer = 9;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneLoadedCallback()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (FindFirstObjectByType<GameManager>() != null)
        {
            return;
        }

        new GameObject("Augmented Defense Demo").AddComponent<DemoBootstrap>();
    }

    private void Awake()
    {
        Debug.Log("DemoBootstrap is rebuilding the Main scene.");
        BuildScene();
    }

    private void BuildScene()
    {
        Time.timeScale = 1f;

        Camera camera = CreateCamera();
        GameManager gameManager = new GameObject("Game Manager").AddComponent<GameManager>();
        EconomyManager economyManager = new GameObject("Economy Manager").AddComponent<EconomyManager>();
        new GameObject("Runtime Augment Stats").AddComponent<RuntimeAugmentStats>();
        AugmentManager augmentManager = new GameObject("Augment Manager").AddComponent<AugmentManager>();
        augmentManager.Configure(CreateAugments(), 3);

        Transform[] path = CreatePath();
        CoreHealth core = CreateCore(path[path.Length - 1].position);
        EnemyData normalEnemy = CreateEnemyData("Normal Enemy", new Color(0.92f, 0.25f, 0.26f), 1f, 50, 1.1f, 10, 4);
        EnemyData runnerEnemy = CreateEnemyData("Runner Enemy", new Color(1f, 0.62f, 0.16f), 0.78f, 30, 1.85f, 8, 3);
        EnemyData tankEnemy = CreateEnemyData("Tank Enemy", new Color(0.62f, 0.3f, 0.9f), 1.35f, 160, 0.68f, 20, 10);
        Enemy enemyPrefab = CreateEnemyPrefab();
        TowerData towerData = CreateTowerData();
        Tower towerPrefab = CreateTowerPrefab();

        EnemySpawner spawner = new GameObject("Enemy Spawner").AddComponent<EnemySpawner>();
        spawner.Configure(enemyPrefab, normalEnemy, path, core, 0.65f);
        spawner.gameObject.AddComponent<SpawnPointWarningView>();
        spawner.gameObject.AddComponent<WaveSpawnVfx>();

        WaveManager waveManager = new GameObject("Wave Manager").AddComponent<WaveManager>();
        waveManager.Configure(spawner, CreateWaves(normalEnemy, runnerEnemy, tankEnemy));

        TowerPlacement placement = new GameObject("Tower Placement").AddComponent<TowerPlacement>();
        placement.Configure(camera, towerPrefab, towerData, ~0);
        placement.gameObject.AddComponent<TowerPlacementPreview>();

        DefenderController player = CreatePlayer(new Vector3(-4.5f, -2.5f, 0f));
        CreateStartingTower(towerPrefab, towerData, new Vector3(-1.5f, 0.7f, 0f));
        CreateUi(core, waveManager, placement, player, augmentManager);

        gameManager.StartGame();
        economyManager.AddGold(0);
    }

    private Camera CreateCamera()
    {
        Camera camera = Camera.main != null ? Camera.main : FindFirstObjectByType<Camera>();
        GameObject cameraObject = camera != null ? camera.gameObject : new GameObject("Main Camera");
        cameraObject.name = "Main Camera";
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);

        if (camera == null)
        {
            camera = cameraObject.AddComponent<Camera>();
        }

        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.07f, 0.09f, 0.11f);
        camera.orthographic = true;
        camera.orthographicSize = 5.2f;
        return camera;
    }

    private AugmentData[] CreateAugments()
    {
        return new[]
        {
            CreateAugment("tower_damage", "Overcharged Turrets", "Tower damage +25%", AugmentType.Tower, 0.25f, 3),
            CreateAugment("tower_attack_speed", "Rapid Cycling", "Tower attack speed +20%", AugmentType.Tower, 0.2f, 3),
            CreateAugment("tower_range", "Long-Range Optics", "Tower range +18%", AugmentType.Tower, 0.18f, 2),
            CreateAugment("defender_damage", "Defender Calibration", "Defender damage +35%", AugmentType.Status, 0.35f, 3),
            CreateAugment("gold_reward", "Salvage Protocol", "Enemy gold rewards +25%", AugmentType.Economy, 0.25f, 2),
            CreateAugment("core_repair", "Emergency Repair", "Restore 25 Core health", AugmentType.Core, 25f, 3)
        };
    }

    private AugmentData CreateAugment(string id, string displayName, string description, AugmentType type, float value, int maxStacks)
    {
        AugmentData augment = ScriptableObject.CreateInstance<AugmentData>();
        augment.id = id;
        augment.displayName = displayName;
        augment.description = description;
        augment.type = type;
        augment.value = value;
        augment.canStack = true;
        augment.maxStacks = maxStacks;
        return augment;
    }

    private Transform[] CreatePath()
    {
        Vector3[] positions =
        {
            new Vector3(-7f, 2.7f, 0f),
            new Vector3(-3.5f, 2.7f, 0f),
            new Vector3(-3.5f, -0.8f, 0f),
            new Vector3(1.2f, -0.8f, 0f),
            new Vector3(1.2f, 2.1f, 0f),
            new Vector3(5.9f, 2.1f, 0f)
        };

        Transform[] path = new Transform[positions.Length];
        GameObject pathRoot = new GameObject("Enemy Path");

        LineRenderer line = pathRoot.AddComponent<LineRenderer>();
        line.positionCount = positions.Length;
        line.useWorldSpace = true;
        line.startWidth = 0.18f;
        line.endWidth = 0.18f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = new Color(0.85f, 0.67f, 0.28f);
        line.endColor = new Color(0.85f, 0.67f, 0.28f);
        line.SetPositions(positions);
        pathRoot.AddComponent<PathPulseView>();

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject waypoint = new GameObject($"Waypoint {i + 1}");
            waypoint.transform.SetParent(pathRoot.transform);
            waypoint.transform.position = positions[i];
            path[i] = waypoint.transform;
        }

        return path;
    }

    private CoreHealth CreateCore(Vector3 position)
    {
        GameObject coreObject = CreateSpriteObject("Core", position, new Color(0.21f, 0.76f, 0.91f), new Vector3(0.9f, 0.9f, 1f));
        coreObject.layer = BlockedLayer;
        coreObject.AddComponent<BoxCollider2D>();
        CoreHealth coreHealth = coreObject.AddComponent<CoreHealth>();
        coreObject.AddComponent<CoreViewAnimator>();
        return coreHealth;
    }

    private DefenderController CreatePlayer(Vector3 position)
    {
        GameObject player = CreateSpriteObject("Player Defender", position, new Color(0.28f, 0.88f, 0.52f), new Vector3(0.55f, 0.55f, 1f));
        player.AddComponent<CircleCollider2D>();
        DefenderController controller = player.AddComponent<DefenderController>();
        controller.Configure(5.5f, 3.4f, 10f, 0.38f, ~0);
        player.AddComponent<DefenderViewAnimator>();
        player.AddComponent<DefenderAttackVfx>();
        return controller;
    }

    private EnemyData CreateEnemyData(string displayName, Color color, float sizeMultiplier, int maxHealth, float moveSpeed, int coreDamage, int goldReward)
    {
        EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
        data.displayName = displayName;
        data.displayColor = color;
        data.sizeMultiplier = sizeMultiplier;
        data.maxHealth = maxHealth;
        data.moveSpeed = moveSpeed;
        data.coreDamage = coreDamage;
        data.goldReward = goldReward;
        return data;
    }

    private TowerData CreateTowerData()
    {
        TowerData data = ScriptableObject.CreateInstance<TowerData>();
        data.type = TowerType.Basic;
        data.cost = 70;
        data.damage = 10f;
        data.attackInterval = 0.65f;
        data.range = 2.8f;
        return data;
    }

    private WaveData[] CreateWaves(EnemyData normal, EnemyData runner, EnemyData tank)
    {
        return new[]
        {
            CreateWave(CreateWaveEntry(normal, 6, 0.7f)),
            CreateWave(CreateWaveEntry(normal, 6, 0.65f), CreateWaveEntry(runner, 4, 0.45f)),
            CreateWave(CreateWaveEntry(normal, 8, 0.6f), CreateWaveEntry(runner, 6, 0.4f)),
            CreateWave(CreateWaveEntry(tank, 3, 1f), CreateWaveEntry(runner, 6, 0.38f), CreateWaveEntry(normal, 6, 0.55f)),
            CreateWave(CreateWaveEntry(tank, 5, 0.9f), CreateWaveEntry(normal, 10, 0.5f), CreateWaveEntry(runner, 8, 0.34f))
        };
    }

    private WaveData CreateWave(params WaveEntry[] entries)
    {
        WaveData wave = ScriptableObject.CreateInstance<WaveData>();
        wave.entries = entries;
        return wave;
    }

    private WaveEntry CreateWaveEntry(EnemyData enemyData, int count, float spawnInterval)
    {
        return new WaveEntry
        {
            enemyData = enemyData,
            count = count,
            spawnInterval = spawnInterval
        };
    }

    private Enemy CreateEnemyPrefab()
    {
        GameObject prefab = CreateSpriteObject("Enemy Prefab", Vector3.zero, new Color(0.92f, 0.25f, 0.26f), new Vector3(0.45f, 0.45f, 1f));
        prefab.SetActive(false);
        prefab.layer = EnemyLayer;
        prefab.AddComponent<CircleCollider2D>();
        prefab.AddComponent<EnemyMovement>();
        Enemy enemy = prefab.AddComponent<Enemy>();
        prefab.AddComponent<EnemyViewAnimator>();
        prefab.AddComponent<EnemyHealthBar>();
        return enemy;
    }

    private Tower CreateTowerPrefab()
    {
        GameObject prefab = CreateSpriteObject("Tower Prefab", Vector3.zero, new Color(0.97f, 0.77f, 0.24f), new Vector3(0.6f, 0.6f, 1f));
        prefab.SetActive(false);
        prefab.layer = BlockedLayer;
        prefab.AddComponent<BoxCollider2D>();
        Tower tower = prefab.AddComponent<Tower>();
        if (prefab.GetComponent<TowerAttack>() == null)
        {
            prefab.AddComponent<TowerAttack>();
        }
        prefab.AddComponent<TowerViewAnimator>();
        prefab.AddComponent<TowerTargetLine>();
        prefab.AddComponent<TowerRangePreview>();
        return tower;
    }

    private void CreateStartingTower(Tower towerPrefab, TowerData towerData, Vector3 position)
    {
        Tower tower = Instantiate(towerPrefab, position, Quaternion.identity);
        tower.gameObject.name = "Starting Tower";
        tower.gameObject.SetActive(true);
        tower.Initialize(towerData);
    }

    private void CreateUi(CoreHealth core, WaveManager waveManager, TowerPlacement placement, DefenderController player, AugmentManager augmentManager)
    {
        EnsureEventSystem();

        Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        Text title = CreateText(canvas.transform, "Title", "Augmented Defense - Play Test", 24, TextAnchor.UpperLeft, new Vector2(18f, -14f), new Vector2(460f, 36f));
        title.color = new Color(0.92f, 0.95f, 0.97f);
        title.gameObject.AddComponent<UITextFeedback>().CaptureBaseState();

        Text coreText = CreateText(canvas.transform, "Core Text", "Core", 18, TextAnchor.UpperLeft, new Vector2(18f, -54f), new Vector2(180f, 28f));
        Text goldText = CreateText(canvas.transform, "Gold Text", "Gold", 18, TextAnchor.UpperLeft, new Vector2(18f, -84f), new Vector2(180f, 28f));
        Text waveText = CreateText(canvas.transform, "Wave Text", "Wave", 18, TextAnchor.UpperLeft, new Vector2(18f, -114f), new Vector2(180f, 28f));
        coreText.gameObject.AddComponent<UITextFeedback>();
        goldText.gameObject.AddComponent<UITextFeedback>();
        waveText.gameObject.AddComponent<UITextFeedback>();
        Text hintText = CreateText(canvas.transform, "Hint Text", "WASD move   Space shoot   Left click place tower", 16, TextAnchor.LowerLeft, new Vector2(18f, 18f), new Vector2(520f, 30f));
        hintText.gameObject.AddComponent<UITextFeedback>();
        hintText.gameObject.AddComponent<UIHintFeedback>().Configure(player, placement);

        Button waveButton = CreateButton(canvas.transform, "Start Wave Button", "Start Wave", new Vector2(-118f, -24f), new Vector2(140f, 38f));
        waveButton.onClick.AddListener(waveManager.StartNextWave);
        waveButton.gameObject.AddComponent<UIButtonStateFeedback>().Configure(true, true);

        Button restartButton = CreateButton(canvas.transform, "Restart Button", "Restart", new Vector2(-118f, -68f), new Vector2(140f, 38f));
        restartButton.gameObject.AddComponent<MainSceneRestarter>();
        restartButton.gameObject.AddComponent<UIButtonStateFeedback>().Configure(false, false);

        GameObject gameOverPanel = CreatePanel(canvas.transform, "Game Over Panel", new Color(0f, 0f, 0f, 0.72f), Vector2.zero, new Vector2(360f, 150f));
        CenterPanel(gameOverPanel);
        gameOverPanel.AddComponent<UIPanelFeedback>();
        CreateText(gameOverPanel.transform, "Game Over Text", "GAME OVER", 30, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(320f, 80f));
        gameOverPanel.SetActive(false);

        GameObject clearPanel = CreatePanel(canvas.transform, "Clear Panel", new Color(0.03f, 0.14f, 0.12f, 0.9f), Vector2.zero, new Vector2(400f, 190f));
        CenterPanel(clearPanel);
        clearPanel.AddComponent<UIPanelFeedback>();
        CreateText(clearPanel.transform, "Clear Text", "SYSTEM SECURED\nALL WAVES CLEARED", 28, TextAnchor.MiddleCenter, new Vector2(0f, 30f), new Vector2(360f, 90f));
        Button clearRestartButton = CreateButton(clearPanel.transform, "Clear Restart Button", "Play Again", new Vector2(-130f, -130f), new Vector2(140f, 38f));
        clearRestartButton.gameObject.AddComponent<MainSceneRestarter>();
        clearPanel.SetActive(false);

        CreateAugmentUi(canvas.transform, augmentManager);

        UIManager uiManager = new GameObject("UI Manager").AddComponent<UIManager>();
        uiManager.Configure(coreText, goldText, waveText, gameOverPanel, clearPanel, core, waveManager);
        placement.PlacementFailed += _ => uiManager.NotifyPlacementFailed();
    }

    private void CreateAugmentUi(Transform canvas, AugmentManager augmentManager)
    {
        GameObject panel = CreatePanel(canvas, "Augment Selection Panel", new Color(0.04f, 0.07f, 0.11f, 0.96f), Vector2.zero, new Vector2(760f, 360f));
        CenterPanel(panel);
        CreateText(panel.transform, "Augment Title", "CHOOSE AN AUGMENT", 27, TextAnchor.MiddleCenter, new Vector2(-380f, -20f), new Vector2(760f, 50f));

        Button[] buttons = new Button[3];
        Text[] labels = new Text[3];
        for (int i = 0; i < buttons.Length; i++)
        {
            float x = -50f - i * 235f;
            buttons[i] = CreateButton(panel.transform, $"Augment Choice {i + 1}", "Augment", new Vector2(x, -95f), new Vector2(215f, 210f));
            labels[i] = buttons[i].GetComponentInChildren<Text>();
            labels[i].fontSize = 17;
        }

        AugmentSelectionUI selectionUi = new GameObject("Augment Selection UI").AddComponent<AugmentSelectionUI>();
        selectionUi.Configure(panel, augmentManager, buttons, labels);
    }

    private void CenterPanel(GameObject panel)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }

    private Text CreateText(Transform parent, string name, string value, int size, TextAnchor anchor, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = size;
        text.alignment = anchor;
        text.color = Color.white;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = AnchorFor(anchor);
        rect.anchorMax = AnchorFor(anchor);
        rect.pivot = PivotFor(anchor);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
        return text;
    }

    private Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject buttonObject = CreatePanel(parent, name, new Color(0.15f, 0.25f, 0.32f, 0.95f), anchoredPosition, sizeDelta);
        Button button = buttonObject.AddComponent<Button>();
        Text text = CreateText(buttonObject.transform, "Label", label, 16, TextAnchor.MiddleCenter, Vector2.zero, sizeDelta);
        button.targetGraphic = buttonObject.GetComponent<Image>();
        button.colors = ColorBlock.defaultColorBlock;
        text.color = Color.white;
        return button;
    }

    private GameObject CreatePanel(Transform parent, string name, Color color, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        Image image = panel.AddComponent<Image>();
        image.color = color;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
        return panel;
    }

    private GameObject CreateSpriteObject(string name, Vector3 position, Color color, Vector3 scale)
    {
        GameObject obj = new GameObject(name);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateSprite();
        renderer.color = color;
        return obj;
    }

    private Sprite CreateSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }

    private Vector2 AnchorFor(TextAnchor anchor)
    {
        if (anchor == TextAnchor.LowerLeft)
        {
            return new Vector2(0f, 0f);
        }

        if (anchor == TextAnchor.MiddleCenter)
        {
            return new Vector2(0.5f, 0.5f);
        }

        return new Vector2(0f, 1f);
    }

    private Vector2 PivotFor(TextAnchor anchor)
    {
        if (anchor == TextAnchor.LowerLeft)
        {
            return new Vector2(0f, 0f);
        }

        if (anchor == TextAnchor.MiddleCenter)
        {
            return new Vector2(0.5f, 0.5f);
        }

        return new Vector2(0f, 1f);
    }

}
