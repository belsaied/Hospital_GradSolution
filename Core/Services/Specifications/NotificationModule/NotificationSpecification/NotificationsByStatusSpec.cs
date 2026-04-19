using Domain.Models.Enums.NotificationEnums;
using Domain.Models.NotificationModule;

namespace Services.Specifications.NotificationModule.NotificationSpecification
{
    public sealed class NotificationsByStatusSpec : BaseSpecifications<Notification, Guid>
    {
        public NotificationsByStatusSpec(string userId, DeliveryStatus status)
            : base(n => n.RecipientUserId == userId && n.DeliveryStatus == status)
        {
            AddOrderByDescending(n => n.CreatedAt);
        }
    }
}
