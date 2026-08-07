using System.Collections.Generic;
using UnityEngine;

public class ResidentialHouse : Building
{
    [Header("Настройки жилья")]
    public int maxCapacity = 5;
    public List<Person> residents = new List<Person>();

    public bool HasFreeBeds => residents.Count < maxCapacity;

    // Переопределяем расчет массы: Вес дома + Вес всех жильцов
    public override float GetWeight()
    {
        // Базовый вес дома + масса всех реальных жильцов (по 0.07 тонны за человека)
        return baseWeight + (residents.Count * 0.07f);
    }

    public bool SettleResident(Person person)
    {
        if (!HasFreeBeds || residents.Contains(person)) return false;

        residents.Add(person);
        person.homeBuilding = this;
        return true;
    }

    public void EvictResident(Person person)
    {
        if (residents.Contains(person))
        {
            residents.Remove(person);
            person.homeBuilding = null;
        }
    }

    // Заполняем информацию для тултипа
    public override string GetStatusInfo()
    {
        return $"<b>{buildingName}</b>\n" +
               $"Занято коек: {residents.Count}/{maxCapacity}\n" +
               $"Общая масса: {GetWeight():F1} т.";
    }
}