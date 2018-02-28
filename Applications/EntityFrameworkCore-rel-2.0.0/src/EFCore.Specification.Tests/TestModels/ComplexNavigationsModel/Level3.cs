// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Microsoft.EntityFrameworkCore.TestModels.ComplexNavigationsModel
{
    public class Level3
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Level2_Required_Id { get; set; }
        public int? Level2_Optional_Id { get; set; }

        public Level4 OneToOne_Required_PK { get; set; }
        public Level4 OneToOne_Optional_PK { get; set; }

        public Level4 OneToOne_Required_FK { get; set; }
        public Level4 OneToOne_Optional_FK { get; set; }

        public ICollection<Level4> OneToMany_Required { get; set; }
        public ICollection<Level4> OneToMany_Optional { get; set; }

        public Level2 OneToOne_Required_PK_Inverse { get; set; }
        public Level2 OneToOne_Optional_PK_Inverse { get; set; }
        public Level2 OneToOne_Required_FK_Inverse { get; set; }
        public Level2 OneToOne_Optional_FK_Inverse { get; set; }

        public Level2 OneToMany_Required_Inverse { get; set; }
        public Level2 OneToMany_Optional_Inverse { get; set; }

        public Level3 OneToOne_Optional_Self { get; set; }

        public ICollection<Level3> OneToMany_Required_Self { get; set; }
        public ICollection<Level3> OneToMany_Optional_Self { get; set; }
        public Level3 OneToMany_Required_Self_Inverse { get; set; }
        public Level3 OneToMany_Optional_Self_Inverse { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((Level3)obj);
        }

        protected bool Equals(Level3 other)
        {
            return Id == other.Id && string.Equals(Name, other.Name) && Level2_Required_Id == other.Level2_Required_Id && Level2_Optional_Id == other.Level2_Optional_Id;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Level2_Required_Id;
                hashCode = (hashCode * 397) ^ (Level2_Optional_Id?.GetHashCode() ?? 0);

                return hashCode;
            }
        }
    }
}
