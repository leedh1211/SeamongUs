using System.Collections.Generic;
using UnityEngine;

public class TrashCleanup : Mission
{
    private int totalTrash;   
    private int collected;     

    public TrashCleanup()
        : base("TrashCleanup", "쓰레기를 정리해주세요.")
    { }

    public override Mission Clone()
    {
       
        return new TrashCleanup();
    }

 
    public void SetTotalTrash(int count)
    {
        totalTrash = count;
        collected = 0;
    }


    public bool CollectOne()
    {
        if (IsCompleted) return false;

        collected++;
        if (collected < totalTrash) return false;
        
        return true;  
    }
}