using UnityEngine;

public class Stress : MonoBehaviour
{
    public int maxStress = 100;
    public int curStress = 0;

    public void UseStress(int amount)
    {
        if (curStress - amount < 0) return;
        curStress -= amount;
    }


    public void RecoverStress(int amount)
    {
        curStress = Mathf.Min(curStress + amount, maxStress);
    }
}
