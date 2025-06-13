using System.Collections.Generic;
using UnityEngine;

public class TrashCleanup : Mission
{
    private int totalTrash;    // 총 뿌린 쓰레기 개수
    private int collected;     // 지금까지 주운 개수

    public TrashCleanup()
        : base("TrashCleanup", "흩어진 쓰레기를 모두 치워주세요.")
    { }

    public override Mission Clone()
    {
        // 새 인스턴스 생성 (총량과 수거 갯수는 UI 쪽에서 설정)
        return new TrashCleanup();
    }

    /// <summary>
    /// 쓰레기 총량을 설정합니다. UI에서 Show 시에 호출해주세요.
    /// </summary>
    public void SetTotalTrash(int count)
    {
        totalTrash = count;
        collected = 0;
    }

    /// <summary>
    /// 쓰레기 하나를 수거했음을 알립니다.
    /// </summary>
    public void CollectOne()
    {
        if (IsCompleted) return;

        collected++;
        if (collected >= totalTrash)
            Complete();  // Mission 베이스 클래스 내부에서 IsCompleted=true 처리
    }
}