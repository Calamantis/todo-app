namespace todo_backend.Services.SecurityService
{
        public interface IPasswordService
        {
            string Hash(string plainPassword);
            bool Verify(string hashedPassword, string plainPassword);
        }

}
