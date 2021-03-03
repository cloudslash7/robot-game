using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverPowerup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Debug.Log("Picked up!");
            Pickup(other);
        }
    }
    void Pickup(Collider2D player) {
        player.GetComponent<Player>().hoverUnlocked = true;
    }
}
