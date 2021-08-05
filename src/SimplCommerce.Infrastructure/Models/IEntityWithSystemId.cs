namespace SimplCommerce.Infrastructure.Models
{
    /// <summary>
    /// Provide a SystemId Column to support mutiple system design
    /// Added by Lin SianTing at 20210805
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IEntityWithSystemId<TId>
    {
        TId SystemId { get; }
    }
}
