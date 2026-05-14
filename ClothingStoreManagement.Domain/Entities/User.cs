namespace ClothingStoreManagement.Domain.Entities
{
    public class User
    {
        public User(string userName, string passwordHash, UserRole role)
        {
            UserName = userName;
            PasswordHash = passwordHash;
            Role = role;
        }

        public int Id { get;  set; }
        public string UserName { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRole Role { get; private set; }
        public bool IsActive { get ; private set; } = true;
        public void UpdatePassword(string newPasswordHash) => PasswordHash = newPasswordHash;
        public void UpdateRole(UserRole newRole) => Role = newRole;     
        public void UpdateUserName(string newUserName) => UserName = newUserName;   
        public void Activate() => IsActive = true;  
        public void Deactivate() => IsActive = false;   
    }
}
