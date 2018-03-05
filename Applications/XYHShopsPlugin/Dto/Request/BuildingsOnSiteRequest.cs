using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class BuildingsOnSiteRequest
    {
        [Required]
        [StringLength(127)]
        public string Id { get; set; }
        public string Name { get; set; }
        [StringLength(127)]
        public string ResidentUser1 { get; set; }
        [StringLength(127)]
        public string ResidentUser2 { get; set; }
        [StringLength(127)]
        public string ResidentUser3 { get; set; }
        [StringLength(127)]
        public string ResidentUser4 { get; set; }
        [StringLength(127)]
        public string ResidentUserName1 { get; set; }
        [StringLength(127)]
        public string ResidentUserName2 { get; set; }
        [StringLength(127)]
        public string ResidentUserName3 { get; set; }
        [StringLength(127)]
        public string ResidentUserName4 { get; set; }



    }
}
