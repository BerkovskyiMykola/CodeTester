// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace IdentityServerHost.Pages.Consent;

public class InputModel
{
    public string Button { get; set; } = string.Empty;
    public IEnumerable<string> ScopesConsented { get; set; } = Enumerable.Empty<string>();
    public bool RememberConsent { get; set; } = true;
    public string ReturnUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}