using System.Collections.Generic;
using UnityEngine;

public class WorkplaceBuilding : Building
{
    [Header("Рабочие места")]
    public SocialClass requiredClass = SocialClass.Worker;
    public int maxJobs = 4;
    public List<Person> workers = new List<Person>();

    public bool HasVacancies => workers.Count < maxJobs;

    public bool HireWorker(Person person)
    {
        if (!HasVacancies || workers.Contains(person)) return false;
        if (person.socialClass != requiredClass) return false; // Проверка квалификации

        workers.Add(person);
        person.workBuilding = this;
        return true;
    }

    public void DismissWorker(Person person)
    {
        if (workers.Contains(person))
        {
            workers.Remove(person);
            person.workBuilding = null;
        }
    }

    public override string GetStatusInfo()
    {
        return $"<b>{buildingName}</b>\n" +
               $"Требуются: {requiredClass}\n" +
               $"Рабочих: {workers.Count}/{maxJobs}\n" +
               $"Масса: {GetWeight():F1} т.";
    }
}