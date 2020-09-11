using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameOverPanel : ActiveOnlyDuringSomeGameStates
{

    public enum eGameOverPanelState
    {
        none, idle, fadeIn, fadeIn2, fadeIn3, display
    }

    private Text finalLevelText;
    private Text finalScoreText;
    private Text gameOverText;
    private Image backgroundForPanel;
    private Image finalTextBGImage;


    public float fadeTime = 1f;

    [Header("Set Game State")]
    [SerializeField]
    private eGameOverPanelState state = eGameOverPanelState.none;

    float stateStartTime, stateDuration;
    eGameOverPanelState nextState;

    // on idle state so it sets gameObject to inactive then base.Awake gets notified if active
    public override void Awake()
    {
        #region Getting/Verifying Members
        backgroundForPanel = GetComponent<Image>();

        Transform gameOverTextLevel = transform.Find("GameOverText");
        if (gameOverTextLevel == null)
        {
            Debug.LogWarning("GameOverText Child Is Missing.");
            return;
        }
        gameOverText = gameOverTextLevel.GetComponent<Text>();
        if (gameOverText == null)
        {
            Debug.LogWarning("Game Over Text Child Is Missing.");
            return;
        }

        Transform finalTextBG = transform.Find("FinalTextBGImage");
        if (finalTextBG == null)
        {
            Debug.LogWarning("FinalTextBGImage Child Is Missing.");
            return;
        }
        finalTextBGImage = finalTextBG.GetComponent<Image>();
        if (finalTextBGImage == null)
        {
            Debug.LogWarning("finalTextBGImage Is Missing.");
            return;
        }
        Transform levelChild = finalTextBG.transform.Find("FinalLevelText");
        if (levelChild == null)
        {
            Debug.LogWarning("levelChild Is Missing.");
            return;
        }
        finalLevelText = levelChild.GetComponent<Text>();
        if (finalLevelText == null)
        {
            Debug.LogWarning("finalLevelText Is Missing.");
            return;
        }

        Transform scorechild = finalTextBG.transform.Find("FinalScoreText");
        if (scorechild == null)
        {
            Debug.LogWarning("scorechild Is Missing.");
            return;
        }
        finalScoreText = scorechild.GetComponent<Text>();
        if (finalScoreText == null)
        {
            Debug.LogWarning("finalScoreText Is Missing.");
            return;
        }
        #endregion

        SetState(eGameOverPanelState.idle);
        base.Awake();
    }


    protected override void DetermineActive()
    {
        base.DetermineActive();
        if (AsteraX.GAME_STATE == AsteraX.eGameState.gameOver)
        {
            SetState(eGameOverPanelState.fadeIn);
        }
    }

    private void SetState(eGameOverPanelState newState)
    {
        stateStartTime = realTime;

        // instead of doing an animation inside of unity set it up here
        switch (newState)
        {
            case eGameOverPanelState.idle:
                gameObject.SetActive(false);
                break;
            case eGameOverPanelState.fadeIn:
                //sets intial state of transition
                gameObject.SetActive(true);
                finalLevelText.text = "Final Level: " + AsteraX.CurrentLevel.ToString("N0");
                finalLevelText.color = Color.clear;
                finalScoreText.text = "Final Score: " + AsteraX.TOTAL_SCORE.ToString("N0");
                finalScoreText.color = Color.clear;
                backgroundForPanel.color = Color.clear;
                finalTextBGImage.color = Color.clear;
                gameOverText.color = Color.clear;
                // state
                stateDuration = fadeTime * 0.1f;
                nextState = eGameOverPanelState.fadeIn2;
                break;
            case eGameOverPanelState.fadeIn2:
                // initial state before fade from update
                finalLevelText.color = Color.clear;
                finalScoreText.color = Color.clear;
                backgroundForPanel.color = Color.black;
                finalTextBGImage.color = Color.clear;
                gameOverText.color = Color.clear;
                // state
                stateDuration = fadeTime * 0.6f;
                nextState = eGameOverPanelState.fadeIn3;
                break;
            case eGameOverPanelState.fadeIn3:
                // state
                stateDuration = fadeTime * 0.2f;
                nextState = eGameOverPanelState.display;
                break;
            case eGameOverPanelState.display:
                // state
                stateDuration = 9999999;
                nextState = eGameOverPanelState.none;
                break;
            default:
                break;

        }
        state = newState;
    }

    private void Update()
    {
        if (state == eGameOverPanelState.none)
        {
            return;
        }
        // find how much time has passed vs the last state change / the set stateduration
        float u = (realTime - stateStartTime) / stateDuration;
        bool moveNext = false;
        if (u > 1)
        {
            u = 1;
            moveNext = true;
        }
        Color whiteOpacity;
        switch (state)
        {
            case eGameOverPanelState.fadeIn:
                backgroundForPanel.color = new Color(0, 0, 0, u);
                break;
            case eGameOverPanelState.fadeIn2:
                whiteOpacity = new Color(1, 1, 1, u * u);
                finalLevelText.color = whiteOpacity;
                finalScoreText.color = whiteOpacity;
                gameOverText.color = whiteOpacity;
                finalTextBGImage.color = whiteOpacity;
                break;
            default:
                break;
        }
        if (moveNext) SetState(nextState);
    }

    // need to get everytime using it, if not then can't set stateStartTime in SetState()
    float realTime
    {
        get
        {
            return Time.realtimeSinceStartup;
        }
    }

}
