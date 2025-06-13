using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixSignpost : Mission
{
    // Mission 클래스 생성자 호출
    public FixSignpost()
        : base(MissionType.FixSignpost.ToString(),
            "이정표 수리하기")
    { }

    // Clone
    public override Mission Clone()
    {
        return new FixSignpost();
    }
}
