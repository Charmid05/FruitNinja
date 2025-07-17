using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
   [SerializeField] private TMP_Text levelText;
[SerializeField] private TMP_Text titleText;

    [SerializeField] private Image fadeImage;

    public int Score { get; private set; } = 0;
    public int Level { get; private set; } = 1;

    private void Awake()
    {
        HandleSingleton();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void HandleSingleton()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void NewGame()
    {
        Time.timeScale = 1f;

        ClearScene();

        blade.enabled = true;
        spawner.enabled = true;

        Score = 0;
        Level = 1;
        scoreText.text = Score.ToString();
        levelText.text = "Level 1";
        titleText.text = GetTitleForLevel(Level);
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();
        foreach (Fruit fruit in fruits)
        {
            Destroy(fruit.gameObject);
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();
        foreach (Bomb bomb in bombs)
        {
            Destroy(bomb.gameObject);
        }
    }

    public void IncreaseScore(int points)
    {
        Score += points;
        scoreText.text = Score.ToString();

        UpdateLevel();

        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);
        if (Score > hiscore)
        {
            PlayerPrefs.SetFloat("hiscore", Score);
        }
    }

    private void UpdateLevel()
    {
        Level = Mathf.Clamp((Score / 50) + 1, 1, 50);
        levelText.text = "Level " + Level.ToString();
        titleText.text = GetTitleForLevel(Level);
    }

    private string GetTitleForLevel(int level)
    {
        if (level <= 5) return "Beginner";
        else if (level <= 15) return "Ninja";
        else if (level <= 30) return "Master";
        else return "Legend";
    }

    public void Explode()
    {
        blade.enabled = false;
        spawner.enabled = false;

        StartCoroutine(ExplodeSequence());
    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);

            Time.timeScale = 1f - t;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        NewGame();

        elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }
    }
}
