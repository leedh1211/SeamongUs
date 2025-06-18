public abstract class Mission
{
    // 외부에서 읽기만 가능하도록 프로퍼티 설정
    public string MissionID { get; }
    public string Description { get; }
    public bool IsCompleted { get; private set; }

    // 생성자로 ID/설명 설정
    protected Mission(string missionID, string description)
    {
        MissionID = missionID;
        Description = description;
    }

    // 각 플레이어에게 개별 인스턴스를 나눠줄 수 있도록 복제해주어야함.
    // 그렇게 안하면, 모든 플레이어가 같은 미션을 공유하게 됨.
    public abstract Mission Clone();

    // 미션 완료 처리 메서드
    public void Complete()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
        }
    }
}
