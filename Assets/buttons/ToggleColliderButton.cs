using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleColliderButton : UdonSharpBehaviour
{
    // Ссылка на управляющий скрипт
    public ColliderToggleManager toggleManager;

    // Метод, который вызывается при нажатии на кнопку
    public override void Interact()
    {
        if (toggleManager != null)
        {
            toggleManager.ToggleColliders(); // Вызываем метод для переключения состояния коллайдеров
        }
    }
}
