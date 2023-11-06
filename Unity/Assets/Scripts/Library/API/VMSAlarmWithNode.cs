using System;
public class VMSAlarmWithNode : VMSAlarm
{
    public string NodeName { get; set; }

    public VMSAlarmWithNode(VMSAlarm alarm, string node)
    {
        this.Date = alarm.Date;
        this.Eu = alarm.Eu;
        this.Id= alarm.Id;
        this.Node = alarm.Node;
        this.Source = alarm.Source;
        this.Status = alarm.Status;
        this.Title = alarm.Title;
        this.Value = alarm.Value;
        this.NodeName = node;
    }
}
