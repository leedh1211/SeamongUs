using System;
using System.Collections.Generic;
using UnityEngine;

public class HerdMission : Mission
{
    private int totalSheep;
    private int collected;

    public HerdMission()
        : base("HerdMission", "양떼를 우리까지 몰아주세요.")
    { }

    public override Mission Clone()
    {
        return new HerdMission();
    }

    // 양 수를 설정
    public void SetTotalSheep(int count)
    {
        totalSheep = count;
        collected = 0;
    }

    // 양 하나가 목표 지점에 도달했을 때 호출
    public void CollectOne()
    {
        if (IsCompleted) return;
        collected++;
        if (collected >= totalSheep)
            Complete();
    }
}
