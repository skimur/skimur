
using Skimur.Data.Mapper;

namespace Skimur.App.ClassMappers
{
    public class UserMapper : Data.Mapper.ClassMapper<User>
    {
        public UserMapper()
        {
            Table("users");
            Map(x => x.Id).Key(KeyType.Guid).Column("id");
            Map(x => x.CreatedDate).Column("created_date");
            Map(x => x.UserName).Column("user_name");
            Map(x => x.Email).Column("email");
            Map(x => x.EmailConfirmed).Column("email_confirmed");
            Map(x => x.PasswordHash).Column("password_hash");
            Map(x => x.SecurityStamp).Column("security_stamp");
            Map(x => x.PhoneNumber).Column("phone_number");
            Map(x => x.PhoneNumberConfirmed).Column("phone_number_confirmed");
            Map(x => x.TwoFactorEnabled).Column("two_factor_enabled");
            Map(x => x.LockoutEndDate).Column("lockout_end_date");
            Map(x => x.LockoutEnabled).Column("lockout_enabled");
            Map(x => x.AccessFailedCount).Column("access_failed_count");
            Map(x => x.SecurityQuestion).Column("security_question");
            Map(x => x.SecurityAnswer).Column("security_answer");
            Map(x => x.FullName).Column("full_name");
            Map(x => x.Bio).Column("bio");
            Map(x => x.Url).Column("url");
            Map(x => x.Location).Column("location");
            Map(x => x.AvatarIdentifier).Column("avatar_identifier");
            Map(x => x.IsAdmin).Column("is_admin");
            Map(x => x.EnableStyles).Column("styles");
            Map(x => x.Ip).Column("ip");
        }
    }
}
