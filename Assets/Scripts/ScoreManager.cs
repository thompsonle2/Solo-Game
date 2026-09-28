using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Numerics;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private int score = 0;
    private int totalEnemies = 7;

    void Start()
    {
        // Counts all GameObjects in the scene with the "Enemy" tag
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        UpdateScoreUI();
    }

    public void AddScore(int amount = 1)
    {
        score += amount;
        UpdateScoreUI();

        // check if all enemies have been defeated
        if (score >= totalEnemies && totalEnemies > 0)
        {
            TriggerWin();
        }
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}/{totalEnemies}";
    }

    private void TriggerWin()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.GameWin();
        }        
    }
}