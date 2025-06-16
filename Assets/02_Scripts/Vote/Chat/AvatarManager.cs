using UnityEngine;

namespace _02_Scripts.Ung_Managers
{
    public class AvatarManager : MonoBehaviour
    {
        public static AvatarManager Instance;

        public Sprite[] avatarList; // 0: 기본, 1: 모자 쓴 버전, 2: 고양이 … 등
        public AnimatorOverrideController[] overrideList; // 각 캐릭터 전용 애니메이터


        private void Awake()
        {
            Instance = this;
        }

        public Sprite GetSprite(int idx) => idx < avatarList.Length ? avatarList[idx] : null;
        public RuntimeAnimatorController GetAnim(int idx) => idx < overrideList.Length ? overrideList[idx] : null;
    }
}