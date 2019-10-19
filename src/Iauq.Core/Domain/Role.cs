using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Iauq.Core.Domain
{
    public class Role : EntityBase
    {
        [DisplayName("نام")]
        [Required(ErrorMessage = "ذکر نام ضروری است.")]
        [StringLength(200, ErrorMessage = "حداکثر طول نام 200 کاراکتر است.")]
        public virtual string Name { get; set; }
        
        [DisplayName("گروه")]
        public virtual int? CategoryId { get; set; }
        
        public virtual Category Category { get; set; }
        
        public virtual ICollection<User> Users { get; set; }
    }
}