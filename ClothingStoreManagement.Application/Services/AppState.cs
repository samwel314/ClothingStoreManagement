using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreManagement.Application.Services
{
    public class AppState
    {
        public UserSessionDto? CurrentUser { get; private set; }
        public ShiftDTO? CurrentShift { get; private set; }

        public bool IsLoggedIn => CurrentUser != null;

        public event Action? OnChange;

        public void Login(UserSessionDto user)
        {
            CurrentUser = user;
            NotifyStateChanged();
        }

        public void SetCurrentShift(ShiftDTO shift)
        {
            CurrentShift = shift;
            NotifyStateChanged();
        }

        public void ClearShift()
        {
            CurrentShift = null;
            NotifyStateChanged();
        }

        public void Logout()
        {
            CurrentUser = null;
            CurrentShift = null;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
