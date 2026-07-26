using System;

namespace Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditGroupAgg
{
    public static class MicroCreditGroupFactory
    {
        public static MicroCreditGroup CreateMicroCreditGroup(Guid? parentId, Guid customerId, Guid microCreditOfficerId, int type, string purpose, string activities, int meetingFrequency, int meetingDayOfWeek, string meetingPlace, int minimumMembers, int maximumMembers, string remarks)
        {
            var microCreditGroup = new MicroCreditGroup();

            microCreditGroup.GenerateNewIdentity();

            microCreditGroup.ParentId = parentId;

            microCreditGroup.CustomerId = customerId;

            microCreditGroup.MicroCreditOfficerId = microCreditOfficerId;

            microCreditGroup.Type = type;

            microCreditGroup.Purpose = purpose;

            microCreditGroup.Activities = activities;

            microCreditGroup.MeetingFrequency = meetingFrequency;

            microCreditGroup.MeetingDayOfWeek = meetingDayOfWeek;

            microCreditGroup.MeetingPlace = meetingPlace;

            microCreditGroup.MinimumMembers = minimumMembers;

            microCreditGroup.MaximumMembers = maximumMembers;

            microCreditGroup.Remarks = remarks;

            microCreditGroup.CreatedDate = DateTime.Now;

            return microCreditGroup;
        }
    }
}
