using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : MonoBehaviour
{
    public void BtnPlayerView(int PlayerIndex)
    {
        GameEvent.ViewPlayer(GameManager.instance.GetPlayer(PlayerIndex), false);
    }
}