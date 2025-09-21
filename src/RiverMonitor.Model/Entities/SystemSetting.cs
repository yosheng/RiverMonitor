﻿namespace RiverMonitor.Model.Entities;

public class SystemSetting
{
    public int Id { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }

    public string? Description { get; set; }
}