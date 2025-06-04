﻿using System.ComponentModel.DataAnnotations;

namespace prototipo2.Models
{
    public class Login
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string ? Username  { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string ? Password { get; set; }
    }
}
