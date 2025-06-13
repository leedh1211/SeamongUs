using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    private List<Buff> activeBuffs = new();

    private void Update()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            Buff buff = activeBuffs[i];
            buff.Update(gameObject, Time.deltaTime);
            if (buff.IsExpired)
            {
                buff.Remove(gameObject);
                activeBuffs.RemoveAt(i);
            }
        }
    }

    public void ApplyBuff(Buff buff)
    {
        buff.Apply(gameObject);
        activeBuffs.Add(buff);
    }
}
