using Microsoft.AspNetCore.Identity;

namespace todo_backend.Services.SecurityService
{
        public class PasswordService : IPasswordService
        {
            private readonly PasswordHasher<object> _hasher;

            public PasswordService()
            {
                _hasher = new PasswordHasher<object>();
            }

            public string Hash(string plainPassword)
            {
                // zwraca sformatowany hash zawierający salt i parametry
                return _hasher.HashPassword(null!, plainPassword);
            }

            public bool Verify(string hashedPassword, string plainPassword)
            {
                var result = _hasher.VerifyHashedPassword(null!, hashedPassword, plainPassword);
                return result == PasswordVerificationResult.Success
                       || result == PasswordVerificationResult.SuccessRehashNeeded;
                // SuccessRehashNeeded oznacza, że hash wymaga rehashu (możesz przy logowaniu nadpisać)
            }
        }
}
