using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveOnlyDuringSomeGameStates : MonoBehaviour
{

    [EnumFlags]
    public AsteraX.eGameState activeStates = AsteraX.eGameState.all;

    public virtual void Awake()
    {
        EventBroker.GameStateChange += DetermineActive;

        // this call makes sure the self is based on the current state when awaken
        DetermineActive();
    }

    protected void OnDestroy()
    {
        EventBroker.GameStateChange -= DetermineActive;

    }

    protected virtual void DetermineActive()
    {
        // This compares the active states of this current GO and the current GameState of the game
        // The single & compares the two via there bit (see AsteraX for the bits) and if that bit is true for both then it's true for the gamestate
        // thus if the current game state is what the GO says it should be active on in the inspector window, then it will be active 
        bool shouldBeActive = (activeStates & AsteraX.GAME_STATE) == AsteraX.GAME_STATE;
        gameObject.SetActive(shouldBeActive);
    }
}
