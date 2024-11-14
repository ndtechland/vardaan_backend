using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Models.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public Nullable<int> ParentCategory_Id { get; set; }
        public string ParentCategory { get; set; }
        public SelectList Categories { get; set; }
    }
}