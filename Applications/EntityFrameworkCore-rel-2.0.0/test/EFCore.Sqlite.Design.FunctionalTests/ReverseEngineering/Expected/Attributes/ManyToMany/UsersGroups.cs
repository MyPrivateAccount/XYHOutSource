using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Sqlite
{
    [Table("Users_Groups")]
    public partial class UsersGroups
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }

        [ForeignKey("GroupId")]
        [InverseProperty("UsersGroups")]
        public Groups Group { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("UsersGroups")]
        public Users User { get; set; }
    }
}
