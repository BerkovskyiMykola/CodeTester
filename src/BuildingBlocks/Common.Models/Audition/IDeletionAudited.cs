namespace Common.Models.Audition;

public interface IDeletionAudited : ISoftDelete
{
    string? DeleterUserId { get; set; }

    DateTime? DeletionTime { get; set; }
}
