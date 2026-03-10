using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class SiteSetting
{
    public int SiteSettingId { get; set; }

    public string SettingKey { get; set; } = null!;

    public string? SettingValue { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
