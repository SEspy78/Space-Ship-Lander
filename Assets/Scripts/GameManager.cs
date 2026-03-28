using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Lander lander;
    private int score;


    private void Start()
    {
        lander.OnCoinPickUp += Lander_OnCoinPickUp;
    }

    private void Lander_OnCoinPickUp(object sender, System.EventArgs e)
    {
       addScore(500);
        
    }

    public void addScore(int addScoreAmount)
    {
        score += addScoreAmount;
        Debug.Log(score);
    }
}
