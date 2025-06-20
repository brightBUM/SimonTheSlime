using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreboardTitleUI;
    [SerializeField] TextMeshProUGUI bananasLevelCompleteUI;
    [SerializeField] TextMeshProUGUI levelTimerCompleteUI;
    [SerializeField] TextMeshProUGUI gemsUI;
    [SerializeField] TextMeshProUGUI levelScoreUI;
    [SerializeField] GameObject nextLevelButton;
    [SerializeField] float levelCompleteTextDelay = 0.2f;
    [SerializeField] float scoreCountTime = 2f;
    private List<TextMeshProUGUI> levelCompleteTexts;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
