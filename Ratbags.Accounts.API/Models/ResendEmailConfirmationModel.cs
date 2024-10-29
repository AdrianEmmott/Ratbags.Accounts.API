﻿using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.API.Models;

public class ResendEmailConfirmationModel
{
    [Required(ErrorMessage = "Email address is required")]
    public string Email { get; set; }=string.Empty;
}
