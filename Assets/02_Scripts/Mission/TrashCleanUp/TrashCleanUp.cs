using System.Collections.Generic;
using UnityEngine;

public class TrashCleanup : Mission
{
    private int totalTrash;    // �� �Ѹ� ������ ����
    private int collected;     // ���ݱ��� �ֿ� ����

    public TrashCleanup()
        : base("TrashCleanup", "����� �����⸦ ��� ġ���ּ���.")
    { }

    public override Mission Clone()
    {
        // �� �ν��Ͻ� ���� (�ѷ��� ���� ������ UI �ʿ��� ����)
        return new TrashCleanup();
    }

    /// <summary>
    /// ������ �ѷ��� �����մϴ�. UI���� Show �ÿ� ȣ�����ּ���.
    /// </summary>
    public void SetTotalTrash(int count)
    {
        totalTrash = count;
        collected = 0;
    }

    /// <summary>
    /// ������ �ϳ��� ���������� �˸��ϴ�.
    /// </summary>
    public bool CollectOne()
    {
        if (IsCompleted) return false;

        collected++;
        if (collected < totalTrash) return false;
        
        return true;  // Mission ���̽� Ŭ���� ���ο��� IsCompleted=true ó��
    }
}