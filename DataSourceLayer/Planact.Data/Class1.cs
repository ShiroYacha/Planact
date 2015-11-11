using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Planact.Data
{
    [Table("FU")]
    public class Class1
    {
        [Key]
        public int Id { get; set; }

        public string FUCKU { get; set; }
    }
}
