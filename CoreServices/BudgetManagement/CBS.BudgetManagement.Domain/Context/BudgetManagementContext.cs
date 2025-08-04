using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBS.BudgetManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace CBS.BudgetManagement.Domain
{


    public class BudgetManagementContext : DbContext
    {
        public BudgetManagementContext(DbContextOptions<BudgetManagementContext> options)
            : base(options)
        {
        }


        public DbSet<FiscalYear> FiscalYears { get; set; }
        public DbSet<BudgetPlan> BudgetPlans { get; set; }
        public DbSet<BudgetItem> BudgetItems { get; set; }
        public DbSet<Expenditure> Expenditures { get; set; }
        public DbSet<BudgetAdjustment> BudgetAdjustments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SpendingLimit> SpendingLimits { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectBudget> ProjectBudgets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



            // FiscalYear configuration
            modelBuilder.Entity<FiscalYear>()
                .HasIndex(fy => new { fy.StartDate, fy.EndDate })
                .IsUnique();

            // BudgetPlan configuration
            modelBuilder.Entity<BudgetPlan>()
                .HasIndex(bp => new { bp.DepartmentId, bp.FiscalYearId })
                .IsUnique();

            // BudgetItem configuration
            modelBuilder.Entity<BudgetItem>()
                .HasOne(bi => bi.BudgetPlan)
                .WithMany(bp => bp.BudgetItems)
                .HasForeignKey(bi => bi.BudgetPlanID)
                .OnDelete(DeleteBehavior.Cascade);



            // BudgetAdjustment configuration
            modelBuilder.Entity<BudgetAdjustment>()
                .HasOne(ba => ba.BudgetPlan)
                .WithMany(bp => bp.BudgetAdjustments)
                .HasForeignKey(ba => ba.BudgetPlanID)
                .OnDelete(DeleteBehavior.Restrict);


            // SpendingLimit configuration
            modelBuilder.Entity<SpendingLimit>()
                .HasIndex(sl => new { sl.DepartmentId, sl.FiscalYearId })
                .IsUnique();

            // Project configuration
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.ProjectName)
                .IsUnique();

            // ProjectBudget configuration
            modelBuilder.Entity<ProjectBudget>()
                .HasKey(pb => new { pb.ProjectId, pb.FiscalYearId });
        }
    }
}