
using UnityEngine;

namespace Utill
{
    public static class ComponentExtensions
    {
        /// TryGetComponent 후 바로 변수에 할당하는 헬퍼
        public static bool AssignComponent<T>(this Component component, ref T target) where T : Component
        {
            if (component.TryGetComponent<T>(out var result))
            {
                target = result;
                return true;
            }
            Debug.Log(target.name +"변수할당에 실패하였습니다.");
            return false;
        }
    }
}