namespace Skimur.App.ReadModel
{
    public class SubUserBanWrapped
    {
        public SubUserBanWrapped(SubUserBan userBan)
        {
            Item = userBan;
        }

        public SubUserBan Item { get; set; }

        public User User { get; set; }

        public User BannedBy { get; set; }
    }
}
