using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;

public class HUD : MonoBehaviour 
{
    #region Private attributes

    // Player
    PlayerController player;

    // Total score owned by player
    int totalScore;

    // Initial player lives
    int initialLives;

    // If healthbar is blinking
    bool blinking;

    #endregion

    #region Public attributes

    // Text to display current player lives
    [SerializeField] Text lives;

    // Players healthbar
    [SerializeField] Image healthBar;

    // Canvas group of players healthbar, so can be hided
    [SerializeField] CanvasGroup healthBarGroup;

    // When players health is low, healthbar starts to blink
    [SerializeField] float lowHealth;

    // Text to display current player score
    [SerializeField] Text score;

    // Game Over panel animator in order to fade in it
    [SerializeField] Animator gameOverAnimator;

    // Animator of Game win panel
    [SerializeField] Animator winGameAnimator;

    // Texto to display highscore
    [SerializeField] Text highScore;

    #endregion

    // Static instance of HUD which allows it to be accessed by any other script.
    public static HUD instance = null;

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

 
        Setup();
    }

    /// <summary>
    /// Setup this instance. Get player ref and start listen HUD events
    /// </summary>
    void Setup()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        initialLives = player.health;
        lives.text = initialLives.ToString();
        blinking = false;
        highScore.text = Persistence.ReturnValue ("Score").ToString();

        EventManager.StartListening<BasicEvent>(Common.ON_LIVES_CHANGED, OnLivesChanged);
        EventManager.StartListening<BasicEvent>(Common.ON_GAME_OVER, OnGameOver);
        EventManager.StartListening<BasicEvent>(Common.ON_RAISE_SCORE, OnRaiseScore);
        EventManager.StartListening<BasicEvent>(Common.ON_WIN_GAME, OnWinGame);
    }

    /// <summary>
    /// Cleanup this instance. Stop listening events
    /// </summary>
    public void Cleanup()
    {
        EventManager.StopListening<BasicEvent>(Common.ON_LIVES_CHANGED, OnLivesChanged);
        EventManager.StopListening<BasicEvent>(Common.ON_GAME_OVER, OnGameOver);
        EventManager.StopListening<BasicEvent>(Common.ON_RAISE_SCORE, OnRaiseScore);
        EventManager.StopListening<BasicEvent>(Common.ON_WIN_GAME, OnWinGame);
    }

    /// <summary>
    /// On player lives changed. Updates screen text
    /// </summary>
    /// <param name="e">Event information. Player lives updated</param>
    void OnLivesChanged(BasicEvent e)
    {
        var intLives = (int)e.Data;

        healthBar.fillAmount = (float)intLives / (float)initialLives;

        if (!blinking && healthBar.fillAmount <= lowHealth)
        {
            blinking = true;
            healthBar.GetComponent<Animator>().SetTrigger("Blink");
        }

        lives.text = intLives.ToString();
    }

    /// <summary>
    /// Game over. Show panel to restart game
    /// </summary>
    /// <param name="e">Event information, useless</param>
    void OnGameOver(BasicEvent e)
    {
        healthBarGroup.alpha = 0f;
        gameOverAnimator.SetTrigger("FadeIn");
    }

    /// <summary>
    /// On player score changed. Updates screen text
    /// </summary>
    /// <param name="e">Event information. Enemy score</param>
    void OnRaiseScore(BasicEvent e)
    {
        var intScore = (int)e.Data;

        totalScore += intScore;

        score.text = totalScore.ToString();
    }

    /// <summary>
    /// On player win game. Show win panel
    /// </summary>
    /// <param name="e">E.</param>
    void OnWinGame(BasicEvent e)
    {
        player.enabled = false;
        ObjectPooler.Instance.ReturnPooledObjects();
        winGameAnimator.SetTrigger("FadeIn");
    }
}
