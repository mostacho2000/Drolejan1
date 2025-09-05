using UnityEngine;

public interface IDamageable
{
    // amount = daño; hitPoint = punto de impacto; hitDir = dirección desde la explosión/proyectil
    void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDir);
}
