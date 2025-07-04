using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Village_Manager.Models;

namespace Village_Manager.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Delivery> Deliveries { get; set; }

    public virtual DbSet<Farmer> Farmers { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<ImportInvoice> ImportInvoices { get; set; }

    public virtual DbSet<ImportInvoiceDetail> ImportInvoiceDetails { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<ProcessedProduct> ProcessedProducts { get; set; }

    public virtual DbSet<ProcessingOrder> ProcessingOrders { get; set; }

    public virtual DbSet<ProcessingReceipt> ProcessingReceipts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductProcessingHistory> ProductProcessingHistories { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<RetailCustomer> RetailCustomers { get; set; }

    public virtual DbSet<RetailOrder> RetailOrders { get; set; }

    public virtual DbSet<RetailOrderItem> RetailOrderItems { get; set; }

    public virtual DbSet<ReturnOrder> ReturnOrders { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Shipper> Shippers { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<TransportRoute> TransportRoutes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<WholesaleCustomer> WholesaleCustomers { get; set; }

    public virtual DbSet<WholesaleOrder> WholesaleOrders { get; set; }

    public virtual DbSet<WholesaleOrderItem> WholesaleOrderItems { get; set; }

    public DbSet<ProductCategory> ProductCategory { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Address__3213E83F1C6D8869");

            entity.ToTable("Address");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddressLine)
                .HasColumnType("text")
                .HasColumnName("address_line");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("postal_code");
            entity.Property(e => e.Province)
                .HasMaxLength(100)
                .HasColumnName("province");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Address__user_id__17F790F9");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart__3213E83F7FC5A18D");

            entity.ToTable("Cart");

            entity.HasIndex(e => e.UserId, "UQ__Cart__B9BE370E25CF0119").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Cart)
                .HasForeignKey<Cart>(d => d.UserId)
                .HasConstraintName("FK__Cart__user_id__7E37BEF6");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CartItem__3213E83FEBCD52EF");

            entity.ToTable("CartItem");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK__CartItem__cart_i__01142BA1");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__CartItem__produc__02084FDA");
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Delivery__3213E83F5B65E2CE");

            entity.ToTable("Delivery");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderType)
                .HasMaxLength(10)
                .HasColumnName("order_type");
            entity.Property(e => e.ShipperId).HasColumnName("shipper_id");
            entity.Property(e => e.ShippingFee)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("shipping_fee");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");

            entity.HasOne(d => d.Shipper).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.ShipperId)
                .HasConstraintName("FK__Delivery__shippe__05D8E0BE");
        });

        modelBuilder.Entity<Farmer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Farmer__3213E83F444D0B7F");

            entity.ToTable("Farmer");

            entity.HasIndex(e => e.Phone, "UQ__Farmer__B43B145FF7EA9A62").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Farmers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Farmer__user_id__4CA06362");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3213E83F811897C0");

            entity.ToTable("Feedback");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderType)
                .HasMaxLength(10)
                .HasColumnName("order_type");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Feedback__user_i__151B244E");
        });

        modelBuilder.Entity<ImportInvoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ImportIn__3213E83FE93CD895");

            entity.ToTable("ImportInvoice");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PurchaseTime)
                .HasColumnType("datetime")
                .HasColumnName("purchase_time");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(100)
                .HasColumnName("supplier_name");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.ImportInvoices)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__ImportInv__wareh__6754599E");
        });

        modelBuilder.Entity<ImportInvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ImportIn__3213E83FB89F5FD4");

            entity.ToTable("ImportInvoiceDetail");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ImportInvoiceId).HasColumnName("import_invoice_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.ImportInvoice).WithMany(p => p.ImportInvoiceDetails)
                .HasForeignKey(d => d.ImportInvoiceId)
                .HasConstraintName("FK__ImportInv__impor__6A30C649");

            entity.HasOne(d => d.Product).WithMany(p => p.ImportInvoiceDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ImportInv__produ__6B24EA82");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Log__3213E83F6ACC4231");

            entity.ToTable("Log");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasColumnType("text")
                .HasColumnName("action");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Logs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Log__user_id__2645B050");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3213E83FB46F2E38");

            entity.ToTable("Notification");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__user___236943A5");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3213E83FAFC4A48D");

            entity.ToTable("Payment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Method)
                .HasMaxLength(50)
                .HasColumnName("method");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderType)
                .HasMaxLength(10)
                .HasColumnName("order_type");
            entity.Property(e => e.PaidAt)
                .HasColumnType("datetime")
                .HasColumnName("paid_at");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(10)
                .HasColumnName("payment_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Payment__payment__1DB06A4F");
        });

        modelBuilder.Entity<ProcessedProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Processe__3213E83F34394CD7");

            entity.ToTable("ProcessedProduct");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.ProcessedDate).HasColumnName("processed_date");
            entity.Property(e => e.ProcessingNote)
                .HasColumnType("text")
                .HasColumnName("processing_note");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ReturnDate).HasColumnName("return_date");
            entity.Property(e => e.TotalWeight)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_weight");

            entity.HasOne(d => d.History).WithMany(p => p.ProcessedProducts)
                .HasForeignKey(d => d.HistoryId)
                .HasConstraintName("FK__Processed__histo__114A936A");
        });

        modelBuilder.Entity<ProcessingOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Processi__3213E83F884CF742");

            entity.ToTable("ProcessingOrder");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExpectedReturnDate).HasColumnName("expected_return_date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SendDate).HasColumnName("send_date");

            entity.HasOne(d => d.Product).WithMany(p => p.ProcessingOrders)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Processin__produ__08B54D69");
        });

        modelBuilder.Entity<ProcessingReceipt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Processi__3213E83F07ADCF84");

            entity.ToTable("ProcessingReceipt");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualQuantity).HasColumnName("actual_quantity");
            entity.Property(e => e.ProcessingOrderId).HasColumnName("processing_order_id");
            entity.Property(e => e.ReceivedDate).HasColumnName("received_date");

            entity.HasOne(d => d.ProcessingOrder).WithMany(p => p.ProcessingReceipts)
                .HasForeignKey(d => d.ProcessingOrderId)
                .HasConstraintName("FK__Processin__proce__0B91BA14");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3213E83F6AE7BBD6");

            entity.ToTable("Product");

            entity.HasIndex(e => e.Name, "UQ__Product__72E12F1BF39EFE2E").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
            entity.Property(e => e.FarmerId).HasColumnName("farmer_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProcessingTime).HasColumnName("processing_time");
            entity.Property(e => e.ProductType)
                .HasMaxLength(20)
                .HasColumnName("product_type");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__categor__59063A47");

            entity.HasOne(d => d.Farmer).WithMany(p => p.Products)
                .HasForeignKey(d => d.FarmerId)
                .HasConstraintName("FK__Product__farmer___59FA5E80");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductC__3213E83FF4BC483D");

            entity.ToTable("ProductCategory");

            entity.HasIndex(e => e.Name, "UQ__ProductC__72E12F1B9B459611").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductI__3213E83F5A9CA22B");

            entity.ToTable("ProductImage");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("uploaded_at");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductIm__produ__5DCAEF64");
        });

        modelBuilder.Entity<ProductProcessingHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductP__3213E83F8503B6DE");

            entity.ToTable("ProductProcessingHistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ReturnDate).HasColumnName("return_date");
            entity.Property(e => e.SentDate).HasColumnName("sent_date");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductProcessingHistories)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductPr__produ__0E6E26BF");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Report__3213E83F347761F2");

            entity.ToTable("Report");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
        });

        modelBuilder.Entity<RetailCustomer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RetailCu__3213E83F19D7F92A");

            entity.ToTable("RetailCustomer");

            entity.HasIndex(e => e.Phone, "UQ__RetailCu__B43B145F15A6D481").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RetailCustomers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RetailCus__user___4222D4EF");
        });

        modelBuilder.Entity<RetailOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RetailOr__3213E83F6A8D23F9");

            entity.ToTable("RetailOrder");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConfirmedAt)
                .HasColumnType("datetime")
                .HasColumnName("confirmed_at");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RetailOrders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RetailOrd__user___76969D2E");
        });

        modelBuilder.Entity<RetailOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RetailOr__3213E83F4015F15E");

            entity.ToTable("RetailOrderItem");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Order).WithMany(p => p.RetailOrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__RetailOrd__order__797309D9");

            entity.HasOne(d => d.Product).WithMany(p => p.RetailOrderItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__RetailOrd__produ__7A672E12");
        });

        modelBuilder.Entity<ReturnOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReturnOr__3213E83FC2B2761F");

            entity.ToTable("ReturnOrder");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderType)
                .HasMaxLength(10)
                .HasColumnName("order_type");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Reason)
                .HasColumnType("text")
                .HasColumnName("reason");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.ReturnOrders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ReturnOrd__user___3587F3E0");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3213E83F0A6EF93E");

            entity.HasIndex(e => e.Name, "UQ__Roles__72E12F1B152C68E6").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Session__3213E83F7438D43C");

            entity.ToTable("Session");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("expires_at");
            entity.Property(e => e.SessionToken)
                .HasColumnType("text")
                .HasColumnName("session_token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Session__user_id__29221CFB");
        });

        modelBuilder.Entity<Shipper>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shipper__3213E83F6E987342");

            entity.ToTable("Shipper");

            entity.HasIndex(e => e.Phone, "UQ__Shipper__B43B145FA0E0248B").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VehicleInfo)
                .HasColumnType("text")
                .HasColumnName("vehicle_info");

            entity.HasOne(d => d.User).WithMany(p => p.Shippers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Shipper__user_id__5165187F");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Staff__3213E83F58180E04");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedWarehouseId).HasColumnName("assigned_warehouse_id");
            entity.Property(e => e.Role)
                .HasMaxLength(100)
                .HasColumnName("role");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.AssignedWarehouse).WithMany(p => p.Staff)
                .HasForeignKey(d => d.AssignedWarehouseId)
                .HasConstraintName("FK__Staff__assigned___2EDAF651");

            entity.HasOne(d => d.User).WithMany(p => p.Staff)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Staff__user_id__2DE6D218");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Stock__3213E83F92A8E2BC");

            entity.ToTable("Stock");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastUpdated)
                .HasColumnType("datetime")
                .HasColumnName("last_updated");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Stock__product_i__6477ECF3");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__Stock__warehouse__6383C8BA");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Supplier__3213E83F577BC177");

            entity.ToTable("Supplier");

            entity.HasIndex(e => e.Name, "UQ__Supplier__72E12F1BDFEDF98E").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContactInfo)
                .HasColumnType("text")
                .HasColumnName("contact_info");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TransportRoute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transpor__3213E83F708C7DE3");

            entity.ToTable("TransportRoute");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DistanceKm)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("distance_km");
            entity.Property(e => e.EndLocation)
                .HasColumnType("text")
                .HasColumnName("end_location");
            entity.Property(e => e.EstimatedTime)
                .HasMaxLength(50)
                .HasColumnName("estimated_time");
            entity.Property(e => e.StartLocation)
                .HasColumnType("text")
                .HasColumnName("start_location");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83FF5556F8C");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164DDF1C3C0").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC5724EADD77B").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__role_id__3D5E1FD2");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Warehous__3213E83F0843A6C6");

            entity.ToTable("Warehouse");

            entity.HasIndex(e => e.Name, "UQ__Warehous__72E12F1B25E6A215").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Location)
                .HasColumnType("text")
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<WholesaleCustomer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Wholesal__3213E83F2B810801");

            entity.ToTable("WholesaleCustomer");

            entity.HasIndex(e => e.CompanyName, "UQ__Wholesal__6D1B87CB1D5ECDA6").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__Wholesal__B43B145F2155A8F4").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(100)
                .HasColumnName("company_name");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(100)
                .HasColumnName("contact_person");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.WholesaleCustomers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Wholesale__user___47DBAE45");
        });

        modelBuilder.Entity<WholesaleOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Wholesal__3213E83FC1855FA8");

            entity.ToTable("WholesaleOrder");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConfirmedAt)
                .HasColumnType("datetime")
                .HasColumnName("confirmed_at");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.WholesaleOrders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Wholesale__user___6EF57B66");
        });

        modelBuilder.Entity<WholesaleOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Wholesal__3213E83F196CF04B");

            entity.ToTable("WholesaleOrderItem");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Order).WithMany(p => p.WholesaleOrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Wholesale__order__71D1E811");

            entity.HasOne(d => d.Product).WithMany(p => p.WholesaleOrderItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Wholesale__produ__72C60C4A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
