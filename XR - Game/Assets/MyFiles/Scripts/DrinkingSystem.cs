using UnityEngine;

public class DrinkingSystem : MonoBehaviour
{
    [SerializeField] private float healthBonus = 20; 
    public AudioClip DrinkingSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Can"))
        {
            ConsumeCan(other.gameObject);
        }
    }

    private void ConsumeCan(GameObject canObject)
    {
        AudioManager.Instance.PlaySFX(DrinkingSound);
        Player.Instance.ReceiveHealth(healthBonus);
        Destroy(canObject);
    }
}
