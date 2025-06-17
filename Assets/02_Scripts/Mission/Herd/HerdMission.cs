public class HerdMission : Mission
{
    private int totalSheep;
#pragma warning disable CS0414 // 필드는 나중에 사용될 예정이므로 현재는 무시
    private int collected;
#pragma warning restore CS0414

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
