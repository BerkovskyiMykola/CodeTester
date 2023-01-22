// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations;

namespace IdentityServerHost.Pages.Login;

public class InputModel
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public bool RememberLogin { get; set; }

    public string ReturnUrl { get; set; } = string.Empty;

    public string Button { get; set; } = string.Empty;
}