using System;
using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using DualPantoFramework;
public class GameManager : MonoBehaviour
{

    public bool introduceGame = true;
    public GameObject player;
    public GameObject enemyOriginal;
    public GameObject enemyPrefab;
    public GameObject goal;

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
            //Debug.Log(collider);
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
    }

<<<<<<< Updated upstream
=======
    private async void NextLevel(int level)
    {
        print("currLevel when entering NextLevel(): " + currLevel);
        print("level NextLevel() call: " + level);
        // max level reached --> gameOver
        if (level > 4)
        {
           
            await GameOver();
        }

        // reset all obstacles; reactivate for each level
        foreach (GameObject o in obstacles)
        {
            o.SetActive(false);
            Debug.Log("disabling " + o.ToString());
        }
        playerSpawn.position = goal.transform.position;

        //level = 1;
        switch (level)
        {
            case 0:
                playLevelDescription();
                activateTags(new string[] { "Wall" });
                     SpawnsEnemies(enemySpawnLvl2);
                goal.transform.position = new Vector3(6.0f, 0.0f, -8.0f);
                break;

            case 1:
                activateTags(new string[] { "Wall", "level1", "level2", "level3" });

                // adding level-colliders
                RegisterCollidersByTag(new string[] { "level1" });
                RegisterCollidersByTag(new string[] { "level2" });
                RegisterCollidersByTag(new string[] { "level3" });
                goal.transform.position = new Vector3(0.0f, 0.0f, -3.0f);

                break;

            case 2:
                activateTags(new string[] { "Wall", "level1", "level2", "level3" });


                SpawnsEnemies(enemySpawnLvl2);
                goal.transform.position = new Vector3(2.0f, 0.0f, -14.0f);

                break;

            case 3:
                //spawnEnemy(enemySpawn[1].position, enemySpawn[1].rotation);
                activateTags(new string[] { "Wall", "level1", "level2", "level3" });


                SpawnsEnemies(enemySpawnLvl3);

                goal.transform.position = new Vector3(2.0f, 0.0f, -5.0f);
                break;

            case 4:
                activateTags(new string[] { "Wall", "level1", "level2", "level3", "level4" });
                RegisterCollidersByTag(new string[] { "level4" });


                SpawnsEnemies(enemySpawnLvl4);

                goal.transform.position = new Vector3(6.0f, 0.0f, -8.0f);

                break;

            default:
                break;

        }
        await ResetRound();
    }

    async private void playLevelDescription(){
        switch (currLevel)
        {
            case 0:
                await _speechOut.Speak("Follow the sound to the goal.");
                break;
            case 1:
                await _speechOut.Speak("Explore the obstacles");
                break;
            case 2:
                await _speechOut.Speak("Watch out there are enemies!"); 
                await _speechOut.Speak("The IT-Handle shows the nearest enemy");
                await _speechOut.Speak("Also you can hear them");
                await _speechOut.Speak("Shot them!");
                break;
            case 3:
                break;

            case 4:
                break;
                
            default:
                await _speechOut.Speak("You completed all levels!");
                break;
        }
    }

    private void SpawnsEnemies(Transform[] spawns)
    {
        
        for (int i = 0; i < spawns.Length; i++)
        {
            if (i > (enemies.Count - 1))
            {
                spawnEnemy(spawns[i].position, spawns[i].rotation);
            }
            else
            {
                enemies[i].GetComponent<EnemyLogic>().
                                                   setSpawnLocation(
                                                       spawns[i].position,
                                                       spawns[i].rotation);

            }
        }
    }

    private void activateTags(String[] s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            print(s[i]);
            foreach (GameObject o in obstacles)
            {
                if (o.tag == s[i])
                {
                    o.SetActive(true);
                    //o.GetComponent<PantoBoxCollider>().Enable();

                }
            }
        }
    }

>>>>>>> Stashed changes
    async void FixedUpdate()
    {
        if (gameRunning && enemyChecking && playWithEnemy && !allEnemiesdefeated)
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
            _enemy.GetComponent<EnemyLogic>().reset();
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
                allEnemiesdefeated = true;

                await _lowerHandle.SwitchTo(goal);
                switchedToGoal = true;

            }
        }
        else
        {

<<<<<<< Updated upstream
=======
        
            setEnemies(false);
            gameRunning = false;
            if (playerLives <= 1)
            {
                await GameOver();
            }
            else playerLives--;


>>>>>>> Stashed changes
            StartCoroutine(playerGotFinished());
        }
    }



    private IEnumerator playerGotFinished()
    {
        _audioSource.PlayOneShot(playerDied);
        yield return new WaitForSeconds(playerDied.length);       
        player.SetActive(false);
        player.GetComponent<PlayerSoundEffect>().stopBackgroundMusic();
        player.GetComponent<PlayerSoundEffect>().playGoalReachedClip();
        yield return new WaitForSeconds(2);
       
        ResetRound();
    }
    public async void OnVictory(GameObject player)
    {
<<<<<<< Updated upstream

        player.SetActive(false);
        setEnemies(false);
        gameRunning = false;

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


=======
        //level++;
        StartCoroutine(playerReachedGoal());
        player.SetActive(false);
        setEnemies(false);
        gameRunning = false;
        
    }

    private IEnumerator playerReachedGoal()
    {
        currLevel +=1;
        player.GetComponent<PlayerSoundEffect>().playGoalReachedClip();
        yield return new WaitForSeconds(1);
        player.GetComponent<PlayerSoundEffect>().playFinisherClip();
        yield return new WaitForSeconds(1);
        playLevelDescription();
         yield return new WaitForSeconds(4);
        player.GetComponent<PlayerSoundEffect>().playGoalReachedClip();
        yield return new WaitForSeconds(2);
        NextLevel((currLevel));
    }
>>>>>>> Stashed changes
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
