using System.Collections.Generic;

interface Subject
{
    List<Observer> observers{get; set;}
    void AddObserver(Observer observer);
    void RemoveObserver(Observer observer);
}
