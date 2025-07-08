﻿using System;
using System.Collections.Generic;

namespace Village_Manager.Models;

public partial class UserLocation
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Label { get; set; }

    public string? Address { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
