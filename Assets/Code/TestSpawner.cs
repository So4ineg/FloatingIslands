using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    public IslandProperties targetIsland;

    void Start()
    {
        if (targetIsland == null) return;

        // Создаем 3 рабочих и 1 инженера
        PopulationManager.Instance.CreateAndSpawnPerson("Джон Доу", "Мужской", 32, SocialClass.Worker, targetIsland);
        PopulationManager.Instance.CreateAndSpawnPerson("Артур Пэндлтон", "Мужской", 45, SocialClass.Engineer, targetIsland);
        PopulationManager.Instance.CreateAndSpawnPerson("Эмма Вотсон", "Женский", 28, SocialClass.Worker, targetIsland);

        // Присвоим Эмме тег болезни для теста
        Person sickPerson = PopulationManager.Instance.CreateAndSpawnPerson("Билл Гейтс", "Мужской", 50, SocialClass.Worker, targetIsland);
        sickPerson.AddTag(PersonTag.Sick);
    }
}