namespace Common.Models.Audition;

public interface ICreationAudited : IHasCreationTime
{
    string? CreatorUserId { get; set; }
}