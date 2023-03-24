namespace Common.Models.Audition;

public interface IModificationAudited : IHasModificationTime
{
    string? LastModifierUserId { get; set; }
}
