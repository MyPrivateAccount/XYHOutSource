using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationCore.Dto
{
    public static class ModelStateExtension
    {
        public static string GetAllErrors(this ModelStateDictionary ModelState)
        {
            if (ModelState.IsValid)
                return "";
            var error = "";
            var errors = ModelState.Values.ToList();
            foreach (var item in errors)
            {
                foreach (var e in item.Errors)
                {
                    error += e.Exception.Message + "  ";
                }
            }
            return error;

        }
    }
}
