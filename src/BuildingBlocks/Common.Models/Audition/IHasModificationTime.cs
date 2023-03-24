namespace Common.Models.Audition;

public interface IHasModificationTime
{
    DateTime? LastModificationTime { get; set; }
}
