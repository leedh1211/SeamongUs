using System;
using System.Collections.Generic;
using UnityEngine;

public class HerdMission : Mission
{
    private int totalSheep;
    private int collected;

    public HerdMission()
        : base("HerdMission", "양치기에게 양을 몰아가세요.")
    { }

    public override Mission Clone()
    {
        return new HerdMission();
    }

    public void SetTotalSheep(int count)
    {
        totalSheep = count;
        collected = 0;
    }

}
