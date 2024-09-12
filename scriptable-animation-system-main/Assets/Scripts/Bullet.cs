using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damageAmount;
    public int damage = 20;
    public int ID;
    public void SetDamageAmount(float amount)
    {
        damageAmount = amount;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("RemotePlayer") || collision.gameObject.layer == LayerMask.NameToLayer("LocalPlayer") || collision.gameObject.layer == LayerMask.NameToLayer("BOT") || collision.gameObject.layer == LayerMask.NameToLayer("Limbs"))
        {
            PlayerHealth _health = collision.transform.GetComponent<PlayerHealth>();
            //_health.TakeDamage(damage);
            Limbs _limb = collision.transform.GetComponent<Limbs>();

            _limb.SendHitToServer(ID, damage);
        }
        
        Destroy(gameObject);
    }
}