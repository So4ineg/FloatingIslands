using UnityEngine;

// Это здание имеет и ВЕС (оно из металла), и ПОДЪЕМНУЮ СИЛУ (оно работает).
// Поэтому мы наследуем ОБА интерфейса!
public class AntiGravGenerator : MonoBehaviour, IWeightable, ILifting
{
    [Header("Параметры генератора")]
    public float weight = 20f;        // Вес самой постройки
    public float liftPower = 150f;    // Сила, с которой он тянет остров вверх
    public bool isWorking = true;     // Можно выключать, если кончилось топливо!

    // Обязательный метод от IWeightable
    public float GetWeight()
    {
        return weight;
    }

    // Обязательный метод от ILifting
    public float GetLiftForce()
    {
        if (isWorking)
        {
            return liftPower;
        }
        return 0f; // Если генератор выключен, он не дает подъемной силы
    }
}
