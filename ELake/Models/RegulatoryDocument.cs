using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class RegulatoryDocument
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DocumentType")]
        public int DocumentTypeId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DocumentType")]
        public DocumentType DocumentType { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Number")]
        public string Number { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "ForceEntryYear")]
        public int ForceEntryYear { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "ForceEntryMonth")]
        [Range(1, 12)]
        public int? ForceEntryMonth { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "ForceEntryDay")]
        [Range(1, 31)]
        public int? ForceEntryDay { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "ForceEntryDayDateYear")]
        public string ForceEntryDayDateYear
        {
            get
            {
                if(ForceEntryMonth == null || ForceEntryDay == null)
                {
                    return ForceEntryYear.ToString();
                }
                else
                {
                    DateTime forceEntryDayDateYear = new DateTime(ForceEntryYear, (int)ForceEntryMonth, (int)ForceEntryDay);
                    return forceEntryDayDateYear.ToShortDateString();
                }
            }
        }

        //[Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "File")]
        //public string File { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PreviousDocument")]
        public int? PreviousDocumentId { get; set; }
        //[Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PreviousDocument")]
        //public RegulatoryDocument PreviousDocument { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Archival")]
        public bool Archival { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DeletingJustification")]
        public string DeletingJustification { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NewDocument")]
        public int? NewDocumentId { get; set; }
        //[Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NewDocument")]
        //public RegulatoryDocument NewDocument { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Description")]
        public string Description { get; set; }
    }

    public class RegulatoryDocumentIndexPageViewModel
    {
        public IEnumerable<RegulatoryDocument> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
