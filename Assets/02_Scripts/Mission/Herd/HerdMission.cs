using System;
using System.Collections.Generic;
using UnityEngine;

public class HerdMission : Mission
{
    private int totalSheep;
    private int collected;

    public HerdMission()
        : base("HerdMission", "�綼�� �츮���� �����ּ���.")
    { }

    public override Mission Clone()
    {
        return new HerdMission();
    }

    // �� ���� ����
    public void SetTotalSheep(int count)
    {
        totalSheep = count;
        collected = 0;
    }

    // �� �ϳ��� ��ǥ ������ �������� �� ȣ��
    public void CollectOne()
    {
        if (IsCompleted) return;
        collected++;
        if (collected >= totalSheep)
            Complete();
    }
}
