﻿using System.ComponentModel.DataAnnotations;

namespace AppSecurityPracticalAssignment210733T.ViewModels
{
    public class Login
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        [Required]
        public string Token {get; set; }
    }
}
