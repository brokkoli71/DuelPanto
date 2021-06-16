using System;
using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using DualPantoFramework;
public class GameManager : MonoBehaviour
{

    public bool introduceGame = true;
    public GameObject player;
    public GameObject enemyOriginal;
    public GameObject enemyPrefab;
    private List<GameObject> enemies;

    public EnemyConfig[] enemyConfigs;
    public Transform playerSpawn;
    public Transform[] enemySpawn;
    public int level = 0;
    public int trophyScore = 10000;
    public UIManager uiManager;

    public AudioClip defaultClip;
    public AudioClip wallClip;
    public AudioClip hitClip;
    public AudioClip heartbeatClip;
    public AudioClip[] enemyDyingClips;
    private AudioSource _audioSource;

    private UpperHandle _upperHandle;
    private LowerHandle _lowerHandle;
    private SpeechIn _speechIn;
    private SpeechOut _speechOut;
    private GameObject oldEnemy = null;
    private int _playerScore = 0;
    private int _enemyScore = 0;
    private int _gameScore = 0;
    private float _totalTime = 0;
    private float _levelStartTime = 0;
    public bool gameRunning = false;
    public bool playWithEnemy = true;


    private bool enemyChecking = true;
    private int defeatedEnemies = 0;
    private readonly Dictionary<string, KeyCode> _commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D }
    };

    void Awake()
    {
        // Ensure these are disabled at the start of the game.
        player.SetActive(false);
        enemies = new List<GameObject>();

        spawnEnemy(enemySpawn[0].position, enemySpawn[0].rotation);
        spawnEnemy(enemySpawn[1].position, enemySpawn[1].rotation);

        _speechIn = new SpeechIn(onRecognized, _commands.Keys.ToArray());
        _speechOut = new SpeechOut();

        //if (level < 0 || level >= enemyConfigs.Length)
        //{
        //    Debug.LogWarning($"Level value {level} < 0 or >= enemyConfigs.Length. Resetting to 0");

        //   level = 0;
        //}
    }

    void Start()
    {
        _upperHandle = GetComponent<UpperHandle>();
        _lowerHandle = GetComponent<LowerHandle>();
        _audioSource = player.GetComponent<AudioSource>();

        uiManager.UpdateUI(_playerScore, _enemyScore, _gameScore);

        Introduction();
    }

    async void Introduction()
    {
        //await _speechOut.Speak("Welcome to Duel Panto");
        //await Task.Delay(1000);
        RegisterColliders();

        if (introduceGame)
        {
            //await IntroducePlayers();
            await IntroduceLaser();
            await IntroduceHealth();
            await IntroduceLevel();
        }

        await _speechOut.Speak("Introduction finished, game starts.");

        await ResetRound();
    }
    /*
    async Task IntroducePlayers()
    {
        await _speechOut.Speak("This is you.");
        await _upperHandle.MoveToPosition(playerSpawn.position, 5f);
        _upperHandle.Freeze();

        await _speechOut.Speak("This is your enemy.");
        await _lowerHandle.MoveToPosition(enemySpawn.position, 5f);
        _lowerHandle.Freeze();
    }
    */
    async Task IntroduceLaser()
    {
        await _speechOut.Speak("In this game you shoot your opponent with a laser.");

        await _speechOut.Speak("When you hear this sound");
        await PlayClipSync(hitClip);
        await _speechOut.Speak("It means you hit your opponent.");

        await _speechOut.Speak("When you hear this.");
        await PlayClipSync(wallClip);
        await _speechOut.Speak("You hit the wall.");

        await _speechOut.Speak("And for this.");
        await PlayClipSync(defaultClip);
        await _speechOut.Speak("You hit nothing.");
    }

    async Task IntroduceHealth()
    {
        await _speechOut.Speak("You take damage when the enemies laser hits you.");
        await _speechOut.Speak("The more health you lose, this heartbeat sound.");

        await PlayClipSync(heartbeatClip);
        await PlayClipSync(heartbeatClip, 100);
        await _speechOut.Speak("will go faster and faster");

    }

    async Task IntroduceLevel()
    {
        await _speechOut.Speak("There are two obstacles.");
        _lowerHandle.Free(); // we free this here, so that level can introduce "objects of interest"
        Level level = GetComponent<Level>();
        await level.PlayIntroduction();

        _upperHandle.Free();
        await _speechOut.Speak("Feel for yourself. Say yes or done when you're ready.");
        await _speechIn.Listen(new Dictionary<string, KeyCode>() { { "yes", KeyCode.Y }, { "done", KeyCode.D } });
    }

    private async Task PlayClipSync(AudioClip clip, int delay = 0)
    {
        _audioSource.PlayOneShot(clip);
        await Task.Delay(Mathf.RoundToInt(clip.length * 1000) + Math.Abs(delay)); // convert sec in ms
    }

    void RegisterColliders()
    {
        PantoCollider[] colliders = FindObjectsOfType<PantoCollider>();
        foreach (PantoCollider collider in colliders)
        {
            Debug.Log(collider);
            collider.CreateObstacle();
            collider.Enable();
        }
    }

    /// <summary>
    /// Starts a new round.
    /// </summary>
    /// <returns></returns>
    async Task ResetRound()
    {
        //await _speechOut.Speak("Spawning player");
        player.transform.position = playerSpawn.position;
        await _upperHandle.SwitchTo(player, 5f);

        //await _speechOut.Speak("Spawning enemy");
        if (playWithEnemy)
        {

            //if (level >= enemyConfigs.Length)
            //    Debug.LogError($"Level {level} is over number of enemies {enemyConfigs.Length}");

            resetEnemies();
            setEnemies(true);
        }

        _upperHandle.Free();

        oldEnemy = enemies[0];

        player.SetActive(true);
        gameRunning = true;
        player.GetComponent<PlayerLogic>().ResetPlayer();

        // deactivate goal until all enemies are dead
        GameObject.FindGameObjectWithTag("Goal").SetActive(false);

        _levelStartTime = Time.time;
    }

    async void FixedUpdate()
    {
        if (gameRunning && enemyChecking && playWithEnemy)
        {
            enemyChecking = false;
            Invoke("ResetEnemyChecking", 0.8f);
            GameObject closestEnemy = GetClosestEnemy(enemies);
            if (oldEnemy != closestEnemy)
            {
                _speechOut.Speak("switching enemy");
                oldEnemy = closestEnemy;
            }
            await _lowerHandle.SwitchTo(closestEnemy, 5f);
        }
    }

    void ResetEnemyChecking()
    {
        enemyChecking = true;
    }

    async void onRecognized(string message)
    {
        Debug.Log("SpeechIn recognized: " + message);
    }

    public void OnApplicationQuit()
    {
        _speechOut.Stop(); //Windows: do not remove this line.
        _speechIn.StopListening(); // [macOS] do not delete this line!
    }

    public void resetEnemies()
    {
        foreach (GameObject _enemy in enemies)
        {
            _enemy.GetComponent<EnemyLogic>().resetLocation();
        }
    }
    public void setEnemies(bool activityStatus)
    {
        foreach (GameObject _enemy in enemies)
        {
            _enemy.SetActive(activityStatus);
        }
    }
    public void spawnEnemy(Vector3 position, Quaternion rotation)
    {
        GameObject newEnemy = Instantiate(enemyPrefab, position, rotation);
        newEnemy.GetComponent<EnemyLogic>().config = enemyConfigs[level];
        newEnemy.GetComponent<EnemyLogic>().target = player.transform;
        newEnemy.GetComponent<EnemyLogic>().setSpawnLocation(position, rotation);

        newEnemy.GetComponent<Shooting>().enemyTransform = player.transform;

        newEnemy.GetComponent<Health>().notifyDefeat = enemyOriginal.GetComponent<Health>().notifyDefeat;
        //newEnemy.GetComponent<Health>().notifyDefeat.

        newEnemy.SetActive(false);
        enemies.Add(newEnemy);
    }

    private GameObject GetClosestEnemy(List<GameObject> enemies)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = player.transform.position;
        foreach (GameObject potentialTarget in enemies)
        {
            if (potentialTarget.activeSelf)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }
        }

        return bestTarget;
    }

    /// <summary>
    /// Ends the round by disabling player and enemy, updating UI, calculating
    /// game score and eventually ending the game.
    /// </summary>
    /// <param name="defeated"></param>
    public async void OnDefeat(GameObject defeated)
    {
        bool playerDefeated = defeated.Equals(player);
        //playerDefeated = false? enemy gets killed
        if (!playerDefeated)
        {
            defeatedEnemies++;
            GameObject defeatedEnemie = enemies.Find(x => x.Equals(defeated));
            //Destroy(defeatedEnemie);
            //enemies.Remove(defeatedEnemie);
            if (defeatedEnemies < enemies.Count)
            {
                AudioSource.PlayClipAtPoint(enemyDyingClips[(int)UnityEngine.Random.Range(0, enemyDyingClips.Length - 1)],
                    defeatedEnemie.transform.position);
            }
            defeatedEnemie.SetActive(false);

            if(enemies.Count == defeatedEnemies)
            {
                await _speechOut.Speak("you eliminated all enemies! No follow the sound to the goal!");
                GameObject.FindGameObjectWithTag("Goal").SetActive(true);
            }
        }
        print("enemy count: " + enemies.Count);
        if (enemies.Count == defeatedEnemies || playerDefeated)
        {

            player.SetActive(false);
            setEnemies(false);
            gameRunning = false;
            defeatedEnemies = 0;


            if (playerDefeated)
            {
                _enemyScore++;
            }
            else
            {
                _playerScore++;
            }

            string defeatedPerson = playerDefeated ? "You" : "Enemy";
            await _speechOut.Speak($"{defeatedPerson} got defeated.");

            level++;
            if (level >= enemyConfigs.Length)
            {
                await GameOver();
            }
            else
            {
                await ResetRound();
            }

        }
    }

    public async void OnVictory(GameObject player)
    {

        player.SetActive(false);
        setEnemies(false);
        gameRunning = false;
        defeatedEnemies = 0;

        await _speechOut.Speak(" Congratulations you have reached the goal.");

        //_gameScore += CalculateGameScore(player, enemy);

        level++;
        if (level >= enemyConfigs.Length)
        {
            await GameOver();
        }
        else
        {
            await ResetRound();
        }
    }


    /// <summary>
    /// Ends the game.
    /// </summary>
    /// <returns></returns>
    async Task GameOver()
    {
        await _speechOut.Speak("Congratulations.");

        if (!GetComponent<DualPantoSync>().debug)
        {
            await _speechOut.Speak($"You achieved a score of {_gameScore}.");
            await _speechOut.Speak("Please enter your name to submit your highscore.");

            await uiManager.GameOver(_gameScore, (int)_totalTime, trophyScore);
        }
        else
        {
            await _speechOut.Speak($"You achieved a score of {_gameScore} in debug mode.");
        }

        await _speechOut.Speak("Thanks for playing DuelPanto. Say quit when you're done.");
        await _speechIn.Listen(new Dictionary<string, KeyCode>() { { "quit", KeyCode.Escape } });

        Application.Quit();
    }

    /// <summary>
    /// Calculates the game score by health, level complete time and enemy
    /// difficulty.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="enemy"></param>
    /// <returns></returns>
    int CalculateGameScore(GameObject player, GameObject enemy)
    {
        Health playerHealth = player.GetComponent<Health>();
        Health enemyHealth = enemy.GetComponent<Health>();

        float levelCompleteTime = Time.time - _levelStartTime;
        _totalTime += levelCompleteTime;
        int timeMultiplier = 1;
        if (levelCompleteTime < 30)
        {
            timeMultiplier = 5;
        }
        else if (levelCompleteTime < 45)
        {
            timeMultiplier = 3;
        }
        else if (levelCompleteTime < 60)
        {
            timeMultiplier = 2;
        }

        int levelScore = playerHealth.healthPoints - enemyHealth.healthPoints;
        if (levelScore > 0)
        {
            int levelMultiplier = (int)(Mathf.Pow(2, level) + 1);
            levelScore *= timeMultiplier * levelMultiplier;
        }

        return levelScore;
    }
}
