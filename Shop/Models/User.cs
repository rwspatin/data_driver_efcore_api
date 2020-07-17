using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class User{
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatorio")]
        [MaxLength(20, ErrorMessage="Este campo deve conter entre 3 e 60 catacteres")]
        [MinLength(3, ErrorMessage="Este campo deve conter entre 3 e 60 catacteres")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatorio")]
        [MaxLength(20, ErrorMessage="Este campo deve conter entre 3 e 60 catacteres")]
        [MinLength(3, ErrorMessage="Este campo deve conter entre 3 e 60 catacteres")]
        public string Password { get; set; }

        public string Role {get;set;}
    }
}