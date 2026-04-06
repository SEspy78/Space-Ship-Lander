using UnityEngine;

public class GameManager : MonoBehaviour
{


    public static GameManager Instance { get; private set; }
    private int score;
    private float time;
    private bool isTimerActive;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Lander.Instance.OnCoinPickUp += Lander_OnCoinPickUp;
        Lander.Instance.OnLanded += Lander_OnLanded;
        Lander.Instance.OnStateChanged += Lander_OnStateChanged;
    }

    private void Lander_OnStateChanged(object sender, Lander.OnStateChangedEventArgs e)
    {
        isTimerActive = e.state == Lander.State.Normal ;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        addScore(e.score);
    }

    private void Lander_OnCoinPickUp(object sender, System.EventArgs e)
    {
       addScore(500);
        
    }

    private void Update()
    {
        if (isTimerActive)
        {
            time += Time.deltaTime;
        }
    }

    public void addScore(int addScoreAmount)
    {
        score += addScoreAmount;
        Debug.Log(score);
    }

    public int getScore()
    {
        return score;
    }


    public float getTime()
    {
        return time;
    }
}
