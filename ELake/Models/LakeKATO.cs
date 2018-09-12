using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class LakeKATO
    {
        public int Id { get; set; }
        public int LakeId { get; set; }
        public int KATOId { get; set; }
        public KATO KATO { get; set; }
    }
}
