namespace TrashMob.Shared.Tests
{
    using TrashMob.Shared.Engine;
    using Xunit;

    public class UpcomingEventsInYourAreaThisWeekNotifierTests : UpcomingEventsInYourAreaNotifierTestsBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaThisWeek;

        protected override INotificationEngine Engine => new UpcomingEventsInYourAreaThisWeekNotifier(EventManager.Object, UserManager.Object, EventAttendeeManager.Object, UserNotificationManager.Object, NonEventUserNotificationManager.Object, EmailSender.Object, EmailManager.Object, MapRepository.Object, Logger.Object);

        protected override int NumberOfDaysToAddForEventOutOfWindow => 8;
    }
}
