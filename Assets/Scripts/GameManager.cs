using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Lander lander;

    public static GameManager Instance { get; private set; }
    private int score;
    private float time;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        lander.OnCoinPickUp += Lander_OnCoinPickUp;
    }

    private void Lander_OnCoinPickUp(object sender, System.EventArgs e)
    {
       addScore(500);
        
    }

    private void Update()
    {
        time += Time.deltaTime; 
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
