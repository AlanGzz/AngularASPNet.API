using System.ComponentModel.DataAnnotations;

namespace AngularASPNet.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "La contraseña debe tener al menos 4 y no mas de 8 caracteres.")]
        public string Password { get; set; }
    }
}