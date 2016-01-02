namespace Skimur.App.ReadModel
{
    public class ModeratorWrapped
    {
        public ModeratorWrapped(Moderator moderator)
        {
            Moderator = moderator;
        }

        public Moderator Moderator { get; private set; }

        public Sub Sub { get; set; }

        public User User { get; set; }

        public bool CanRemove { get; set; }
        
        public bool CanChangePermissions { get; set; }
    }
}
