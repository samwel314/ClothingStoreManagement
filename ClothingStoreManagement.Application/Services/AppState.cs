using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreManagement.Application.Services
{
    public class AppState 
    {
        public UserSessionDto? CurrentUser { get; private set; }
        public ShiftSessionDto? CurrentShift { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;
        public event Action? OnChange;
        public void Login(UserSessionDto user)
        {
            CurrentUser = user;
            NotifyStateChanged(); /// -*-*-*-*-* ---------------*-------
        }
        public void CompleteShift(ShiftSessionDto shift)
        {
            CurrentShift = shift;
            NotifyStateChanged();
        }
        public void Logout()
        {
            CurrentUser = null;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
