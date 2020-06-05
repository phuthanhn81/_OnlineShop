namespace Model.Object
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Credential")]
    [Serializable] // muốn dùng Session
    public partial class Credential
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string UserGroupID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string RoleID { get; set; }
    }
}
