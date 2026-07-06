using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(-1000)]
public sealed class DemoBootstrap : MonoBehaviour
{
    private const int EnemyLayer = 8;
    private const int BlockedLayer = 9;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateForEmptyScene()
    {
        if (FindFirstObjectByType<GameManager>() != null)
        {
            return;
        }

        new GameObject("Augmented Defense Demo").AddComponent<DemoBootstrap>();
    }

    private void Awake()
    {
        BuildScene();
    }

    private void BuildScene()
    {
        Time.timeScale = 1f;

        Camera camera = CreateCamera();
        GameManager gameManager = new GameObject("Game Manager").AddComponent<GameManager>();
        EconomyManager economyManager = new GameObject("Economy Manager").AddComponent<EconomyManager>();

        Transform[] path = CreatePath();
        CoreHealth core = CreateCore(path[path.Length - 1].position);
        EnemyData enemyData = CreateEnemyData();
        Enemy enemyPrefab = CreateEnemyPrefab();
        TowerData towerData = CreateTowerData();
        Tower towerPrefab = CreateTowerPrefab();

        EnemySpawner spawner = new GameObject("Enemy Spawner").AddComponent<EnemySpawner>();
        spawner.Configure(enemyPrefab, enemyData, path, core, 0.65f);

        WaveManager waveManager = new GameObject("Wave Manager").AddComponent<WaveManager>();
        waveManager.Configure(spawner, CreateWaves(enemyData));

        TowerPlacement placement = new GameObject("Tower Placement").AddComponent<TowerPlacement>();
        placement.Configure(camera, towerPrefab, towerData, ~0);

        CreatePlayer(new Vector3(-4.5f, -2.5f, 0f));
        CreateStartingTower(towerPrefab, towerData, new Vector3(-1.5f, 0.7f, 0f));
        CreateUi(core, waveManager);

        gameManager.StartGame();
        economyManager.AddGold(0);
    }

    private Camera CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.07f, 0.09f, 0.11f);
        camera.orthographic = true;
        camera.orthographicSize = 5.2f;
        return camera;
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
        return coreObject.AddComponent<CoreHealth>();
    }

    private void CreatePlayer(Vector3 position)
    {
        GameObject player = CreateSpriteObject("Player Defender", position, new Color(0.28f, 0.88f, 0.52f), new Vector3(0.55f, 0.55f, 1f));
        player.AddComponent<CircleCollider2D>();
        player.AddComponent<DefenderController>().Configure(5.5f, 3.4f, 14f, 0.28f, ~0);
    }

    private EnemyData CreateEnemyData()
    {
        EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
        data.maxHealth = 35;
        data.moveSpeed = 1.1f;
        data.coreDamage = 10;
        data.goldReward = 7;
        return data;
    }

    private TowerData CreateTowerData()
    {
        TowerData data = ScriptableObject.CreateInstance<TowerData>();
        data.type = TowerType.Basic;
        data.cost = 50;
        data.damage = 11f;
        data.attackInterval = 0.55f;
        data.range = 2.8f;
        return data;
    }

    private WaveData[] CreateWaves(EnemyData enemyData)
    {
        WaveData[] waves = new WaveData[3];
        int[] counts = { 6, 9, 12 };

        for (int i = 0; i < waves.Length; i++)
        {
            WaveData wave = ScriptableObject.CreateInstance<WaveData>();
            wave.enemyCount = counts[i];
            wave.enemyData = enemyData;
            waves[i] = wave;
        }

        return waves;
    }

    private Enemy CreateEnemyPrefab()
    {
        GameObject prefab = CreateSpriteObject("Enemy Prefab", Vector3.zero, new Color(0.92f, 0.25f, 0.26f), new Vector3(0.45f, 0.45f, 1f));
        prefab.layer = EnemyLayer;
        prefab.AddComponent<CircleCollider2D>();
        prefab.AddComponent<EnemyMovement>();
        Enemy enemy = prefab.AddComponent<Enemy>();
        prefab.SetActive(false);
        return enemy;
    }

    private Tower CreateTowerPrefab()
    {
        GameObject prefab = CreateSpriteObject("Tower Prefab", Vector3.zero, new Color(0.97f, 0.77f, 0.24f), new Vector3(0.6f, 0.6f, 1f));
        prefab.layer = BlockedLayer;
        prefab.AddComponent<BoxCollider2D>();
        Tower tower = prefab.AddComponent<Tower>();
        prefab.AddComponent<TowerAttack>();
        prefab.SetActive(false);
        return tower;
    }

    private void CreateStartingTower(Tower towerPrefab, TowerData towerData, Vector3 position)
    {
        Tower tower = Instantiate(towerPrefab, position, Quaternion.identity);
        tower.gameObject.name = "Starting Tower";
        tower.gameObject.SetActive(true);
        tower.Initialize(towerData);
    }

    private void CreateUi(CoreHealth core, WaveManager waveManager)
    {
        EnsureEventSystem();

        Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        Text title = CreateText(canvas.transform, "Title", "Augmented Defense - Play Test", 24, TextAnchor.UpperLeft, new Vector2(18f, -14f), new Vector2(460f, 36f));
        title.color = new Color(0.92f, 0.95f, 0.97f);

        Text coreText = CreateText(canvas.transform, "Core Text", "Core", 18, TextAnchor.UpperLeft, new Vector2(18f, -54f), new Vector2(180f, 28f));
        Text goldText = CreateText(canvas.transform, "Gold Text", "Gold", 18, TextAnchor.UpperLeft, new Vector2(18f, -84f), new Vector2(180f, 28f));
        Text waveText = CreateText(canvas.transform, "Wave Text", "Wave", 18, TextAnchor.UpperLeft, new Vector2(18f, -114f), new Vector2(180f, 28f));
        CreateText(canvas.transform, "Hint Text", "WASD move   Space shoot   Left click place tower", 16, TextAnchor.LowerLeft, new Vector2(18f, 18f), new Vector2(520f, 30f));

        Button waveButton = CreateButton(canvas.transform, "Start Wave Button", "Start Wave", new Vector2(-118f, -24f), new Vector2(140f, 38f));
        waveButton.onClick.AddListener(waveManager.StartNextWave);

        Button restartButton = CreateButton(canvas.transform, "Restart Button", "Restart", new Vector2(-118f, -68f), new Vector2(140f, 38f));
        restartButton.onClick.AddListener(RestartDemo);

        GameObject gameOverPanel = CreatePanel(canvas.transform, "Game Over Panel", new Color(0f, 0f, 0f, 0.72f), Vector2.zero, new Vector2(360f, 150f));
        CreateText(gameOverPanel.transform, "Game Over Text", "GAME OVER", 30, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(320f, 80f));
        gameOverPanel.SetActive(false);

        UIManager uiManager = new GameObject("UI Manager").AddComponent<UIManager>();
        uiManager.Configure(coreText, goldText, waveText, gameOverPanel, core, waveManager);
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

    private void RestartDemo()
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene.buildIndex >= 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(activeScene.buildIndex);
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(activeScene.name);
    }
}
