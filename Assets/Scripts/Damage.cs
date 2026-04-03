using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] int damageAmount = 10;

    public int GetDamage()
    {
        return damageAmount;
    }

   
}
