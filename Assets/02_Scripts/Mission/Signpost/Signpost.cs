public class Signpost : Mission
{
    // Mission 클래스 생성자 호출
    public Signpost()
        : base(MissionType.Signpost.ToString(),
            "이정표 수리하기")
    { }

    // Clone
    public override Mission Clone()
    {
        return new Signpost();
    }
}
