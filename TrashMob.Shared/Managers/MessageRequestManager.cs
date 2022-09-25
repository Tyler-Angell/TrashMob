﻿
namespace TrashMob.Shared.Managers
{
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class MessageRequestManager : IMessageRequestManager
    {
        private readonly INotificationManager notificationManager;
        private readonly IMessageRequestRepository messageRequestRepository;

        public MessageRequestManager(IMessageRequestRepository messageRequestRepository, INotificationManager notificationManager)
        {
            this.messageRequestRepository = messageRequestRepository;
            this.notificationManager = notificationManager;
        }      

        public async Task SendMessageRequest(MessageRequest messageRequest)
        {
            await messageRequestRepository.AddMessageRequest(messageRequest);
            await notificationManager.SendMessageRequest(messageRequest);
        }
    }
}
