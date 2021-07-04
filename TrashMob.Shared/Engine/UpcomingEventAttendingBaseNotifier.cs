﻿
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public abstract class UpcomingEventAttendingBaseNotifier : NotificationEngineBase, INotificationEngine
    {
        public UpcomingEventAttendingBaseNotifier(IEventRepository eventRepository,
                                                     IUserRepository userRepository,
                                                     IEventAttendeeRepository eventAttendeeRepository,
                                                     IUserNotificationRepository userNotificationRepository,
                                                     IUserNotificationPreferenceRepository userNotificationPreferenceRepository,
                                                     IEmailSender emailSender,
                                                     IMapRepository mapRepository,
                                                     ILogger logger) :
            base(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, mapRepository, logger)
        {
        }

        public async Task GenerateNotificationsAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Generating Notifications for {0}", NotificationType);

            // Get list of users who have notifications turned on for locations
            var users = await UserRepository.GetAllUsers().ConfigureAwait(false);
            int notificationCounter = 0;

            Logger.LogInformation("Generating {0} Notifications for {1} total users", NotificationType, users.Count());

            // for each user
            foreach (var user in users)
            {
                if (await IsOptedOut(user).ConfigureAwait(false))
                {
                    continue;
                }

                var eventsToNotifyUserFor = new List<Event>();

                // Get list of active events
                var events = await EventRepository.GetActiveEvents().ConfigureAwait(false);

                // Get list of events user is already attending
                var eventsUserIsAttending = await EventAttendeeRepository.GetEventsUserIsAttending(user.Id).ConfigureAwait(false);

                // Limit the list of events to process to those in the next window UTC
                foreach (var mobEvent in events.Where(e => e.CreatedByUserId != user.Id && e.EventDate <= DateTimeOffset.UtcNow.AddHours(NumberOfHoursInWindow)))
                {
                    // Verify that the user is attending the event.
                    if (!eventsUserIsAttending.Any(ea => ea.Id == mobEvent.Id))
                    {
                        continue;
                    }

                    if (await UserHasAlreadyReceivedNotification(user, mobEvent).ConfigureAwait(false))
                    {
                        continue;
                    }

                    // Add to the event list to be sent
                    eventsToNotifyUserFor.Add(mobEvent);
                }

                // Populate email
                notificationCounter += await SendNotifications(user, eventsToNotifyUserFor, cancellationToken).ConfigureAwait(false);
            }

            Logger.LogInformation("Generating {0} Total {1} Notifications", notificationCounter, NotificationType);
        }
    }
}
