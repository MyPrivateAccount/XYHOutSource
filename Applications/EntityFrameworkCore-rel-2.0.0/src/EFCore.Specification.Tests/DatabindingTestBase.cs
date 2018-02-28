// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore.TestModels.ConcurrencyModel;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public abstract class DatabindingTestBase<TTestStore, TFixture> : IClassFixture<TFixture>, IDisposable
        where TTestStore : TestStore
        where TFixture : F1FixtureBase<TTestStore>, new()
    {
        protected DatabindingTestBase(TFixture fixture)
        {
            Fixture = fixture;

            TestStore = Fixture.CreateTestStore();
        }

        protected const int TotalCount = 40;

        protected const int DeletedTeam = Team.Hispania;
        protected const int DeletedCount = 4;

        protected const int ModifedTeam = Team.Lotus;
        protected const int ModifiedCount = 3;

        protected const int AddedTeam = Team.Sauber;
        protected const int AddedCount = 2;

        protected const int UnchangedTeam = Team.Mercedes;
        protected const int UnchangedCount = 3;

        protected void SetupContext(F1Context context)
        {
            var drivers = context.Drivers;
            drivers.Load();

            foreach (var driver in drivers.Local.Where(d => d.TeamId == DeletedTeam).ToList())
            {
                drivers.Remove(driver);
            }

            foreach (var driver in drivers.Local.Where(d => d.TeamId == ModifedTeam).ToList())
            {
                driver.Races = 5;
            }

            drivers.Add(
                new Driver
                {
                    Name = "Pedro de la Rosa",
                    TeamId = AddedTeam,
                    CarNumber = 13
                });
            drivers.Add(
                new Driver
                {
                    Name = "Kamui Kobayashi",
                    TeamId = AddedTeam,
                    CarNumber = null
                });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void DbSet_Local_contains_Unchanged_Modified_and_Added_entities_but_not_Deleted_entities(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(0, local.Count(d => d.TeamId == DeletedTeam));
                Assert.Equal(ModifiedCount, local.Count(d => d.TeamId == ModifedTeam));
                Assert.Equal(UnchangedCount, local.Count(d => d.TeamId == UnchangedTeam));
                Assert.Equal(AddedCount, local.Count(d => d.TeamId == AddedTeam));
                Assert.Equal(TotalCount, local.Count);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Adding_entity_to_context_is_reflected_in_local_view(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(0, local.Count);

                var larry = new Driver
                {
                    Name = "Larry David",
                    TeamId = Team.Ferrari,
                    CarNumber = 13
                };
                context.Drivers.Add(larry);

                Assert.Equal(1, local.Count);
                Assert.True(local.Contains(larry));
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Attaching_entity_to_context_is_reflected_in_local_view(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(0, local.Count);

                var larry = new Driver
                {
                    Name = "Larry David",
                    TeamId = Team.Ferrari,
                    CarNumber = 13
                };
                context.Drivers.Attach(larry);

                Assert.Equal(1, local.Count);
                Assert.True(local.Contains(larry));
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Entities_materialized_into_context_are_reflected_in_local_view(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(0, local.Count);

                context.Drivers.Where(d => d.TeamId == UnchangedTeam).Load();

                Assert.Equal(UnchangedCount, local.Count);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Entities_detached_from_context_are_removed_from_local_view(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(TotalCount, local.Count);

                foreach (var driver in context.Drivers.Local.Where(d => d.TeamId == UnchangedTeam).ToList())
                {
                    context.Entry(driver).State = EntityState.Detached;
                }

                Assert.Equal(0, local.Count(d => d.TeamId == UnchangedTeam));
                Assert.Equal(TotalCount - UnchangedCount, local.Count);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Entities_deleted_from_context_are_removed_from_local_view(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(TotalCount, local.Count);

                foreach (var driver in context.Drivers.Local.Where(d => d.TeamId == UnchangedTeam).ToList())
                {
                    context.Drivers.Remove(driver);
                }

                Assert.Equal(0, local.Count(d => d.TeamId == UnchangedTeam));
                Assert.Equal(TotalCount - UnchangedCount, local.Count);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Entities_with_state_changed_to_deleted_are_removed_from_local_view(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(TotalCount, local.Count);

                foreach (var driver in context.Drivers.Local.Where(d => d.TeamId == UnchangedTeam).ToList())
                {
                    context.Entry(driver).State = EntityState.Deleted;
                }

                Assert.Equal(0, local.Count(d => d.TeamId == UnchangedTeam));
                Assert.Equal(TotalCount - UnchangedCount, local.Count);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Entities_with_state_changed_to_detached_are_removed_from_local_view(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(TotalCount, local.Count);

                foreach (var driver in context.Drivers.Local.Where(d => d.TeamId == UnchangedTeam).ToList())
                {
                    context.Entry(driver).State = EntityState.Detached;
                }

                Assert.Equal(0, local.Count(d => d.TeamId == UnchangedTeam));
                Assert.Equal(TotalCount - UnchangedCount, local.Count);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Entities_with_state_changed_from_deleted_to_added_are_added_to_local_view(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(TotalCount, local.Count);

                foreach (var driver in context.Drivers.Where(d => d.TeamId == DeletedTeam).ToList())
                {
                    context.Entry(driver).State = EntityState.Added;
                }

                Assert.Equal(DeletedCount, local.Count(d => d.TeamId == DeletedTeam));
                Assert.Equal(TotalCount + DeletedCount, local.Count);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Entities_with_state_changed_from_deleted_to_unchanged_are_added_to_local_view(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(TotalCount, local.Count);

                foreach (var driver in context.Drivers.Where(d => d.TeamId == DeletedTeam).ToList())
                {
                    context.Entry(driver).State = EntityState.Unchanged;
                }

                Assert.Equal(DeletedCount, local.Count(d => d.TeamId == DeletedTeam));
                Assert.Equal(TotalCount + DeletedCount, local.Count);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Entities_added_to_local_view_are_added_to_state_manager(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(0, local.Count);

                var larry = new Driver
                {
                    Id = -1,
                    Name = "Larry David",
                    TeamId = Team.Ferrari,
                    CarNumber = 13
                };

                local.Add(larry);

                Assert.Same(larry, context.Drivers.Find(-1));
                Assert.Equal(EntityState.Added, context.Entry(larry).State);
                Assert.Equal(1, local.Count);
                Assert.Equal(1, localView.Count);
                Assert.Contains(larry, local);
                Assert.Contains(larry, localView);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Entities_removed_from_the_local_view_are_marked_deleted_in_the_state_manager(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(TotalCount, local.Count);

                var alonso = local.Single(d => d.Name == "Fernando Alonso");

                local.Remove(alonso);

                Assert.Equal(EntityState.Deleted, context.Entry(alonso).State);

                Assert.Equal(TotalCount - 1, local.Count);
                Assert.Equal(TotalCount - 1, localView.Count);
                Assert.DoesNotContain(alonso, local);
                Assert.DoesNotContain(alonso, localView);
            }
        }

        [Fact]
        public virtual void Adding_entity_to_local_view_that_is_already_in_the_state_manager_and_not_Deleted_is_noop()
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var local = context.Drivers.Local;

                Assert.Equal(TotalCount, local.Count);

                var alonso = local.Single(d => d.Name == "Fernando Alonso");

                local.Add(alonso);

                Assert.Equal(EntityState.Unchanged, context.Entry(alonso).State);
                Assert.Equal(TotalCount, local.Count);
                Assert.Contains(alonso, local);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Adding_entity_to_local_view_that_is_Deleted_in_the_state_manager_makes_entity_Added(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(TotalCount, local.Count);
                Assert.Equal(TotalCount, localView.Count);

                var deletedDrivers = context.Drivers.Where(d => d.TeamId == DeletedTeam).ToList();

                foreach (var driver in deletedDrivers)
                {
                    local.Add(driver);
                }

                Assert.True(deletedDrivers.TrueForAll(d => context.Entry(d).State == EntityState.Added));
                Assert.Equal(TotalCount + DeletedCount, local.Count);
                Assert.Equal(TotalCount + DeletedCount, localView.Count);

                foreach (var driver in deletedDrivers)
                {
                    Assert.Contains(driver, local);
                    Assert.Contains(driver, localView);
                }
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Adding_entity_to_state_manager_of_different_type_than_local_view_type_has_no_effect_on_local_view(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                Assert.Equal(TotalCount, local.Count);

                context.Teams.Add(
                    new Team
                    {
                        Id = -1,
                        Name = "Wubbsy Racing"
                    });

                Assert.Equal(TotalCount, local.Count);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public virtual void Adding_entity_to_state_manager_of_subtype_still_shows_up_in_local_view(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                context.Drivers.Load();

                var localView = context.Drivers.Local;
                var local = toObservableCollection
                    ? (ICollection<Driver>)localView.ToObservableCollection()
                    : localView;

                var newDriver = new TestDriver();
                context.Drivers.Add(newDriver);

                Assert.True(local.Contains(newDriver));
            }
        }

        [Fact]
        public virtual void DbSet_Local_is_cached_on_the_set()
        {
            using (var context = CreateF1Context())
            {
                var local = context.Drivers.Local;

                Assert.Same(local, context.Drivers.Local);
            }
        }

        [Fact]
        public virtual void DbSet_Local_does_not_call_DetectChanges()
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var alonso = context.Drivers.Single(d => d.Name == "Fernando Alonso");
                alonso.CarNumber = 13;

                Assert.Equal(EntityState.Unchanged, context.Entry(alonso).State);

                context.ChangeTracker.AutoDetectChangesEnabled = true;

                _ = context.Drivers.Local;

                context.ChangeTracker.AutoDetectChangesEnabled = false;

                Assert.Equal(EntityState.Unchanged, context.Entry(alonso).State);

                context.ChangeTracker.AutoDetectChangesEnabled = true;

                Assert.Equal(EntityState.Modified, context.Entry(alonso).State);
            }
        }

        [Fact]
        public virtual void Load_executes_query_on_DbQuery()
        {
            using (var context = CreateF1Context())
            {
                context.Drivers.Where(d => d.TeamId == UnchangedTeam).Load();

                Assert.Equal(UnchangedCount, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void LocalView_is_initialized_with_entities_from_the_context(
            bool toObservableCollection)
        {
            using (var context = CreateF1Context())
            {
                context.Drivers.Load();
                context.Set<TestDriver>().Load();
                context.Teams.Load();

                var driversLocal = toObservableCollection
                    ? (ICollection<Driver>)context.Drivers.Local.ToObservableCollection()
                    : context.Drivers.Local;

                var testDriversLocal = toObservableCollection
                    ? (ICollection<TestDriver>)context.Set<TestDriver>().Local.ToObservableCollection()
                    : context.Set<TestDriver>().Local;

                var teamsLocal = toObservableCollection
                    ? (ICollection<Team>)context.Teams.Local.ToObservableCollection()
                    : context.Teams.Local;

                Assert.Equal(42, driversLocal.Count);
                Assert.Equal(20, testDriversLocal.Count);
                Assert.Equal(12, teamsLocal.Count);

                Assert.All(context.ChangeTracker.Entries<Driver>().Select(e => e.Entity), e => Assert.True(driversLocal.Contains(e)));
                Assert.All(context.ChangeTracker.Entries<TestDriver>().Select(e => e.Entity), e => Assert.True(driversLocal.Contains(e)));
                Assert.All(context.ChangeTracker.Entries<TestDriver>().Select(e => e.Entity), e => Assert.True(testDriversLocal.Contains(e)));
                Assert.All(context.ChangeTracker.Entries<Team>().Select(e => e.Entity), e => Assert.True(teamsLocal.Contains(e)));

                Assert.All(context.ChangeTracker.Entries<Driver>().Select(e => e.Entity), e => Assert.False(teamsLocal.Contains((object)e)));
            }
        }

        [Fact]
        public virtual void DbSet_Local_ToBindingList_contains_Unchanged_Modified_and_Added_entities_but_not_Deleted_entities()
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);
                var bindingList = context.Drivers.Local.ToBindingList();

                Assert.Equal(0, bindingList.Count(d => d.TeamId == DeletedTeam));
                Assert.Equal(ModifiedCount, bindingList.Count(d => d.TeamId == ModifedTeam));
                Assert.Equal(UnchangedCount, bindingList.Count(d => d.TeamId == UnchangedTeam));
                Assert.Equal(AddedCount, bindingList.Count(d => d.TeamId == AddedTeam));
                Assert.Equal(TotalCount, bindingList.Count);
            }
        }

        [Fact]
        public virtual void Adding_entity_to_context_is_reflected_in_local_binding_list()
        {
            using (var context = CreateF1Context())
            {
                var bindingList = context.Drivers.Local.ToBindingList();

                Assert.Equal(0, bindingList.Count);

                var larry = new Driver
                {
                    Name = "Larry David",
                    TeamId = Team.Ferrari,
                    CarNumber = 13
                };
                context.Drivers.Add(larry);

                Assert.True(bindingList.Contains(larry));
                Assert.Equal(1, bindingList.Count);
            }
        }

        [Fact]
        public virtual void Entities_materialized_into_context_are_reflected_in_local_binding_list()
        {
            using (var context = CreateF1Context())
            {
                var bindingList = context.Drivers.Local.ToBindingList();

                Assert.Equal(0, bindingList.Count);

                context.Drivers.Where(d => d.TeamId == UnchangedTeam).Load();

                Assert.Equal(UnchangedCount, bindingList.Count);
            }
        }

        [Fact]
        public virtual void Entities_detached_from_context_are_removed_from_local_binding_list()
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);
                var bindingList = context.Drivers.Local.ToBindingList();

                Assert.Equal(TotalCount, bindingList.Count);

                foreach (var driver in context.Drivers.Local.Where(d => d.TeamId == UnchangedTeam).ToList())
                {
                    context.Entry(driver).State = EntityState.Detached;
                }

                Assert.Equal(0, bindingList.Count(d => d.TeamId == UnchangedTeam));
                Assert.Equal(TotalCount - UnchangedCount, bindingList.Count);
            }
        }

        [Fact]
        public virtual void Entities_deleted_from_context_are_removed_from_local_binding_list()
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);
                var bindingList = context.Drivers.Local.ToBindingList();

                Assert.Equal(TotalCount, bindingList.Count);

                foreach (var driver in context.Drivers.Local.Where(d => d.TeamId == UnchangedTeam).ToList())
                {
                    context.Drivers.Remove(driver);
                }

                Assert.Equal(0, bindingList.Count(d => d.TeamId == UnchangedTeam));
                Assert.Equal(TotalCount - UnchangedCount, bindingList.Count);
            }
        }

        [Fact]
        public virtual void Entities_added_to_local_binding_list_are_added_to_state_manager()
        {
            using (var context = CreateF1Context())
            {
                var local = context.Drivers.Local;
                var observable = local.ToObservableCollection();
                var bindingList = local.ToBindingList();

                Assert.Equal(0, bindingList.Count);

                var larry = new Driver
                {
                    Id = -1,
                    Name = "Larry David",
                    TeamId = Team.Ferrari,
                    CarNumber = 13
                };

                bindingList.Add(larry);

                Assert.Same(larry, context.Drivers.Find(-1));
                Assert.Equal(EntityState.Added, context.Entry(larry).State);
                Assert.Equal(1, bindingList.Count);
                Assert.Equal(1, local.Count);
                Assert.Equal(1, observable.Count);
                Assert.Contains(larry, bindingList);
                Assert.Contains(larry, local);
                Assert.Contains(larry, observable);
            }
        }

        [Fact]
        public virtual void Entities_removed_from_the_local_binding_list_are_marked_deleted_in_the_state_manager()
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var local = context.Drivers.Local;
                var observable = local.ToObservableCollection();
                var bindingList = local.ToBindingList();

                Assert.Equal(TotalCount, bindingList.Count);

                var alonso = bindingList.Single(d => d.Name == "Fernando Alonso");

                bindingList.Remove(alonso);

                Assert.Equal(EntityState.Deleted, context.Entry(alonso).State);
                Assert.Equal(TotalCount - 1, bindingList.Count);
                Assert.Equal(TotalCount - 1, local.Count);
                Assert.Equal(TotalCount - 1, observable.Count);
                Assert.DoesNotContain(alonso, bindingList);
                Assert.DoesNotContain(alonso, local);
                Assert.DoesNotContain(alonso, observable);
            }
        }

        [Fact]
        public virtual void Adding_entity_to_local_binding_list_that_is_Deleted_in_the_state_manager_makes_entity_Added()
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);

                var local = context.Drivers.Local;
                var observable = local.ToObservableCollection();
                var bindingList = local.ToBindingList();

                Assert.Equal(TotalCount, bindingList.Count);

                var deletedDrivers = context.Drivers.Where(d => d.TeamId == DeletedTeam).ToList();

                foreach (var driver in deletedDrivers)
                {
                    bindingList.Add(driver);
                }

                Assert.True(deletedDrivers.TrueForAll(d => context.Entry(d).State == EntityState.Added));
                Assert.Equal(TotalCount + DeletedCount, bindingList.Count);
                Assert.Equal(TotalCount + DeletedCount, local.Count);
                Assert.Equal(TotalCount + DeletedCount, observable.Count);

                foreach (var driver in deletedDrivers)
                {
                    Assert.Contains(driver, bindingList);
                    Assert.Contains(driver, local);
                    Assert.Contains(driver, observable);
                }
            }
        }

        [Fact]
        public virtual void Adding_entity_to_state_manager_of_different_type_than_local_view_type_has_no_effect_on_local_binding_list()
        {
            using (var context = CreateF1Context())
            {
                SetupContext(context);
                var bindingList = context.Drivers.Local.ToBindingList();
                var count = bindingList.Count;

                context.Teams.Add(
                    new Team
                    {
                        Id = -1,
                        Name = "Wubbsy Racing"
                    });

                Assert.Equal(count, bindingList.Count);
            }
        }

        [Fact]
        public virtual void Adding_entity_to_state_manager_of_subtype_still_shows_up_in_local_binding_list()
        {
            using (var context = CreateF1Context())
            {
                context.Drivers.Load();
                var bindingList = context.Drivers.Local.ToBindingList();

                var testDriver = new TestDriver();
                context.Drivers.Add(testDriver);

                Assert.True(bindingList.Contains(testDriver));
            }
        }

        [Fact]
        public virtual void Sets_of_subtypes_can_still_be_sorted()
        {
            using (var context = CreateF1Context())
            {
                var testDrivers = context.Set<TestDriver>();
                testDrivers.Attach(new TestDriver { Id = 3 });
                testDrivers.Attach(new TestDriver { Id = 1 });
                testDrivers.Attach(new TestDriver { Id = 4 });

                var bindingList = testDrivers.Local.ToBindingList();

                ((IBindingList)bindingList).ApplySort(
                    TypeDescriptor.GetProperties(typeof(Driver))["Id"],
                    ListSortDirection.Ascending);

                Assert.Equal(1, bindingList[0].Id);
                Assert.Equal(3, bindingList[1].Id);
                Assert.Equal(4, bindingList[2].Id);
            }
        }

        [Fact]
        public virtual void Sets_containing_instances_of_subtypes_can_still_be_sorted()
        {
            using (var context = CreateF1Context())
            {
                context.Drivers.Attach(new TestDriver { Id = 3 });
                context.Drivers.Attach(new TestDriver { Id = 1 });
                context.Drivers.Attach(new TestDriver { Id = 4 });

                var bindingList = context.Drivers.Local.ToBindingList();

                ((IBindingList)bindingList).ApplySort(
                    TypeDescriptor.GetProperties(typeof(Driver))["Id"],
                    ListSortDirection.Ascending);

                Assert.Equal(1, bindingList[0].Id);
                Assert.Equal(3, bindingList[1].Id);
                Assert.Equal(4, bindingList[2].Id);
            }
        }

        [Fact]
        public virtual void DbSet_Local_ToBindingList_is_cached_on_the_set()
        {
            using (var context = CreateF1Context())
            {
                var bindingList = context.Drivers.Local.ToBindingList();

                Assert.Same(bindingList, context.Drivers.Local.ToBindingList());
            }
        }

        [Fact]
        public virtual void Entity_added_to_context_is_added_to_navigation_property_binding_list()
        {
            using (var context = CreateF1Context())
            {
                var ferrari = context.Teams.Include(t => t.Drivers).Single(t => t.Id == Team.Ferrari);
                var navBindingList = ((IListSource)ferrari.Drivers).GetList();

                var larry = new Driver
                {
                    Name = "Larry David",
                    TeamId = Team.Ferrari,
                    CarNumber = 13
                };
                context.Drivers.Add(larry);

                Assert.True(navBindingList.Contains(larry));
            }
        }

        [Fact]
        public virtual void Entity_added_to_navigation_property_binding_list_is_added_to_context_after_DetectChanges()
        {
            using (var context = CreateF1Context())
            {
                var ferrari = context.Teams.Include(t => t.Drivers).Single(t => t.Id == Team.Ferrari);
                var navBindingList = ((IListSource)ferrari.Drivers).GetList();
                var localDrivers = context.Drivers.Local;

                var larry = new Driver
                {
                    Id = -1,
                    Name = "Larry David",
                    TeamId = Team.Ferrari,
                    CarNumber = 13
                };
                navBindingList.Add(larry);

                Assert.False(localDrivers.Contains(larry));

                context.ChangeTracker.DetectChanges();

                Assert.True(localDrivers.Contains(larry));
                Assert.Same(larry, context.Drivers.Find(-1));
            }
        }

        [Fact]
        public virtual void Entity_removed_from_navigation_property_binding_list_is_removed_from_nav_property_but_not_marked_Deleted()
        {
            using (var context = CreateF1Context())
            {
                var ferrari = context.Teams.Include(t => t.Drivers).Single(t => t.Id == Team.Ferrari);
                var navBindingList = ((IListSource)ferrari.Drivers).GetList();
                var localDrivers = context.Drivers.Local;

                var alonso = localDrivers.Single(d => d.Name == "Fernando Alonso");
                navBindingList.Remove(alonso);

                Assert.True(localDrivers.Contains(alonso));

                context.ChangeTracker.DetectChanges();

                Assert.True(localDrivers.Contains(alonso)); // Because it is not marked as Deleted

                Assert.False(ferrari.Drivers.Contains(alonso)); // But has been removed from nav prop
            }
        }

        protected F1Context CreateF1Context()
        {
            var context = Fixture.CreateContext(TestStore);
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            return context;
        }

        protected TFixture Fixture { get; }

        protected TTestStore TestStore { get; }

        public virtual void Dispose() => TestStore.Dispose();
    }
}
