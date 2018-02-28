// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public abstract class LoadTestBase<TTestStore, TFixture> : IClassFixture<TFixture>, IDisposable
        where TTestStore : TestStore
        where TFixture : LoadTestBase<TTestStore, TFixture>.LoadFixtureBase
    {
        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var collectionEntry = context.Entry(parent).Collection(e => e.Children);

                context.Entry(parent).State = state;

                Assert.False(collectionEntry.IsLoaded);

                if (async)
                {
                    await collectionEntry.LoadAsync();
                }
                else
                {
                    collectionEntry.Load();
                }

                Assert.True(collectionEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, parent.Children.Count());
                Assert.All(parent.Children.Select(e => e.Parent), c => Assert.Same(parent, c));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Single(e => e.Id == 12);

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, child.Parent);
                Assert.Same(child, parent.Children.Single());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Singles.Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.Single);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.Single);

                context.Entry(parent).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var single = context.ChangeTracker.Entries<Single>().Single().Entity;

                Assert.Same(single, parent.Single);
                Assert.Same(parent, single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_PK_to_PK_reference_to_principal(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.SinglePkToPks.Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.SinglePkToPk);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_PK_to_PK_reference_to_dependent(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.SinglePkToPk);

                context.Entry(parent).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var single = context.ChangeTracker.Entries<SinglePkToPk>().Single().Entity;

                Assert.Same(single, parent.SinglePkToPk);
                Assert.Same(parent, single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_using_Query(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var collectionEntry = context.Entry(parent).Collection(e => e.Children);

                context.Entry(parent).State = state;

                Assert.False(collectionEntry.IsLoaded);

                var children = async
                    ? await collectionEntry.Query().ToListAsync()
                    : collectionEntry.Query().ToList();

                Assert.False(collectionEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, children.Count);
                Assert.Equal(2, parent.Children.Count());
                Assert.All(children.Select(e => e.Parent), c => Assert.Same(parent, c));
                Assert.All(children, p => Assert.Contains(p, parent.Children));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_using_Query(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Single(e => e.Id == 12);

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, child.Parent);
                Assert.Same(child, parent.Children.Single());

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_using_Query(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Singles.Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.Single);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_using_Query(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.Single);

                context.Entry(parent).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var single = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(single);
                Assert.Same(single, parent.Single);
                Assert.Same(parent, single.Parent);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_PK_to_PK_reference_to_principal_using_Query(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.SinglePkToPks.Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.SinglePkToPk);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_PK_to_PK_reference_to_dependent_using_Query(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.SinglePkToPk);

                context.Entry(parent).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var single = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(single);
                Assert.Same(single, parent.SinglePkToPk);
                Assert.Same(parent, single.Parent);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_null_FK(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Attach(new Child { Id = 767, ParentId = null }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
                Assert.Null(child.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_null_FK(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Attach(new Single { Id = 767, ParentId = null }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());

                Assert.Null(single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_using_Query_null_FK(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Attach(new Child { Id = 767, ParentId = null }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleOrDefaultAsync()
                    : referenceEntry.Query().SingleOrDefault();

                Assert.False(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Null(parent);
                Assert.Null(child.Parent);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_using_Query_null_FK(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Attach(new Single { Id = 767, ParentId = null }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleOrDefaultAsync()
                    : referenceEntry.Query().SingleOrDefault();

                Assert.False(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Null(parent);
                Assert.Null(single.Parent);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_not_found(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Attach(new Parent { Id = 767, AlternateId = "NewRoot" }).Entity;

                ClearLog();

                var collectionEntry = context.Entry(parent).Collection(e => e.Children);

                context.Entry(parent).State = state;

                Assert.False(collectionEntry.IsLoaded);

                if (async)
                {
                    await collectionEntry.LoadAsync();
                }
                else
                {
                    collectionEntry.Load();
                }

                Assert.True(collectionEntry.IsLoaded);

                RecordLog();

                Assert.Equal(0, parent.Children.Count());
                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_not_found(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Attach(new Child { Id = 767, ParentId = 787 }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
                Assert.Null(child.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_not_found(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Attach(new Single { Id = 767, ParentId = 787 }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());

                Assert.Null(single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_not_found(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Attach(new Parent { Id = 767, AlternateId = "NewRoot" }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.Single);

                context.Entry(parent).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());

                Assert.Null(parent.Single);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_using_Query_not_found(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Attach(new Parent { Id = 767, AlternateId = "NewRoot" }).Entity;

                ClearLog();

                var collectionEntry = context.Entry(parent).Collection(e => e.Children);

                context.Entry(parent).State = state;

                Assert.False(collectionEntry.IsLoaded);

                var children = async
                    ? await collectionEntry.Query().ToListAsync()
                    : collectionEntry.Query().ToList();

                Assert.False(collectionEntry.IsLoaded);

                RecordLog();

                Assert.Equal(0, children.Count);
                Assert.Equal(0, parent.Children.Count());

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_using_Query_not_found(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Attach(new Child { Id = 767, ParentId = 787 }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleOrDefaultAsync()
                    : referenceEntry.Query().SingleOrDefault();

                Assert.False(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Null(parent);
                Assert.Null(child.Parent);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_using_Query_not_found(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Attach(new Single { Id = 767, ParentId = 787 }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleOrDefaultAsync()
                    : referenceEntry.Query().SingleOrDefault();

                Assert.False(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Null(parent);
                Assert.Null(single.Parent);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_using_Query_not_found(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Attach(new Parent { Id = 767, AlternateId = "NewRoot" }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.Single);

                context.Entry(parent).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var single = async
                    ? await referenceEntry.Query().SingleOrDefaultAsync()
                    : referenceEntry.Query().SingleOrDefault();

                Assert.False(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Null(single);
                Assert.Null(parent.Single);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_already_loaded(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Include(e => e.Children).Single();

                ClearLog();

                var collectionEntry = context.Entry(parent).Collection(e => e.Children);

                context.Entry(parent).State = state;

                Assert.True(collectionEntry.IsLoaded);

                if (async)
                {
                    await collectionEntry.LoadAsync();
                }
                else
                {
                    collectionEntry.Load();
                }

                Assert.True(collectionEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, parent.Children.Count());
                Assert.All(parent.Children.Select(e => e.Parent), c => Assert.Same(parent, c));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_already_loaded(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Include(e => e.Parent).Single(e => e.Id == 12);

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.True(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, child.Parent);
                Assert.Same(child, parent.Children.Single());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_already_loaded(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Singles.Include(e => e.Parent).Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.True(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.Single);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_already_loaded(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Include(e => e.Single).Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.Single);

                context.Entry(parent).State = state;

                Assert.True(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var single = context.ChangeTracker.Entries<Single>().Single().Entity;

                Assert.Same(single, parent.Single);
                Assert.Same(parent, single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_PK_to_PK_reference_to_principal_already_loaded(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.SinglePkToPks.Include(e => e.Parent).Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.True(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.SinglePkToPk);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_PK_to_PK_reference_to_dependent_already_loaded(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Include(e => e.SinglePkToPk).Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.SinglePkToPk);

                context.Entry(parent).State = state;

                Assert.True(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var single = context.ChangeTracker.Entries<SinglePkToPk>().Single().Entity;

                Assert.Same(single, parent.SinglePkToPk);
                Assert.Same(parent, single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_using_Query_already_loaded(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Include(e => e.Children).Single();

                ClearLog();

                var collectionEntry = context.Entry(parent).Collection(e => e.Children);

                context.Entry(parent).State = state;

                Assert.True(collectionEntry.IsLoaded);

                var children = async
                    ? await collectionEntry.Query().ToListAsync()
                    : collectionEntry.Query().ToList();

                Assert.True(collectionEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, children.Count);
                Assert.Equal(2, parent.Children.Count());
                Assert.All(children.Select(e => e.Parent), c => Assert.Same(parent, c));
                Assert.All(children, p => Assert.Contains(p, parent.Children));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_using_Query_already_loaded(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Include(e => e.Parent).Single(e => e.Id == 12);

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.True(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, child.Parent);
                Assert.Same(child, parent.Children.Single());

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_using_Query_already_loaded(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Singles.Include(e => e.Parent).Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.True(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.Single);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_using_Query_already_loaded(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Include(e => e.Single).Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.Single);

                context.Entry(parent).State = state;

                Assert.True(referenceEntry.IsLoaded);

                var single = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(single);
                Assert.Same(single, parent.Single);
                Assert.Same(parent, single.Parent);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_PK_to_PK_reference_to_principal_using_Query_already_loaded(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.SinglePkToPks.Include(e => e.Parent).Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.True(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.SinglePkToPk);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_PK_to_PK_reference_to_dependent_using_Query_already_loaded(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Include(e => e.SinglePkToPk).Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.SinglePkToPk);

                context.Entry(parent).State = state;

                Assert.True(referenceEntry.IsLoaded);

                var single = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(single);
                Assert.Same(single, parent.SinglePkToPk);
                Assert.Same(parent, single.Parent);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var navigationEntry = context.Entry(parent).Navigation("Children");

                context.Entry(parent).State = state;

                Assert.False(navigationEntry.IsLoaded);

                if (async)
                {
                    await navigationEntry.LoadAsync();
                }
                else
                {
                    navigationEntry.Load();
                }

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, parent.Children.Count());
                Assert.All(parent.Children.Select(e => e.Parent), c => Assert.Same(parent, c));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Single(e => e.Id == 12);

                ClearLog();

                var navigationEntry = context.Entry(child).Navigation("Parent");

                context.Entry(child).State = state;

                Assert.False(navigationEntry.IsLoaded);

                if (async)
                {
                    await navigationEntry.LoadAsync();
                }
                else
                {
                    navigationEntry.Load();
                }

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, child.Parent);
                Assert.Same(child, parent.Children.Single());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Singles.Single();

                ClearLog();

                var navigationEntry = context.Entry(single).Navigation("Parent");

                context.Entry(single).State = state;

                Assert.False(navigationEntry.IsLoaded);

                if (async)
                {
                    await navigationEntry.LoadAsync();
                }
                else
                {
                    navigationEntry.Load();
                }

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.Single);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var navigationEntry = context.Entry(parent).Navigation("Single");

                context.Entry(parent).State = state;

                Assert.False(navigationEntry.IsLoaded);

                if (async)
                {
                    await navigationEntry.LoadAsync();
                }
                else
                {
                    navigationEntry.Load();
                }

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var single = context.ChangeTracker.Entries<Single>().Single().Entity;

                Assert.Same(single, parent.Single);
                Assert.Same(parent, single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_using_Query_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var navigationEntry = context.Entry(parent).Navigation("Children");

                context.Entry(parent).State = state;

                Assert.False(navigationEntry.IsLoaded);

                var children = async
                    ? await navigationEntry.Query().OfType<object>().ToListAsync()
                    : navigationEntry.Query().OfType<object>().ToList();

                Assert.False(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, children.Count);
                Assert.Equal(2, parent.Children.Count());
                Assert.All(children.Select(e => ((Child)e).Parent), c => Assert.Same(parent, c));
                Assert.All(children, p => Assert.Contains(p, parent.Children));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_using_Query_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Single(e => e.Id == 12);

                ClearLog();

                var navigationEntry = context.Entry(child).Navigation("Parent");

                context.Entry(child).State = state;

                Assert.False(navigationEntry.IsLoaded);

                var parent = async
                    ? await navigationEntry.Query().OfType<object>().SingleAsync()
                    : navigationEntry.Query().OfType<object>().Single();

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, child.Parent);
                Assert.Same(child, ((Parent)parent).Children.Single());

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_using_Query_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Singles.Single();

                ClearLog();

                var navigationEntry = context.Entry(single).Navigation("Parent");

                context.Entry(single).State = state;

                Assert.False(navigationEntry.IsLoaded);

                var parent = async
                    ? await navigationEntry.Query().OfType<object>().SingleAsync()
                    : navigationEntry.Query().OfType<object>().Single();

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, single.Parent);
                Assert.Same(single, ((Parent)parent).Single);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_using_Query_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var navigationEntry = context.Entry(parent).Navigation("Single");

                context.Entry(parent).State = state;

                Assert.False(navigationEntry.IsLoaded);

                var single = async
                    ? await navigationEntry.Query().OfType<object>().SingleAsync()
                    : navigationEntry.Query().OfType<object>().Single();

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(single);
                Assert.Same(single, parent.Single);
                Assert.Same(parent, ((Single)single).Parent);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_not_found_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Attach(new Parent { Id = 767, AlternateId = "NewRoot" }).Entity;

                ClearLog();

                var navigationEntry = context.Entry(parent).Navigation("Children");

                context.Entry(parent).State = state;

                Assert.False(navigationEntry.IsLoaded);

                if (async)
                {
                    await navigationEntry.LoadAsync();
                }
                else
                {
                    navigationEntry.Load();
                }

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(0, parent.Children.Count());
                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_not_found_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Attach(new Child { Id = 767, ParentId = 787 }).Entity;

                ClearLog();

                var navigationEntry = context.Entry(child).Navigation("Parent");

                context.Entry(child).State = state;

                Assert.False(navigationEntry.IsLoaded);

                if (async)
                {
                    await navigationEntry.LoadAsync();
                }
                else
                {
                    navigationEntry.Load();
                }

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
                Assert.Null(child.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_not_found_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Attach(new Single { Id = 767, ParentId = 787 }).Entity;

                ClearLog();

                var navigationEntry = context.Entry(single).Navigation("Parent");

                context.Entry(single).State = state;

                Assert.False(navigationEntry.IsLoaded);

                if (async)
                {
                    await navigationEntry.LoadAsync();
                }
                else
                {
                    navigationEntry.Load();
                }

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());

                Assert.Null(single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_not_found_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Attach(new Parent { Id = 767, AlternateId = "NewRoot" }).Entity;

                ClearLog();

                var navigationEntry = context.Entry(parent).Navigation("Single");

                context.Entry(parent).State = state;

                Assert.False(navigationEntry.IsLoaded);

                if (async)
                {
                    await navigationEntry.LoadAsync();
                }
                else
                {
                    navigationEntry.Load();
                }

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());

                Assert.Null(parent.Single);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_using_Query_not_found_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Attach(new Parent { Id = 767, AlternateId = "NewRoot" }).Entity;

                ClearLog();

                var navigationEntry = context.Entry(parent).Navigation("Children");

                context.Entry(parent).State = state;

                Assert.False(navigationEntry.IsLoaded);

                var children = async
                    ? await navigationEntry.Query().OfType<object>().ToListAsync()
                    : navigationEntry.Query().OfType<object>().ToList();

                Assert.False(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(0, children.Count);
                Assert.Equal(0, parent.Children.Count());

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_using_Query_not_found_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Attach(new Child { Id = 767, ParentId = 787 }).Entity;

                ClearLog();

                var navigationEntry = context.Entry(child).Navigation("Parent");

                context.Entry(child).State = state;

                Assert.False(navigationEntry.IsLoaded);

                var parent = async
                    ? await navigationEntry.Query().OfType<object>().SingleOrDefaultAsync()
                    : navigationEntry.Query().OfType<object>().SingleOrDefault();

                Assert.False(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Null(parent);
                Assert.Null(child.Parent);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_using_Query_not_found_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Attach(new Single { Id = 767, ParentId = 787 }).Entity;

                ClearLog();

                var navigationEntry = context.Entry(single).Navigation("Parent");

                context.Entry(single).State = state;

                Assert.False(navigationEntry.IsLoaded);

                var parent = async
                    ? await navigationEntry.Query().OfType<object>().SingleOrDefaultAsync()
                    : navigationEntry.Query().OfType<object>().SingleOrDefault();

                Assert.False(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Null(parent);
                Assert.Null(single.Parent);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_using_Query_not_found_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Attach(new Parent { Id = 767, AlternateId = "NewRoot" }).Entity;

                ClearLog();

                var navigationEntry = context.Entry(parent).Navigation("Single");

                context.Entry(parent).State = state;

                Assert.False(navigationEntry.IsLoaded);

                var single = async
                    ? await navigationEntry.Query().OfType<object>().SingleOrDefaultAsync()
                    : navigationEntry.Query().OfType<object>().SingleOrDefault();

                Assert.False(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Null(single);
                Assert.Null(parent.Single);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_already_loaded_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Include(e => e.Children).Single();

                ClearLog();

                var navigationEntry = context.Entry(parent).Navigation("Children");

                context.Entry(parent).State = state;

                Assert.True(navigationEntry.IsLoaded);

                if (async)
                {
                    await navigationEntry.LoadAsync();
                }
                else
                {
                    navigationEntry.Load();
                }

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, parent.Children.Count());
                Assert.All(parent.Children.Select(e => e.Parent), c => Assert.Same(parent, c));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_already_loaded_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Include(e => e.Parent).Single(e => e.Id == 12);

                ClearLog();

                var navigationEntry = context.Entry(child).Navigation("Parent");

                context.Entry(child).State = state;

                Assert.True(navigationEntry.IsLoaded);

                if (async)
                {
                    await navigationEntry.LoadAsync();
                }
                else
                {
                    navigationEntry.Load();
                }

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, child.Parent);
                Assert.Same(child, parent.Children.Single());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_already_loaded_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Singles.Include(e => e.Parent).Single();

                ClearLog();

                var navigationEntry = context.Entry(single).Navigation("Parent");

                context.Entry(single).State = state;

                Assert.True(navigationEntry.IsLoaded);

                if (async)
                {
                    await navigationEntry.LoadAsync();
                }
                else
                {
                    navigationEntry.Load();
                }

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.Single);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_already_loaded_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Include(e => e.Single).Single();

                ClearLog();

                var navigationEntry = context.Entry(parent).Navigation("Single");

                context.Entry(parent).State = state;

                Assert.True(navigationEntry.IsLoaded);

                if (async)
                {
                    await navigationEntry.LoadAsync();
                }
                else
                {
                    navigationEntry.Load();
                }

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var single = context.ChangeTracker.Entries<Single>().Single().Entity;

                Assert.Same(single, parent.Single);
                Assert.Same(parent, single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_using_Query_already_loaded_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Include(e => e.Children).Single();

                ClearLog();

                var navigationEntry = context.Entry(parent).Navigation("Children");

                context.Entry(parent).State = state;

                Assert.True(navigationEntry.IsLoaded);

                var children = async
                    ? await navigationEntry.Query().OfType<object>().ToListAsync()
                    : navigationEntry.Query().OfType<object>().ToList();

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, children.Count);
                Assert.Equal(2, parent.Children.Count());
                Assert.All(children.Select(e => ((Child)e).Parent), c => Assert.Same(parent, c));
                Assert.All(children, p => Assert.Contains(p, parent.Children));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_using_Query_already_loaded_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Include(e => e.Parent).Single(e => e.Id == 12);

                ClearLog();

                var navigationEntry = context.Entry(child).Navigation("Parent");

                context.Entry(child).State = state;

                Assert.True(navigationEntry.IsLoaded);

                var parent = async
                    ? await navigationEntry.Query().OfType<object>().SingleAsync()
                    : navigationEntry.Query().OfType<object>().Single();

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, child.Parent);
                Assert.Same(child, ((Parent)parent).Children.Single());

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_using_Query_already_loaded_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Singles.Include(e => e.Parent).Single();

                ClearLog();

                var navigationEntry = context.Entry(single).Navigation("Parent");

                context.Entry(single).State = state;

                Assert.True(navigationEntry.IsLoaded);

                var parent = async
                    ? await navigationEntry.Query().OfType<object>().SingleAsync()
                    : navigationEntry.Query().OfType<object>().Single();

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, single.Parent);
                Assert.Same(single, ((Parent)parent).Single);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_using_Query_already_loaded_untyped(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Include(e => e.Single).Single();

                ClearLog();

                var navigationEntry = context.Entry(parent).Navigation("Single");

                context.Entry(parent).State = state;

                Assert.True(navigationEntry.IsLoaded);

                var single = async
                    ? await navigationEntry.Query().OfType<object>().SingleAsync()
                    : navigationEntry.Query().OfType<object>().Single();

                Assert.True(navigationEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(single);
                Assert.Same(single, parent.Single);
                Assert.Same(parent, ((Single)single).Parent);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_alternate_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var collectionEntry = context.Entry(parent).Collection(e => e.ChildrenAk);

                context.Entry(parent).State = state;

                Assert.False(collectionEntry.IsLoaded);

                if (async)
                {
                    await collectionEntry.LoadAsync();
                }
                else
                {
                    collectionEntry.Load();
                }

                Assert.True(collectionEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, parent.ChildrenAk.Count());
                Assert.All(parent.ChildrenAk.Select(e => e.Parent), c => Assert.Same(parent, c));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_alternate_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.ChildrenAks.Single(e => e.Id == 32);

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, child.Parent);
                Assert.Same(child, parent.ChildrenAk.Single());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_alternate_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.SingleAks.Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.SingleAk);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_alternate_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.SingleAk);

                context.Entry(parent).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var single = context.ChangeTracker.Entries<SingleAk>().Single().Entity;

                Assert.Same(single, parent.SingleAk);
                Assert.Same(parent, single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_using_Query_alternate_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var collectionEntry = context.Entry(parent).Collection(e => e.ChildrenAk);

                context.Entry(parent).State = state;

                Assert.False(collectionEntry.IsLoaded);

                var children = async
                    ? await collectionEntry.Query().ToListAsync()
                    : collectionEntry.Query().ToList();

                Assert.False(collectionEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, children.Count);
                Assert.Equal(2, parent.ChildrenAk.Count());
                Assert.All(children.Select(e => e.Parent), c => Assert.Same(parent, c));
                Assert.All(children, p => Assert.Contains(p, parent.ChildrenAk));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_using_Query_alternate_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.ChildrenAks.Single(e => e.Id == 32);

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, child.Parent);
                Assert.Same(child, parent.ChildrenAk.Single());

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_using_Query_alternate_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.SingleAks.Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.SingleAk);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_using_Query_alternate_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.SingleAk);

                context.Entry(parent).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var single = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(single);
                Assert.Same(single, parent.SingleAk);
                Assert.Same(parent, single.Parent);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_null_FK_alternate_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Attach(new ChildAk { Id = 767, ParentId = null }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
                Assert.Null(child.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_null_FK_alternate_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Attach(new SingleAk { Id = 767, ParentId = null }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());

                Assert.Null(single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_using_Query_null_FK_alternate_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Attach(new ChildAk { Id = 767, ParentId = null }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleOrDefaultAsync()
                    : referenceEntry.Query().SingleOrDefault();

                Assert.False(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Null(parent);
                Assert.Null(child.Parent);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_using_Query_null_FK_alternate_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Attach(new SingleAk { Id = 767, ParentId = null }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleOrDefaultAsync()
                    : referenceEntry.Query().SingleOrDefault();

                Assert.False(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Null(parent);
                Assert.Null(single.Parent);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_shadow_fk(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var collectionEntry = context.Entry(parent).Collection(e => e.ChildrenShadowFk);

                context.Entry(parent).State = state;

                Assert.False(collectionEntry.IsLoaded);

                if (async)
                {
                    await collectionEntry.LoadAsync();
                }
                else
                {
                    collectionEntry.Load();
                }

                Assert.True(collectionEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, parent.ChildrenShadowFk.Count());
                Assert.All(parent.ChildrenShadowFk.Select(e => e.Parent), c => Assert.Same(parent, c));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_shadow_fk(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.ChildrenShadowFks.Single(e => e.Id == 52);

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, child.Parent);
                Assert.Same(child, parent.ChildrenShadowFk.Single());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_shadow_fk(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.SingleShadowFks.Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.SingleShadowFk);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_shadow_fk(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.SingleShadowFk);

                context.Entry(parent).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var single = context.ChangeTracker.Entries<SingleShadowFk>().Single().Entity;

                Assert.Same(single, parent.SingleShadowFk);
                Assert.Same(parent, single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_using_Query_shadow_fk(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var collectionEntry = context.Entry(parent).Collection(e => e.ChildrenShadowFk);

                context.Entry(parent).State = state;

                Assert.False(collectionEntry.IsLoaded);

                var children = async
                    ? await collectionEntry.Query().ToListAsync()
                    : collectionEntry.Query().ToList();

                Assert.False(collectionEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, children.Count);
                Assert.Equal(2, parent.ChildrenShadowFk.Count());
                Assert.All(children.Select(e => e.Parent), c => Assert.Same(parent, c));
                Assert.All(children, p => Assert.Contains(p, parent.ChildrenShadowFk));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_using_Query_shadow_fk(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.ChildrenShadowFks.Single(e => e.Id == 52);

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, child.Parent);
                Assert.Same(child, parent.ChildrenShadowFk.Single());

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_using_Query_shadow_fk(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.SingleShadowFks.Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.SingleShadowFk);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_using_Query_shadow_fk(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.SingleShadowFk);

                context.Entry(parent).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var single = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(single);
                Assert.Same(single, parent.SingleShadowFk);
                Assert.Same(parent, single.Parent);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_null_FK_shadow_fk(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Attach(new ChildShadowFk { Id = 767 }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
                Assert.Null(child.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_null_FK_shadow_fk(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Attach(new SingleShadowFk { Id = 767 }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());

                Assert.Null(single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_using_Query_null_FK_shadow_fk(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Attach(new ChildShadowFk { Id = 767 }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleOrDefaultAsync()
                    : referenceEntry.Query().SingleOrDefault();

                Assert.False(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Null(parent);
                Assert.Null(child.Parent);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_using_Query_null_FK_shadow_fk(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Attach(new SingleShadowFk { Id = 767 }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleOrDefaultAsync()
                    : referenceEntry.Query().SingleOrDefault();

                Assert.False(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Null(parent);
                Assert.Null(single.Parent);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_composite_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var collectionEntry = context.Entry(parent).Collection(e => e.ChildrenCompositeKey);

                context.Entry(parent).State = state;

                Assert.False(collectionEntry.IsLoaded);

                if (async)
                {
                    await collectionEntry.LoadAsync();
                }
                else
                {
                    collectionEntry.Load();
                }

                Assert.True(collectionEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, parent.ChildrenCompositeKey.Count());
                Assert.All(parent.ChildrenCompositeKey.Select(e => e.Parent), c => Assert.Same(parent, c));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_composite_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.ChildrenCompositeKeys.Single(e => e.Id == 52);

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, child.Parent);
                Assert.Same(child, parent.ChildrenCompositeKey.Single());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_composite_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.SingleCompositeKeys.Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var parent = context.ChangeTracker.Entries<Parent>().Single().Entity;

                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.SingleCompositeKey);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_composite_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.SingleCompositeKey);

                context.Entry(parent).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                var single = context.ChangeTracker.Entries<SingleCompositeKey>().Single().Entity;

                Assert.Same(single, parent.SingleCompositeKey);
                Assert.Same(parent, single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_collection_using_Query_composite_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var collectionEntry = context.Entry(parent).Collection(e => e.ChildrenCompositeKey);

                context.Entry(parent).State = state;

                Assert.False(collectionEntry.IsLoaded);

                var children = async
                    ? await collectionEntry.Query().ToListAsync()
                    : collectionEntry.Query().ToList();

                Assert.False(collectionEntry.IsLoaded);

                RecordLog();

                Assert.Equal(2, children.Count);
                Assert.Equal(2, parent.ChildrenCompositeKey.Count());
                Assert.All(children.Select(e => e.Parent), c => Assert.Same(parent, c));
                Assert.All(children, p => Assert.Contains(p, parent.ChildrenCompositeKey));

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_using_Query_composite_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.ChildrenCompositeKeys.Single(e => e.Id == 52);

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, child.Parent);
                Assert.Same(child, parent.ChildrenCompositeKey.Single());

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_using_Query_composite_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.SingleCompositeKeys.Single();

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(parent);
                Assert.Same(parent, single.Parent);
                Assert.Same(single, parent.SingleCompositeKey);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_dependent_using_Query_composite_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                ClearLog();

                var referenceEntry = context.Entry(parent).Reference(e => e.SingleCompositeKey);

                context.Entry(parent).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var single = async
                    ? await referenceEntry.Query().SingleAsync()
                    : referenceEntry.Query().Single();

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.NotNull(single);
                Assert.Same(single, parent.SingleCompositeKey);
                Assert.Same(parent, single.Parent);

                Assert.Equal(2, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_null_FK_composite_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Attach(new ChildCompositeKey { Id = 767, ParentId = 567 }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
                Assert.Null(child.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_null_FK_composite_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Attach(new SingleCompositeKey { Id = 767, ParentAlternateId = "Boot" }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                if (async)
                {
                    await referenceEntry.LoadAsync();
                }
                else
                {
                    referenceEntry.Load();
                }

                Assert.True(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());

                Assert.Null(single.Parent);
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_many_to_one_reference_to_principal_using_Query_null_FK_composite_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Attach(new ChildCompositeKey { Id = 767, ParentAlternateId = "Boot" }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleOrDefaultAsync()
                    : referenceEntry.Query().SingleOrDefault();

                Assert.False(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Null(parent);
                Assert.Null(child.Parent);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Theory]
        [InlineData(EntityState.Unchanged, true)]
        [InlineData(EntityState.Unchanged, false)]
        [InlineData(EntityState.Modified, true)]
        [InlineData(EntityState.Modified, false)]
        [InlineData(EntityState.Deleted, true)]
        [InlineData(EntityState.Deleted, false)]
        public virtual async Task Load_one_to_one_reference_to_principal_using_Query_null_FK_composite_key(EntityState state, bool async)
        {
            using (var context = CreateContext())
            {
                var single = context.Attach(new SingleCompositeKey { Id = 767, ParentId = 567 }).Entity;

                ClearLog();

                var referenceEntry = context.Entry(single).Reference(e => e.Parent);

                context.Entry(single).State = state;

                Assert.False(referenceEntry.IsLoaded);

                var parent = async
                    ? await referenceEntry.Query().SingleOrDefaultAsync()
                    : referenceEntry.Query().SingleOrDefault();

                Assert.False(referenceEntry.IsLoaded);

                RecordLog();

                Assert.Null(parent);
                Assert.Null(single.Parent);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            }
        }

        [Fact]
        public virtual void Can_change_IsLoaded_flag_for_collection()
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                var collectionEntry = context.Entry(parent).Collection(e => e.Children);

                Assert.False(collectionEntry.IsLoaded);

                collectionEntry.IsLoaded = true;

                Assert.True(collectionEntry.IsLoaded);

                collectionEntry.Load();

                Assert.Equal(0, parent.Children.Count());
                Assert.Equal(1, context.ChangeTracker.Entries().Count());

                Assert.True(collectionEntry.IsLoaded);

                collectionEntry.IsLoaded = false;

                Assert.False(collectionEntry.IsLoaded);

                collectionEntry.Load();

                Assert.Equal(2, parent.Children.Count());
                Assert.All(parent.Children.Select(e => e.Parent), c => Assert.Same(parent, c));
                Assert.Equal(3, context.ChangeTracker.Entries().Count());

                Assert.True(collectionEntry.IsLoaded);
            }
        }

        [Fact]
        public virtual void Can_change_IsLoaded_flag_for_reference_only_if_null()
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Single(e => e.Id == 12);

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                Assert.False(referenceEntry.IsLoaded);

                referenceEntry.IsLoaded = true;

                Assert.True(referenceEntry.IsLoaded);

                referenceEntry.Load();

                Assert.True(referenceEntry.IsLoaded);

                Assert.Equal(1, context.ChangeTracker.Entries().Count());

                referenceEntry.IsLoaded = true;

                referenceEntry.IsLoaded = false;

                referenceEntry.Load();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                Assert.True(referenceEntry.IsLoaded);

                Assert.Equal(
                    CoreStrings.ReferenceMustBeLoaded("Parent", typeof(Child).Name),
                    Assert.Throws<InvalidOperationException>(() => referenceEntry.IsLoaded = false).Message);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public virtual async Task Load_collection_for_detached_throws(bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                var collectionEntry = context.Entry(parent).Collection(e => e.Children);

                context.Entry(parent).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Parent.Children), nameof(Parent)),
                    (await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                        {
                            if (async)
                            {
                                await collectionEntry.LoadAsync();
                            }
                            else
                            {
                                collectionEntry.Load();
                            }
                        })).Message);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public virtual async Task Load_collection_using_string_for_detached_throws(bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                var collectionEntry = context.Entry(parent).Collection(nameof(Parent.Children));

                context.Entry(parent).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Parent.Children), nameof(Parent)),
                    (await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                        {
                            if (async)
                            {
                                await collectionEntry.LoadAsync();
                            }
                            else
                            {
                                collectionEntry.Load();
                            }
                        })).Message);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public virtual async Task Load_collection_with_navigation_for_detached_throws(bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                var collectionEntry = context.Entry(parent).Navigation(nameof(Parent.Children));

                context.Entry(parent).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Parent.Children), nameof(Parent)),
                    (await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                        {
                            if (async)
                            {
                                await collectionEntry.LoadAsync();
                            }
                            else
                            {
                                collectionEntry.Load();
                            }
                        })).Message);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public virtual async Task Load_reference_to_principal_for_detached_throws(bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Single(e => e.Id == 12);

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Child.Parent), nameof(Child)),
                    (await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                        {
                            if (async)
                            {
                                await referenceEntry.LoadAsync();
                            }
                            else
                            {
                                referenceEntry.Load();
                            }
                        })).Message);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public virtual async Task Load_reference_with_navigation_to_principal_for_detached_throws(bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Single(e => e.Id == 12);

                var referenceEntry = context.Entry(child).Navigation(nameof(Child.Parent));

                context.Entry(child).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Child.Parent), nameof(Child)),
                    (await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                        {
                            if (async)
                            {
                                await referenceEntry.LoadAsync();
                            }
                            else
                            {
                                referenceEntry.Load();
                            }
                        })).Message);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public virtual async Task Load_reference_using_string_to_principal_for_detached_throws(bool async)
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Single(e => e.Id == 12);

                var referenceEntry = context.Entry(child).Reference(nameof(Child.Parent));

                context.Entry(child).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Child.Parent), nameof(Child)),
                    (await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                        {
                            if (async)
                            {
                                await referenceEntry.LoadAsync();
                            }
                            else
                            {
                                referenceEntry.Load();
                            }
                        })).Message);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public virtual async Task Load_reference_to_dependent_for_detached_throws(bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                var referenceEntry = context.Entry(parent).Reference(e => e.Single);

                context.Entry(parent).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Parent.Single), nameof(Parent)),
                    (await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                        {
                            if (async)
                            {
                                await referenceEntry.LoadAsync();
                            }
                            else
                            {
                                referenceEntry.Load();
                            }
                        })).Message);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public virtual async Task Load_reference_to_dependent_with_navigation_for_detached_throws(bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                var referenceEntry = context.Entry(parent).Navigation(nameof(Parent.Single));

                context.Entry(parent).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Parent.Single), nameof(Parent)),
                    (await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                        {
                            if (async)
                            {
                                await referenceEntry.LoadAsync();
                            }
                            else
                            {
                                referenceEntry.Load();
                            }
                        })).Message);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public virtual async Task Load_reference_to_dependent_using_string_for_detached_throws(bool async)
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                var referenceEntry = context.Entry(parent).Reference(nameof(Parent.Single));

                context.Entry(parent).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Parent.Single), nameof(Parent)),
                    (await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                        {
                            if (async)
                            {
                                await referenceEntry.LoadAsync();
                            }
                            else
                            {
                                referenceEntry.Load();
                            }
                        })).Message);
            }
        }

        [Fact]
        public virtual void Query_collection_for_detached_throws()
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                var collectionEntry = context.Entry(parent).Collection(e => e.Children);

                context.Entry(parent).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Parent.Children), nameof(Parent)),
                    Assert.Throws<InvalidOperationException>(() => collectionEntry.Query()).Message);
            }
        }

        [Fact]
        public virtual void Query_collection_using_string_for_detached_throws()
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                var collectionEntry = context.Entry(parent).Collection(nameof(Parent.Children));

                context.Entry(parent).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Parent.Children), nameof(Parent)),
                    Assert.Throws<InvalidOperationException>(() => collectionEntry.Query()).Message);
            }
        }

        [Fact]
        public virtual void Query_collection_with_navigation_for_detached_throws()
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                var collectionEntry = context.Entry(parent).Navigation(nameof(Parent.Children));

                context.Entry(parent).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Parent.Children), nameof(Parent)),
                    Assert.Throws<InvalidOperationException>(() => collectionEntry.Query()).Message);
            }
        }

        [Fact]
        public virtual void Query_reference_to_principal_for_detached_throws()
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Single(e => e.Id == 12);

                var referenceEntry = context.Entry(child).Reference(e => e.Parent);

                context.Entry(child).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Child.Parent), nameof(Child)),
                    Assert.Throws<InvalidOperationException>(() => referenceEntry.Query()).Message);
            }
        }

        [Fact]
        public virtual void Query_reference_with_navigation_to_principal_for_detached_throws()
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Single(e => e.Id == 12);

                var referenceEntry = context.Entry(child).Navigation(nameof(Child.Parent));

                context.Entry(child).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Child.Parent), nameof(Child)),
                    Assert.Throws<InvalidOperationException>(() => referenceEntry.Query()).Message);
            }
        }

        [Fact]
        public virtual void Query_reference_using_string_to_principal_for_detached_throws()
        {
            using (var context = CreateContext())
            {
                var child = context.Children.Single(e => e.Id == 12);

                var referenceEntry = context.Entry(child).Reference(nameof(Child.Parent));

                context.Entry(child).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Child.Parent), nameof(Child)),
                    Assert.Throws<InvalidOperationException>(() => referenceEntry.Query()).Message);
            }
        }

        [Fact]
        public virtual void Query_reference_to_dependent_for_detached_throws()
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                var referenceEntry = context.Entry(parent).Reference(e => e.Single);

                context.Entry(parent).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Parent.Single), nameof(Parent)),
                    Assert.Throws<InvalidOperationException>(() => referenceEntry.Query()).Message);
            }
        }

        [Fact]
        public virtual void Query_reference_to_dependent_with_navigation_for_detached_throws()
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                var referenceEntry = context.Entry(parent).Navigation(nameof(Parent.Single));

                context.Entry(parent).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Parent.Single), nameof(Parent)),
                    Assert.Throws<InvalidOperationException>(() => referenceEntry.Query()).Message);
            }
        }

        [Fact]
        public virtual void Query_reference_to_dependent_using_string_for_detached_throws()
        {
            using (var context = CreateContext())
            {
                var parent = context.Parents.Single();

                var referenceEntry = context.Entry(parent).Reference(nameof(Parent.Single));

                context.Entry(parent).State = EntityState.Detached;

                Assert.Equal(
                    CoreStrings.CannotLoadDetached(nameof(Parent.Single), nameof(Parent)),
                    Assert.Throws<InvalidOperationException>(() => referenceEntry.Query()).Message);
            }
        }

        protected class Parent
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }

            public string AlternateId { get; set; }

            public IEnumerable<Child> Children { get; set; }
            public SinglePkToPk SinglePkToPk { get; set; }
            public Single Single { get; set; }

            public IEnumerable<ChildAk> ChildrenAk { get; set; }
            public SingleAk SingleAk { get; set; }

            public IEnumerable<ChildShadowFk> ChildrenShadowFk { get; set; }
            public SingleShadowFk SingleShadowFk { get; set; }

            public IEnumerable<ChildCompositeKey> ChildrenCompositeKey { get; set; }
            public SingleCompositeKey SingleCompositeKey { get; set; }
        }

        protected class Child
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }

            public int? ParentId { get; set; }
            public Parent Parent { get; set; }
        }

        protected class SinglePkToPk
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }

            public Parent Parent { get; set; }
        }

        protected class Single
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }

            public int? ParentId { get; set; }
            public Parent Parent { get; set; }
        }

        protected class ChildAk
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }

            public string ParentId { get; set; }
            public Parent Parent { get; set; }
        }

        protected class SingleAk
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }

            public string ParentId { get; set; }
            public Parent Parent { get; set; }
        }

        protected class ChildShadowFk
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }

            public Parent Parent { get; set; }
        }

        protected class SingleShadowFk
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }

            public Parent Parent { get; set; }
        }

        protected class ChildCompositeKey
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }

            public int? ParentId { get; set; }
            public string ParentAlternateId { get; set; }
            public Parent Parent { get; set; }
        }

        protected class SingleCompositeKey
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }

            public int? ParentId { get; set; }
            public string ParentAlternateId { get; set; }
            public Parent Parent { get; set; }
        }

        protected class LoadContext : DbContext
        {
            public LoadContext(DbContextOptions options)
                : base(options)
            {
            }

            public DbSet<Parent> Parents { get; set; }
            public DbSet<SinglePkToPk> SinglePkToPks { get; set; }
            public DbSet<Single> Singles { get; set; }
            public DbSet<Child> Children { get; set; }
            public DbSet<SingleAk> SingleAks { get; set; }
            public DbSet<ChildAk> ChildrenAks { get; set; }
            public DbSet<SingleShadowFk> SingleShadowFks { get; set; }
            public DbSet<ChildShadowFk> ChildrenShadowFks { get; set; }
            public DbSet<SingleCompositeKey> SingleCompositeKeys { get; set; }
            public DbSet<ChildCompositeKey> ChildrenCompositeKeys { get; set; }
        }

        protected LoadTestBase(TFixture fixture)
        {
            Fixture = fixture;

            TestStore = Fixture.CreateTestStore();
        }

        protected LoadContext CreateContext() => (LoadContext)Fixture.CreateContext(TestStore);

        protected TFixture Fixture { get; }
        protected TTestStore TestStore { get; }

        public virtual void Dispose() => TestStore.Dispose();

        protected virtual void ClearLog()
        {
        }

        protected virtual void RecordLog()
        {
        }

        public abstract class LoadFixtureBase
        {
            public abstract TTestStore CreateTestStore();

            public abstract DbContext CreateContext(TTestStore testStore);

            protected virtual void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<SingleShadowFk>()
                    .Property<int?>("ParentId");

                modelBuilder.Entity<Parent>(b =>
                    {
                        b.Property(e => e.AlternateId).ValueGeneratedOnAdd();

                        b.HasMany(e => e.Children)
                            .WithOne(e => e.Parent)
                            .HasForeignKey(e => e.ParentId);

                        b.HasOne(e => e.SinglePkToPk)
                            .WithOne(e => e.Parent)
                            .HasForeignKey<SinglePkToPk>(e => e.Id)
                            .IsRequired();

                        b.HasOne(e => e.Single)
                            .WithOne(e => e.Parent)
                            .HasForeignKey<Single>(e => e.ParentId);

                        b.HasMany(e => e.ChildrenAk)
                            .WithOne(e => e.Parent)
                            .HasPrincipalKey(e => e.AlternateId)
                            .HasForeignKey(e => e.ParentId);

                        b.HasOne(e => e.SingleAk)
                            .WithOne(e => e.Parent)
                            .HasPrincipalKey<Parent>(e => e.AlternateId)
                            .HasForeignKey<SingleAk>(e => e.ParentId);

                        b.HasMany(e => e.ChildrenShadowFk)
                            .WithOne(e => e.Parent)
                            .HasPrincipalKey(e => e.Id)
                            .HasForeignKey("ParentId");

                        b.HasOne(e => e.SingleShadowFk)
                            .WithOne(e => e.Parent)
                            .HasPrincipalKey<Parent>(e => e.Id)
                            .HasForeignKey<SingleShadowFk>("ParentId");

                        b.HasMany(e => e.ChildrenCompositeKey)
                            .WithOne(e => e.Parent)
                            .HasPrincipalKey(e => new { e.AlternateId, e.Id })
                            .HasForeignKey(e => new { e.ParentAlternateId, e.ParentId });

                        b.HasOne(e => e.SingleCompositeKey)
                            .WithOne(e => e.Parent)
                            .HasPrincipalKey<Parent>(e => new { e.AlternateId, e.Id })
                            .HasForeignKey<SingleCompositeKey>(e => new { e.ParentAlternateId, e.ParentId });
                    });
            }

            protected virtual object CreateFullGraph()
                => new Parent
                {
                    Id = 707,
                    AlternateId = "Root",
                    Children = new List<Child>
                    {
                        new Child { Id = 11 },
                        new Child { Id = 12 }
                    },
                    SinglePkToPk = new SinglePkToPk { Id = 707 },
                    Single = new Single { Id = 21 },
                    ChildrenAk = new List<ChildAk>
                    {
                        new ChildAk { Id = 31 },
                        new ChildAk { Id = 32 }
                    },
                    SingleAk = new SingleAk { Id = 42 },
                    ChildrenShadowFk = new List<ChildShadowFk>
                    {
                        new ChildShadowFk { Id = 51 },
                        new ChildShadowFk { Id = 52 }
                    },
                    SingleShadowFk = new SingleShadowFk { Id = 62 },
                    ChildrenCompositeKey = new List<ChildCompositeKey>
                    {
                        new ChildCompositeKey { Id = 51 },
                        new ChildCompositeKey { Id = 52 }
                    },
                    SingleCompositeKey = new SingleCompositeKey { Id = 62 }
                };

            protected virtual void Seed(DbContext context)
            {
                context.Add(CreateFullGraph());
                context.SaveChanges();
            }
        }
    }
}
