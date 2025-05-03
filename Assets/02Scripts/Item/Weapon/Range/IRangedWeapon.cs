using UnityEngine;
public interface IRangedWeapon
{
    float aimFOV(float fov);
    Vector3 aimOffset();
    void Reload(float reloatTime, int ammunitionNum);
}
