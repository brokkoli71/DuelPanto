using System;
using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using DualPantoFramework;
public class GameManager : MonoBehaviour
{
    public GameObject goal;
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

    public AudioClip[] enemyDyingClips;

    public AudioClip switchingEnemy;
    public AudioClip playerDied;
    public AudioClip enenmiesDefeated;
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
    public bool allEnemiesDefeated = false;
    private bool switchedToGoal = false;

    private bool enemyChecking = true;
    private int defeatedEnemies = 0;
    public bool allEnemiesdefeated = false;
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
            await IntroduceLevel();
        }

        //await _speechOut.Speak("Introduction finished, game starts.");

        await ResetRound();
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

        _lowerHandle.Free();

        //await _speechOut.Speak("Spawning enemy");
        if (playWithEnemy)
        {

            //if (level >= enemyConfigs.Length)
            //    Debug.LogError($"Level {level} is over number of enemies {enemyConfigs.Length}");

            resetEnemies();
            allEnemiesdefeated = false;
            setEnemies(true);
            oldEnemy = enemies[0];
            defeatedEnemies = 0;
        }

        _upperHandle.Free();



        player.SetActive(true);
        gameRunning = true;
        player.GetComponent<PlayerLogic>().ResetPlayer();
        player.GetComponent<PlayerSoundEffect>().ResetMusic();

        _levelStartTime = Time.time;

        allEnemiesDefeated = false;
        switchedToGoal = false;
        defeatedEnemies = 0;
    }

    async void FixedUpdate()
    {
        if (gameRunning && enemyChecking && playWithEnemy && !allEnemiesDefeated)
        {
            enemyChecking = false;
            Invoke("ResetEnemyChecking", 0.8f);
            GameObject closestEnemy = GetClosestEnemy(enemies);
            if (oldEnemy != closestEnemy)
            {
                //_speechOut.Speak("switching enemy");
                _audioSource.PlayOneShot(switchingEnemy);
                oldEnemy = closestEnemy;
            }
            await _lowerHandle.SwitchTo(closestEnemy, 5f);
        }
        else if (allEnemiesDefeated && !switchedToGoal)
        {
            await _lowerHandle.SwitchTo(goal);
            switchedToGoal = true;
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
        bool enemyKilled = !defeated.Equals(player);
        if (enemyKilled)
        {
            defeatedEnemies++;
            GameObject defeatedEnemie = enemies.Find(x => x.Equals(defeated));
            //Destroy(defeatedEnemie);
            //enemies.Remove(defeatedEnemie);


            AudioSource.PlayClipAtPoint(enemyDyingClips[(int)UnityEngine.Random.Range(0, enemyDyingClips.Length - 1)],
                defeatedEnemie.transform.position);


            defeatedEnemie.SetActive(false);

            if (enemies.Count == defeatedEnemies)
            {
                //_speechOut.Speak("you eliminated all enemies! No follow the sound to the goal!");
                _audioSource.PlayOneShot(enenmiesDefeated);

                await _lowerHandle.SwitchTo(goal);
                switchedToGoal = true;

                defeatedEnemie.SetActive(false);
                // activate goal
                await _speechOut.Speak("you eliminated all enemies! Now follow the sound to the goal!");
                allEnemiesDefeated = true;
                return;
            }

            defeatedEnemie.SetActive(false);
        }
        else
        {
            print("Player got killed!");
            _audioSource.PlayOneShot(playerDied);
            player.SetActive(false);
            setEnemies(false);
            gameRunning = false;
            //await _speechOut.Speak("You got defeated.");

            await ResetRound();
        }
    }

    public async void OnVictory(GameObject player)
    {
        if (allEnemiesDefeated)
        {
            player.SetActive(false);
            setEnemies(false);
            gameRunning = false;
            defeatedEnemies = 0;


            //await _speechOut.Speak(" Congratulations you have reached the goal.");

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

}
