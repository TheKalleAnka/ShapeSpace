using System;

interface Observer
{
    void OnNotify(Object caller, string eventID);
}
