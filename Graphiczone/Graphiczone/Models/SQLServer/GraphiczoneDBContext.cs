using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Graphiczone.Models.SQLServer
{
    public partial class GraphiczoneDBContext : DbContext
    {
        public GraphiczoneDBContext()
        {
        }

        public GraphiczoneDBContext(DbContextOptions<GraphiczoneDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<OrderDetailPrint> OrderDetailPrint { get; set; }
        public virtual DbSet<OrderPrint> OrderPrint { get; set; }
        public virtual DbSet<Print> Print { get; set; }
        public virtual DbSet<ProofPayment> ProofPayment { get; set; }
        public virtual DbSet<Shipping> Shipping { get; set; }
        public virtual DbSet<TypePrint> TypePrint { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("server=(local)\\SQLEXPRESS;Database=GraphiczoneDB;uid=admingraphiczone;pwd=123456");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CusId);

                entity.Property(e => e.CusId)
                    .HasColumnName("cus_id")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CusAddress)
                    .HasColumnName("cus_address")
                    .HasColumnType("text");

                entity.Property(e => e.CusEmail)
                    .HasColumnName("cus_email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CusFirstname)
                    .HasColumnName("cus_firstname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CusLastname)
                    .HasColumnName("cus_lastname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CusPassword)
                    .HasColumnName("cus_password")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CusTel)
                    .HasColumnName("cus_tel")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CusUsername)
                    .HasColumnName("cus_username")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OrderDetailPrint>(entity =>
            {
                entity.HasKey(e => e.OrdPrintId);

                entity.Property(e => e.OrdPrintId)
                    .HasColumnName("ordPrint_id")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.OrPrintId)
                    .HasColumnName("orPrint_id")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.OrdPrintDetail)
                    .HasColumnName("ordPrint_detail")
                    .HasColumnType("text");

                entity.Property(e => e.OrdPrintFile)
                    .HasColumnName("ordPrint_file")
                    .HasColumnType("text");

                entity.Property(e => e.OrdPrintHeight).HasColumnName("ordPrint_height");

                entity.Property(e => e.OrdPrintNum).HasColumnName("ordPrint_num");

                entity.Property(e => e.OrdPrintPriceset).HasColumnName("ordPrint_priceset");

                entity.Property(e => e.OrdPrintTotal).HasColumnName("ordPrint_total");

                entity.Property(e => e.OrdPrintWidth).HasColumnName("ordPrint_width");

                entity.Property(e => e.PrintId)
                    .HasColumnName("print_id")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.OrPrint)
                    .WithMany(p => p.OrderDetailPrint)
                    .HasForeignKey(d => d.OrPrintId)
                    .HasConstraintName("FK_OrderDetailPrint_OrderPrint");

                entity.HasOne(d => d.Print)
                    .WithMany(p => p.OrderDetailPrint)
                    .HasForeignKey(d => d.PrintId)
                    .HasConstraintName("FK_OrderDetailPrint_Print");
            });

            modelBuilder.Entity<OrderPrint>(entity =>
            {
                entity.HasKey(e => e.OrPrintId);

                entity.Property(e => e.OrPrintId)
                    .HasColumnName("orPrint_id")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CusId)
                    .HasColumnName("cus_id")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.OrPrintDate)
                    .HasColumnName("orPrint_date")
                    .HasColumnType("date");

                entity.Property(e => e.OrPrintDue)
                    .HasColumnName("orPrint_due")
                    .HasColumnType("date");

                entity.Property(e => e.OrPrintLocalshipping)
                    .HasColumnName("orPrint_localshipping")
                    .HasColumnType("text");

                entity.Property(e => e.OrPrintShipping).HasColumnName("orPrint_shipping");

                entity.Property(e => e.OrPrintStatus).HasColumnName("orPrint_status");

                entity.Property(e => e.OrPrintTotal).HasColumnName("orPrint_total");

                entity.HasOne(d => d.Cus)
                    .WithMany(p => p.OrderPrint)
                    .HasForeignKey(d => d.CusId)
                    .HasConstraintName("FK_OrderPrint_Customer");

                entity.HasOne(d => d.OrPrint)
                    .WithOne(p => p.OrderPrint)
                    .HasForeignKey<OrderPrint>(d => d.OrPrintId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderPrint_ProofPayment");

                entity.HasOne(d => d.OrPrintNavigation)
                    .WithOne(p => p.OrderPrint)
                    .HasForeignKey<OrderPrint>(d => d.OrPrintId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderPrint_Shipping");
            });

            modelBuilder.Entity<Print>(entity =>
            {
                entity.Property(e => e.PrintId)
                    .HasColumnName("print_id")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.PrintName)
                    .HasColumnName("print_name")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PrintPrice).HasColumnName("print_price");

                entity.Property(e => e.PrintUnit)
                    .HasColumnName("print_unit")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.TypePrintId)
                    .HasColumnName("typePrint_id")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.TypePrint)
                    .WithMany(p => p.Print)
                    .HasForeignKey(d => d.TypePrintId)
                    .HasConstraintName("FK_Print_TypePrint");
            });

            modelBuilder.Entity<ProofPayment>(entity =>
            {
                entity.HasKey(e => e.OrPrintId);

                entity.Property(e => e.OrPrintId)
                    .HasColumnName("orPrint_id")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.PrfPayBank)
                    .HasColumnName("prfPay_bank")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PrfPayDate)
                    .HasColumnName("prfPay_date")
                    .HasColumnType("date");

                entity.Property(e => e.PrfPayDetail)
                    .HasColumnName("prfPay_detail")
                    .HasColumnType("text");

                entity.Property(e => e.PrfPayFile)
                    .HasColumnName("prfPay_file")
                    .HasColumnType("text");

                entity.Property(e => e.PrfPayId)
                    .HasColumnName("prfPay_id")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PrfPayTime)
                    .HasColumnName("prfPay_time")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Shipping>(entity =>
            {
                entity.HasKey(e => e.OrPrintId);

                entity.Property(e => e.OrPrintId)
                    .HasColumnName("orPrint_id")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ShippingDate)
                    .HasColumnName("shipping_date")
                    .HasColumnType("date");

                entity.Property(e => e.ShippingFile)
                    .HasColumnName("shipping_file")
                    .HasColumnType("text");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Shipping)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Shipping_User");
            });

            modelBuilder.Entity<TypePrint>(entity =>
            {
                entity.Property(e => e.TypePrintId)
                    .HasColumnName("typePrint_id")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.TypePrintName)
                    .HasColumnName("typePrint_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.UserAddress)
                    .HasColumnName("user_address")
                    .HasColumnType("text");

                entity.Property(e => e.UserEmail)
                    .HasColumnName("user_email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserFirstname)
                    .HasColumnName("user_firstname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserLastname)
                    .HasColumnName("user_lastname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserPassword)
                    .HasColumnName("user_password")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserPosition)
                    .HasColumnName("user_position")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserSalary).HasColumnName("user_salary");

                entity.Property(e => e.UserStatus).HasColumnName("user_status");

                entity.Property(e => e.UserTel)
                    .HasColumnName("user_tel")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.UserUsername)
                    .HasColumnName("user_username")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
