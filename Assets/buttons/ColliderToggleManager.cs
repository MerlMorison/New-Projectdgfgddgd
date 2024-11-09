using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ColliderToggleManager : UdonSharpBehaviour
{
    // Массив для хранения корневых объектов с коллайдерами
    public GameObject[] targetObjects;

    // Флаг для отслеживания состояния (включены или выключены коллайдеры)
    [UdonSynced] // Синхронизация состояния между игроками
    private bool isColliderEnabled = true;

    // Метод для переключения состояния коллайдеров
    public void ToggleColliders()
    {
        isColliderEnabled = !isColliderEnabled; // Инвертируем текущее состояние
        UpdateCollidersState(); // Обновляем состояние коллайдеров
    }

    // Метод для обновления состояния коллайдеров на всех объектах
    private void UpdateCollidersState()
    {
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
            {
                // Включаем/выключаем коллайдеры у объекта и всех его дочерних объектов
                ToggleCollidersInChildren(obj);
            }
        }
    }

    // Рекурсивный метод для включения/выключения коллайдеров у объекта и его дочерних объектов
    private void ToggleCollidersInChildren(GameObject obj)
    {
        Collider[] colliders = obj.GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = isColliderEnabled; // Включаем/выключаем каждый коллайдер
        }

        foreach (Transform child in obj.transform)
        {
            ToggleCollidersInChildren(child.gameObject);
        }
    }
}
