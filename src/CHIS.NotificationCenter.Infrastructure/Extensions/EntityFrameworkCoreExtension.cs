//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.ChangeTracking;
//using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using CHIS.NotificationCenter.Domain.SeedWork;
//namespace CHIS.NotificationCenter.Infrastructure.Extensions
//{
//    public static class EntityFrameworkCoreExtension
//    {
//        //private static readonly MethodInfo SetIsDeletedShadowPropertyMethodInfo = typeof(EntityFrameworkCoreExtension).GetMethods(BindingFlags.Public | BindingFlags.Static)
//        //    .Single(t => t.IsGenericMethod && t.Name ==  "SetIsDeletedShadowProperty");

//        //private static readonly MethodInfo SetTenantShadowPropertyMethodInfo = typeof(EntityFrameworkCoreExtension).GetMethods(BindingFlags.Public | BindingFlags.Static)
//        //    .Single(t => t.IsGenericMethod && t.Name == "SetTenantShadowProperty");

//        private static readonly MethodInfo SetAuditingShadowPropertiesMethodInfo = typeof(EntityFrameworkCoreExtension).GetMethods(BindingFlags.Public | BindingFlags.Static)
//            .Single(t => t.IsGenericMethod && t.Name == "SetAuditingShadowProperties");

//        //private static readonly MethodInfo SetConcurrencyTokensMethodInfo = typeof(EntityFrameworkCoreExtension).GetMethods(BindingFlags.Public | BindingFlags.Static)
//        //    .Single(t => t.IsGenericMethod && t.Name == "SetConcurrencyTokens");

//        public static void ApplyGlobalConfigurations(this ModelBuilder modelBuilder)
//        {
//            if (modelBuilder is null)
//            {
//                throw new ArgumentNullException(nameof(modelBuilder));
//            }

//            foreach (var tp in modelBuilder.Model.GetEntityTypes())
//            {
//                var t = tp.ClrType;

//                // set auditing properties
//                if (typeof(IAuditedEntity).IsAssignableFrom(t))
//                {
//                    var method = SetAuditingShadowPropertiesMethodInfo.MakeGenericMethod(t);
//                    method.Invoke(modelBuilder, new object[] { modelBuilder });
//                }

//                // set tenant properties
//                //if (typeof(ITenant).IsAssignableFrom(t))
//                //{
//                //    var method = SetTenantShadowPropertyMethodInfo.MakeGenericMethod(t);
//                //    method.Invoke(modelBuilder, new object[] { modelBuilder });
//                //}

//                // set soft delete property
//                //if (typeof(ISoftDelete).IsAssignableFrom(t))
//                //{
//                //    var method = SetIsDeletedShadowPropertyMethodInfo.MakeGenericMethod(t);
//                //    method.Invoke(modelBuilder, new object[] { modelBuilder });
//                //}

//                // set concurrencyTokens
//                //if (typeof(IConcurrencyAwareEntity).IsAssignableFrom(t))
//                //{
//                //    var method = SetConcurrencyTokensMethodInfo.MakeGenericMethod(t);
//                //    method.Invoke(modelBuilder, new object[] { modelBuilder });
//                //}

//                // set maxlength for property
//                //foreach (var property in tp.GetProperties().Where(w => w.ClrType == typeof(string) && w.GetMaxLength() == null))
//                //{
//                //    property.SetMaxLength(256);
//                //}
//            }

//        }

       

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="builder"></param>
//        /// <param name="databaseProvider"></param>
//        public static void SetAuditingShadowProperties<T>(ModelBuilder builder) where T : class, IAuditedEntity
//        {
//            if (builder is null)
//            {
//                throw new ArgumentNullException(nameof(builder));
//            }

//            builder.Entity<T>().Property<string>("DataFirstRegisteredDateTime").
//            builder.Entity<T>().Property<string>("UpdateTrace").HasMaxLength(100);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="builder"></param>
//        //public static void SetConcurrencyTokens<T>(this ModelBuilder builder) where T : class, IConcurrencyAwareEntity
//        //{
//        //    if (builder is null)
//        //    {
//        //        throw new ArgumentNullException(nameof(builder));
//        //    }

//        //    var converter = new ValueConverter<byte[], DateTime>(v => new DateTime(BitConverter.ToInt64(v, 0)), v => BitConverter.GetBytes(v.Ticks));
//        //    builder.Entity<T>().Property(p => p.RowVersion).Metadata.SetValueConverter(converter);

//        //    if (builder.Entity<T>().Property(p => p.RowVersion).Metadata.ValueGenerated != Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate)
//        //    {
//        //        builder.Entity<T>().Property(p => p.RowVersion).Metadata.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate;
//        //    }

//        //    if (builder.Entity<T>().Property(p => p.RowVersion).Metadata.IsConcurrencyToken)
//        //    {
//        //        builder.Entity<T>().Property(p => p.RowVersion).Metadata.IsConcurrencyToken = true;
//        //    }
//        //}

//        public static void SetShadowProperties(this ChangeTracker changeTracker, ICallContext callcontext)
//        {
//            changeTracker.DetectChanges();

//            var timestamp = DateTime.UtcNow;

//            foreach (var entry in changeTracker.Entries())
//            {
//                if (entry.Entity is IAuditedEntity)
//                {
//                    if (entry.State == EntityState.Added)
//                    {
//                        CreateTrace trace = callcontext.GetCreateTrace();

//                        entry.Property("CreateTrace").CurrentValue = JsonConvert.SerializeObject(trace);
//                    }

//                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
//                    {
//                        UpdateTrace trace = callcontext.GetUpdateTrace();

//                        entry.Property("UpdateTrace").CurrentValue = JsonConvert.SerializeObject(trace);
//                    }
//                }

//                //if (entry.State == EntityState.Added && entry.Entity is ITenant)
//                //{
//                //    entry.Property("TenantId").CurrentValue = callcontext.TenantId;
//                //}


//                //if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete)
//                //{
//                //    entry.State = EntityState.Modified;
//                //    entry.Property("IsDeleted").CurrentValue = true;
//                //}
//            }
//        }

//    }
//}
