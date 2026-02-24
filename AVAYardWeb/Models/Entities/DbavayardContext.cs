using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AVAYardWeb.Models.Entities;

public partial class DbavayardContext : DbContext
{
    public DbavayardContext(DbContextOptions<DbavayardContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccAccount> AccAccounts { get; set; }

    public virtual DbSet<AccAccountLine> AccAccountLines { get; set; }

    public virtual DbSet<GenerateCode> GenerateCodes { get; set; }

    public virtual DbSet<LogAction> LogActions { get; set; }

    public virtual DbSet<LogSystem> LogSystems { get; set; }

    public virtual DbSet<OrderBank> OrderBanks { get; set; }

    public virtual DbSet<OrderContainer> OrderContainers { get; set; }

    public virtual DbSet<OrderContainerLocation> OrderContainerLocations { get; set; }

    public virtual DbSet<OrderContainerMatchdetail> OrderContainerMatchdetails { get; set; }

    public virtual DbSet<OrderContainerRepair> OrderContainerRepairs { get; set; }

    public virtual DbSet<OrderContainerStatus> OrderContainerStatuses { get; set; }

    public virtual DbSet<OrderPayment> OrderPayments { get; set; }

    public virtual DbSet<OrderPaymentDetail> OrderPaymentDetails { get; set; }

    public virtual DbSet<OrderPaymentStage> OrderPaymentStages { get; set; }

    public virtual DbSet<OrderPaymentType> OrderPaymentTypes { get; set; }

    public virtual DbSet<OrderReceipt> OrderReceipts { get; set; }

    public virtual DbSet<OrderReceiptType> OrderReceiptTypes { get; set; }

    public virtual DbSet<OrderRepairStatus> OrderRepairStatuses { get; set; }

    public virtual DbSet<OrderTransaction> OrderTransactions { get; set; }

    public virtual DbSet<OrderTransactionDetail> OrderTransactionDetails { get; set; }

    public virtual DbSet<OrderTransactionFile> OrderTransactionFiles { get; set; }

    public virtual DbSet<RateDropService> RateDropServices { get; set; }

    public virtual DbSet<RateMatchService> RateMatchServices { get; set; }

    public virtual DbSet<TaxAddress> TaxAddresses { get; set; }

    public virtual DbSet<TaxTransportation> TaxTransportations { get; set; }

    public virtual DbSet<TransAgent> TransAgents { get; set; }

    public virtual DbSet<TransContainerSize> TransContainerSizes { get; set; }

    public virtual DbSet<TransContainerType> TransContainerTypes { get; set; }

    public virtual DbSet<TransTransportation> TransTransportations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Thai_100_CI_AS");

        modelBuilder.Entity<AccAccount>(entity =>
        {
            entity.HasKey(e => e.AccUsername);

            entity.ToTable("acc_account");

            entity.Property(e => e.AccUsername)
                .HasMaxLength(20)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("acc_username");
            entity.Property(e => e.AccFullname)
                .HasMaxLength(150)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("acc_fullname");
            entity.Property(e => e.AccGroup)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("acc_group");
            entity.Property(e => e.AccPassword)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("acc_password");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActived).HasColumnName("is_actived");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.Remark)
                .UseCollation("Thai_CI_AS")
                .HasColumnType("text")
                .HasColumnName("remark");
        });

        modelBuilder.Entity<AccAccountLine>(entity =>
        {
            entity.HasKey(e => new { e.AccUsername, e.LineId });

            entity.ToTable("acc_account_line");

            entity.Property(e => e.AccUsername)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("acc_username");
            entity.Property(e => e.LineId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("line_id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActived).HasColumnName("is_actived");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
        });

        modelBuilder.Entity<GenerateCode>(entity =>
        {
            entity.HasKey(e => e.GenCode);

            entity.ToTable("generate_code");

            entity.Property(e => e.GenCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("gen_code");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.GenType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("gen_type");
        });

        modelBuilder.Entity<LogAction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__log_acti__3213E83F8990F03D");

            entity.ToTable("log_action");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.AfterData).HasColumnName("after_data");
            entity.Property(e => e.BeforeData).HasColumnName("before_data");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Module)
                .HasMaxLength(100)
                .HasColumnName("module");
            entity.Property(e => e.RefCode)
                .HasMaxLength(100)
                .HasColumnName("ref_code");
        });

        modelBuilder.Entity<LogSystem>(entity =>
        {
            entity.ToTable("log_system");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.LogAction)
                .UseCollation("Thai_CI_AS")
                .HasColumnType("text")
                .HasColumnName("log_action");
            entity.Property(e => e.LogReferenceCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("log_reference_code");
        });

        modelBuilder.Entity<OrderBank>(entity =>
        {
            entity.HasKey(e => e.BankCode);

            entity.ToTable("order_bank");

            entity.Property(e => e.BankCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .UseCollation("Thai_CI_AS")
                .HasColumnName("bank_code");
            entity.Property(e => e.BankName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("bank_name");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
        });

        modelBuilder.Entity<OrderContainer>(entity =>
        {
            entity.HasKey(e => e.OrderCode);

            entity.ToTable("order_container");

            entity.Property(e => e.OrderCode)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("order_code");
            entity.Property(e => e.AgentCode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("agent_code");
            entity.Property(e => e.BookingNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("booking_no");
            entity.Property(e => e.ContainerNo)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("container_no");
            entity.Property(e => e.ContainerSizeCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("container_size_code");
            entity.Property(e => e.ContainerStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("container_status");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsApprove).HasColumnName("is_approve");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.IsExchange).HasColumnName("is_exchange");
            entity.Property(e => e.IsReceipt).HasColumnName("is_receipt");
            entity.Property(e => e.IssueDate)
                .HasColumnType("datetime")
                .HasColumnName("issue_date");
            entity.Property(e => e.IssueType)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("issue_type");
            entity.Property(e => e.PaymentStageCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("payment_stage_code");
            entity.Property(e => e.Remark)
                .HasColumnType("text")
                .HasColumnName("remark");
            entity.Property(e => e.SealNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("seal_no");
            entity.Property(e => e.TareWeight).HasColumnName("tare_weight");
            entity.Property(e => e.TransportationCode)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("transportation_code");
            entity.Property(e => e.TruckLicense)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("truck_license");

            entity.HasOne(d => d.AgentCodeNavigation).WithMany(p => p.OrderContainers)
                .HasForeignKey(d => d.AgentCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_container_trans_agent");

            entity.HasOne(d => d.ContainerSizeCodeNavigation).WithMany(p => p.OrderContainers)
                .HasForeignKey(d => d.ContainerSizeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_container_trans_container_size");

            entity.HasOne(d => d.TransportationCodeNavigation).WithMany(p => p.OrderContainers)
                .HasForeignKey(d => d.TransportationCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_container_trans_transportation");
        });

        modelBuilder.Entity<OrderContainerLocation>(entity =>
        {
            entity.HasKey(e => e.ContainerNo).HasName("PK_order_container_location_1");

            entity.ToTable("order_container_location");

            entity.Property(e => e.ContainerNo)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("container_no");
            entity.Property(e => e.ContainerSizeCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("container_size_code");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.LocationStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("location_status");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("order_code");

            entity.HasOne(d => d.ContainerSizeCodeNavigation).WithMany(p => p.OrderContainerLocations)
                .HasForeignKey(d => d.ContainerSizeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_container_location_trans_container_size");

            entity.HasOne(d => d.OrderCodeNavigation).WithMany(p => p.OrderContainerLocations)
                .HasForeignKey(d => d.OrderCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_container_location_order_container1");
        });

        modelBuilder.Entity<OrderContainerMatchdetail>(entity =>
        {
            entity.HasKey(e => e.OrderCode);

            entity.ToTable("order_container_matchdetail");

            entity.Property(e => e.OrderCode)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("order_code");
            entity.Property(e => e.DetentionDate).HasColumnName("detention_date");
            entity.Property(e => e.MatchType)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("match_type");

            entity.HasOne(d => d.OrderCodeNavigation).WithOne(p => p.OrderContainerMatchdetail)
                .HasForeignKey<OrderContainerMatchdetail>(d => d.OrderCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_container_matchdetail_order_container");
        });

        modelBuilder.Entity<OrderContainerRepair>(entity =>
        {
            entity.HasKey(e => e.OrderCode);

            entity.ToTable("order_container_repair");

            entity.Property(e => e.OrderCode)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("order_code");
            entity.Property(e => e.ContainerNo)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("container_no");
            entity.Property(e => e.RepairStatusCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("repair_status_code");

            entity.HasOne(d => d.OrderCodeNavigation).WithOne(p => p.OrderContainerRepair)
                .HasForeignKey<OrderContainerRepair>(d => d.OrderCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_container_repair_order_container");

            entity.HasOne(d => d.RepairStatusCodeNavigation).WithMany(p => p.OrderContainerRepairs)
                .HasForeignKey(d => d.RepairStatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_container_repair_order_container_repair");
        });

        modelBuilder.Entity<OrderContainerStatus>(entity =>
        {
            entity.HasKey(e => e.ContainerStatusCode);

            entity.ToTable("order_container_status");

            entity.Property(e => e.ContainerStatusCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("container_status_code");
            entity.Property(e => e.ContainerStatusName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("container_status_name");
        });

        modelBuilder.Entity<OrderPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentCode);

            entity.ToTable("order_payment");

            entity.Property(e => e.PaymentCode)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("payment_code");
            entity.Property(e => e.BankCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("bank_code");
            entity.Property(e => e.ContainerNo)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("container_no");
            entity.Property(e => e.ContainerSizeCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("container_size_code");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsPaid).HasColumnName("is_paid");
            entity.Property(e => e.IsTaxInvoice).HasColumnName("is_tax_invoice");
            entity.Property(e => e.IssueType)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("issue_type");
            entity.Property(e => e.NetTotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("net_total");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("order_code");
            entity.Property(e => e.PaymentTypeCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("payment_type_code");
            entity.Property(e => e.TaxAddress)
                .HasMaxLength(250)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("tax_address");
            entity.Property(e => e.TaxId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("tax_id");
            entity.Property(e => e.TaxName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("tax_name");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total");
            entity.Property(e => e.TransportationCode)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("transportation_code");
            entity.Property(e => e.TruckLicense)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("truck_license");
            entity.Property(e => e.Vat)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("vat");

            entity.HasOne(d => d.ContainerSizeCodeNavigation).WithMany(p => p.OrderPayments)
                .HasForeignKey(d => d.ContainerSizeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_payment_trans_container_size");

            entity.HasOne(d => d.OrderCodeNavigation).WithMany(p => p.OrderPayments)
                .HasForeignKey(d => d.OrderCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_payment_order_container");

            entity.HasOne(d => d.PaymentTypeCodeNavigation).WithMany(p => p.OrderPayments)
                .HasForeignKey(d => d.PaymentTypeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_payment_order_payment_type");

            entity.HasOne(d => d.TransportationCodeNavigation).WithMany(p => p.OrderPayments)
                .HasForeignKey(d => d.TransportationCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_payment_trans_transportation");
        });

        modelBuilder.Entity<OrderPaymentDetail>(entity =>
        {
            entity.HasKey(e => e.PaymentCode).HasName("PK_order_payment_detail_1");

            entity.ToTable("order_payment_detail");

            entity.Property(e => e.PaymentCode)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("payment_code");
            entity.Property(e => e.Cost10Days)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("cost_10_days");
            entity.Property(e => e.Cost7Days)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("cost_7_days");
            entity.Property(e => e.Overstay10Days).HasColumnName("overstay_10_days");
            entity.Property(e => e.Overstay7Days).HasColumnName("overstay_7_days");
            entity.Property(e => e.RepairCharge)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("repair_charge");
            entity.Property(e => e.ServiceCharge)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("service_charge");
            entity.Property(e => e.StayDays).HasColumnName("stay_days");

            entity.HasOne(d => d.PaymentCodeNavigation).WithOne(p => p.OrderPaymentDetail)
                .HasForeignKey<OrderPaymentDetail>(d => d.PaymentCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_payment_detail_order_payment_detail");
        });

        modelBuilder.Entity<OrderPaymentStage>(entity =>
        {
            entity.HasKey(e => e.PaymentStageCode);

            entity.ToTable("order_payment_stage");

            entity.Property(e => e.PaymentStageCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("payment_stage_code");
            entity.Property(e => e.PaymentStageName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("payment_stage_name");
        });

        modelBuilder.Entity<OrderPaymentType>(entity =>
        {
            entity.HasKey(e => e.PaymentTypeCode);

            entity.ToTable("order_payment_type");

            entity.Property(e => e.PaymentTypeCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("payment_type_code");
            entity.Property(e => e.PaymentTypeName)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("payment_type_name");
        });

        modelBuilder.Entity<OrderReceipt>(entity =>
        {
            entity.HasKey(e => e.ReceiptCode);

            entity.ToTable("order_receipt");

            entity.Property(e => e.ReceiptCode)
                .HasMaxLength(13)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("receipt_code");
            entity.Property(e => e.BankBranchName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("bank_branch_name");
            entity.Property(e => e.BankName)
                .HasMaxLength(70)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("bank_name");
            entity.Property(e => e.ChequeDate).HasColumnName("cheque_date");
            entity.Property(e => e.ChequeNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("cheque_no");
            entity.Property(e => e.ContainerNo)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("container_no");
            entity.Property(e => e.ContainerSizeCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("container_size_code");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Description)
                .HasMaxLength(120)
                .HasColumnName("description");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.IsTaxInvoice).HasColumnName("is_tax_invoice");
            entity.Property(e => e.IssueSession)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("issue_session");
            entity.Property(e => e.NetTotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("net_total");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("order_code");
            entity.Property(e => e.PaymentCode)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("payment_code");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .UseCollation("Thai_CI_AS")
                .HasColumnName("payment_type");
            entity.Property(e => e.ReceiptDate).HasColumnName("receipt_date");
            entity.Property(e => e.Remark)
                .HasColumnType("text")
                .HasColumnName("remark");
            entity.Property(e => e.TaxAddress)
                .HasMaxLength(250)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("tax_address");
            entity.Property(e => e.TaxId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("tax_id");
            entity.Property(e => e.TaxName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("tax_name");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total");
            entity.Property(e => e.Vat)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("vat");

            entity.HasOne(d => d.ContainerSizeCodeNavigation).WithMany(p => p.OrderReceipts)
                .HasForeignKey(d => d.ContainerSizeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_receipt_trans_container_size");

            entity.HasOne(d => d.OrderCodeNavigation).WithMany(p => p.OrderReceipts)
                .HasForeignKey(d => d.OrderCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_receipt_order_container");

            entity.HasOne(d => d.PaymentCodeNavigation).WithMany(p => p.OrderReceipts)
                .HasForeignKey(d => d.PaymentCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_receipt_order_payment");
        });

        modelBuilder.Entity<OrderReceiptType>(entity =>
        {
            entity.HasKey(e => e.ReceiptTypeCode);

            entity.ToTable("order_receipt_type");

            entity.Property(e => e.ReceiptTypeCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .UseCollation("Thai_CI_AS")
                .HasColumnName("receipt_type_code");
            entity.Property(e => e.ReceiptTypeName)
                .HasMaxLength(70)
                .IsUnicode(false)
                .UseCollation("Thai_CI_AS")
                .HasColumnName("receipt_type_name");
        });

        modelBuilder.Entity<OrderRepairStatus>(entity =>
        {
            entity.HasKey(e => e.RepairStatusCode);

            entity.ToTable("order_repair_status");

            entity.Property(e => e.RepairStatusCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("repair_status_code");
            entity.Property(e => e.RepairStatusName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("repair_status_name");
        });

        modelBuilder.Entity<OrderTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionCode);

            entity.ToTable("order_transaction");

            entity.Property(e => e.TransactionCode)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("transaction_code");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("order_code");
            entity.Property(e => e.TransactionDate)
                .HasColumnType("datetime")
                .HasColumnName("transaction_date");
            entity.Property(e => e.TransactionStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("transaction_status");
        });

        modelBuilder.Entity<OrderTransactionDetail>(entity =>
        {
            entity.ToTable("order_transaction_detail");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CheckedResult)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("checked_result");
            entity.Property(e => e.TransactionCode)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("transaction_code");

            entity.HasOne(d => d.TransactionCodeNavigation).WithMany(p => p.OrderTransactionDetails)
                .HasForeignKey(d => d.TransactionCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_transaction_detail_order_transaction_detail");
        });

        modelBuilder.Entity<OrderTransactionFile>(entity =>
        {
            entity.ToTable("order_transaction_file");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FileName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("file_path");
            entity.Property(e => e.FileType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("file_type");
            entity.Property(e => e.TransactionCode)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("transaction_code");

            entity.HasOne(d => d.TransactionCodeNavigation).WithMany(p => p.OrderTransactionFiles)
                .HasForeignKey(d => d.TransactionCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_container_order_transaction_file");
        });

        modelBuilder.Entity<RateDropService>(entity =>
        {
            entity.ToTable("rate_drop_service");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContainerTypeCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("container_type_code");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.RateServiceCharge)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("rate_service_charge");

            entity.HasOne(d => d.ContainerTypeCodeNavigation).WithMany(p => p.RateDropServices)
                .HasForeignKey(d => d.ContainerTypeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_rate_drop_service_trans_container_type");
        });

        modelBuilder.Entity<RateMatchService>(entity =>
        {
            entity.ToTable("rate_match_service");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AgentCode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("agent_code");
            entity.Property(e => e.ContainerTypeCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("container_type_code");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.Limit10Days)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("limit_10_days");
            entity.Property(e => e.Limit7Days)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("limit_7_days");
            entity.Property(e => e.MatchType)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("match_type");
            entity.Property(e => e.RateServiceCharge)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("rate_service_charge");

            entity.HasOne(d => d.AgentCodeNavigation).WithMany(p => p.RateMatchServices)
                .HasForeignKey(d => d.AgentCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_rate_match_service_trans_agent");

            entity.HasOne(d => d.ContainerTypeCodeNavigation).WithMany(p => p.RateMatchServices)
                .HasForeignKey(d => d.ContainerTypeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_rate_match_service_trans_container_type");
        });

        modelBuilder.Entity<TaxAddress>(entity =>
        {
            entity.HasKey(e => e.TaxId);

            entity.ToTable("tax_address");

            entity.Property(e => e.TaxId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tax_id");
            entity.Property(e => e.Acronym)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("acronym");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActived).HasColumnName("is_actived");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<TaxTransportation>(entity =>
        {
            entity.HasKey(e => new { e.TrasportationCode, e.TaxId });

            entity.ToTable("tax_transportation");

            entity.Property(e => e.TrasportationCode)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("trasportation_code");
            entity.Property(e => e.TaxId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tax_id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");

            entity.HasOne(d => d.Tax).WithMany(p => p.TaxTransportations)
                .HasForeignKey(d => d.TaxId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tax_transportation_tax_address");

            entity.HasOne(d => d.TrasportationCodeNavigation).WithMany(p => p.TaxTransportations)
                .HasForeignKey(d => d.TrasportationCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tax_transportation_trans_transportation");
        });

        modelBuilder.Entity<TransAgent>(entity =>
        {
            entity.HasKey(e => e.AgentCode);

            entity.ToTable("trans_agent");

            entity.Property(e => e.AgentCode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("agent_code");
            entity.Property(e => e.AgentName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("agent_name");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActived).HasColumnName("is_actived");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
        });

        modelBuilder.Entity<TransContainerSize>(entity =>
        {
            entity.HasKey(e => e.ContainerSizeCode).HasName("PK_trans_container_type");

            entity.ToTable("trans_container_size");

            entity.Property(e => e.ContainerSizeCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("container_size_code");
            entity.Property(e => e.ContainerSizeName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("container_size_name");
            entity.Property(e => e.ContainerTypeCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("container_type_code");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActived).HasColumnName("is_actived");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");

            entity.HasOne(d => d.ContainerTypeCodeNavigation).WithMany(p => p.TransContainerSizes)
                .HasForeignKey(d => d.ContainerTypeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_trans_container_size_trans_container_type");
        });

        modelBuilder.Entity<TransContainerType>(entity =>
        {
            entity.HasKey(e => e.ContainerTypeCode).HasName("PK_trans_container_type_1");

            entity.ToTable("trans_container_type");

            entity.Property(e => e.ContainerTypeCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("container_type_code");
            entity.Property(e => e.ContainerTypeName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("container_type_name");
        });

        modelBuilder.Entity<TransTransportation>(entity =>
        {
            entity.HasKey(e => e.TransportationCode);

            entity.ToTable("trans_transportation");

            entity.Property(e => e.TransportationCode)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("transportation_code");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActived).HasColumnName("is_actived");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Remark).HasColumnName("remark");
            entity.Property(e => e.TransportationAcronym)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("transportation_acronym");
            entity.Property(e => e.TransportationName)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("transportation_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
