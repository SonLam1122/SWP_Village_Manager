using System;
using System.Collections.Generic;

namespace Village_Manager.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int RoleId { get; set; }

    public bool HasAcceptedGeolocation { get; set; }

    public string? Phone { get; set; }

    public DateTime? CreatedAt { get; set; }


    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual Cart? Cart { get; set; }

    public virtual ICollection<Farmer> Farmers { get; set; } = new List<Farmer>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<RetailCustomer> RetailCustomers { get; set; } = new List<RetailCustomer>();

    public virtual ICollection<RetailOrder> RetailOrders { get; set; } = new List<RetailOrder>();

    public virtual ICollection<ReturnOrder> ReturnOrders { get; set; } = new List<ReturnOrder>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

   

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
