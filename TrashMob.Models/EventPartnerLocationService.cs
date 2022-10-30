﻿#nullable disable

namespace TrashMob.Models
{
    using System;

    public partial class EventPartnerLocationService : BaseModel
    {
        public EventPartnerLocationService()
        {
        }

        public Guid EventId { get; set; }

        public Guid PartnerLocationId { get; set; }

        public int ServiceTypeId { get; set; }

        public int EventPartnerLocationServiceStatusId { get; set; }

        public virtual PartnerLocation PartnerLocation { get; set; }

        public virtual Event Event { get; set; }

        public virtual ServiceType ServiceType { get; set; }

        public virtual EventPartnerLocationServiceStatus EventPartnerLocationServiceStatus { get; set; }
    }
}
