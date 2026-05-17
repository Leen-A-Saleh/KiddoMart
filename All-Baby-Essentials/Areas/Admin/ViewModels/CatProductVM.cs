using All_Baby_Essentials.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace All_Baby_Essentials.Areas.Admin.ViewModels
{
    public class CatProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
