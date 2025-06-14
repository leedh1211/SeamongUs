using System;
using System.Collections.Generic;

public static class MissionFactory
{
    // Enum 마다 어떤 클래스 인스턴스를 만들지보기 위한 딕셔너리
    private static readonly Dictionary<MissionType, Func<Mission>> map
        = new Dictionary<MissionType, Func<Mission>>()
    {
        // { MissionType.FixSignpost, () => new FixSignpost() },
        { MissionType.Laundry,    () => new Laundry()    },
        // { MissionType.TrashCleanup,() => new TrashCleanup() }
    };

    // 전체 미션 타입 목록
    public static IEnumerable<MissionType> GetAllTypes()
    {
        return map.Keys;
    }

    public static Mission Create(MissionType type)
    {
        return map[type]();
    }
}
