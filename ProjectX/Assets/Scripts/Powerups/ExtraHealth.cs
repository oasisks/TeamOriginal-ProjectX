using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraHealth : Powerup
{
    [SerializeField] int extraHealth;
    public override void affectPlayer(GameObject player)
    {
        Player playerObj = player.GetComponent<Player>();
        playerObj.health += extraHealth;
    }
}
