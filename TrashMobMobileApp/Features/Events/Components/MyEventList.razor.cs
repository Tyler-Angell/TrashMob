﻿using Microsoft.AspNetCore.Components;
using TrashMob.Models;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Shared;
using TrashMobMobileApp.StateContainers;

namespace TrashMobMobileApp.Features.Events.Components
{
    public partial class MyEventList
    {
        private List<Event> _myEvents = new();
        private List<Event> _myEventsStatic = new();
        private bool _isLoading;
        private string _eventSearchText;
        private bool _isViewOpen;
        private Event _selectedEvent;

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Inject]
        public UserStateInformation StateInformation { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetMyEventsAsync();
        }

        private async Task GetMyEventsAsync()
        {
            var currentUser = App.CurrentUser;
            if (currentUser != null)
            {
                _isLoading = true;
                _myEventsStatic = (await MobEventManager.GetUserEventsAsync(currentUser.Id, StateInformation.ShowFutureEvents)).ToList();
                _myEvents = _myEventsStatic;
                _isLoading = false;
            }
        }

        private void OnSearchTextChanged(string searchText)
        {
            _eventSearchText = searchText;
            if (string.IsNullOrEmpty(_eventSearchText))
            {
                _myEvents = _myEventsStatic;
                return;
            }

            _myEvents = _myEventsStatic.FindAll(item => item.Name.Contains(_eventSearchText, StringComparison.OrdinalIgnoreCase));
        }

        private void OnCreateEvent() => Navigator.NavigateTo(Routes.CreateEvent);

        private void OnViewEventDetails(Event mobEvent)
        {
            _selectedEvent = mobEvent;
            _isViewOpen = !_isViewOpen;
        }

        private void OnCompleteEvent(Event mobEvent)
            => Navigator.NavigateTo(string.Format(Routes.CompleteEvent, mobEvent.Id.ToString()));

        private async Task OnShowFutureEventsChangedAsync(bool val)
        {
            StateInformation.ShowFutureEvents = val;
            await GetMyEventsAsync();
        }

        private void OnCancelEvent(Event mobEvent) 
            => Navigator.NavigateTo(string.Format(Routes.CancelEvent, mobEvent.Id.ToString()));

        private void OnEdit(Event mobEvent)
            => Navigator.NavigateTo(string.Format(Routes.EditEvent, mobEvent.Id));
    }
}
