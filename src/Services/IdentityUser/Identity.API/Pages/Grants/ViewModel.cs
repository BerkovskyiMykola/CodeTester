namespace IdentityServerHost.Pages.Grants;

public class ViewModel
{
    public IEnumerable<GrantViewModel> Grants { get; set; } = Enumerable.Empty<GrantViewModel>();
}

public class GrantViewModel
{
    public string ClientId { get; set; } = string.Empty; 
    public string ClientName { get; set; } = string.Empty; 
    public string ClientUrl { get; set; } = string.Empty; 
    public string ClientLogoUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime? Expires { get; set; }
    public IEnumerable<string> IdentityGrantNames { get; set; } = new List<string>();
    public IEnumerable<string> ApiGrantNames { get; set; } = Enumerable.Empty<string>();
}