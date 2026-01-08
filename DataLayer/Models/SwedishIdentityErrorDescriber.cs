using Microsoft.AspNetCore.Identity;
namespace DataLayer.Models
{
    public class SwedishIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
            => new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"Lösenordet måste vara minst {length} bokstäver."
            };

        public override IdentityError PasswordRequiresDigit()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = $"Lösenordet måste innehålla en siffra."
            };

        public override IdentityError PasswordRequiresUpper()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = $"Lösenordet måste innehålla minst en stor bokstav."
            };

        public override IdentityError PasswordRequiresNonAlphanumeric()
           => new IdentityError
           {
               Code = nameof(PasswordRequiresNonAlphanumeric),
               Description = $"Lösenordet måste innehålla minst ett specialtecken (t.ex. !, @, #, ?)."
           };

        public override IdentityError DuplicateUserName(string userName)
        => new IdentityError
        {
            Code = nameof(DuplicateUserName),
            Description = $"Användarnamnet '{userName}' är upptaget."
        };
    }

}
