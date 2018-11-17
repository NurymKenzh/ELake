using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class DocumentType
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name { get; set; }
    }

    public class DocumentTypeIndexPageViewModel
    {
        public IEnumerable<DocumentType> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
