using UnityEngine;

namespace _02_Scripts.Ung_Managers
{
    public class AvatarManager : MonoBehaviour
    {
        public static AvatarManager Instance;

        public Sprite[] avatarList; // 0: 기본, 1: 모자 쓴 버전, 2: 고양이 … 등

        private void Awake()
        {
            Instance = this;
        }

        public Sprite GetSprite(int index)
        {
            if (index >= 0 && index < avatarList.Length)
                return avatarList[index];
            return null;
        }
    }

}