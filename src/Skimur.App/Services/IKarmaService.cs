using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.Services
{
    public interface IKarmaService
    {
        void AdjustKarma(Guid userId, Guid subId, KarmaType karmaType, int change);
        
        void DeleteAllKarmaForUser(Guid userId);

        void IncreaseKarma(Guid userId, Guid subId, KarmaType karmaType);

        void DecreaseKarma(Guid userId, Guid subId, KarmaType karmaType);

        Dictionary<KarmaReportKey, int> GetKarma(Guid userId);
    }

    public enum KarmaType
    {
        Post,
        Comment
    }

    public class KarmaReportKey
    {
        private readonly Guid _subId;
        private readonly KarmaType _type;

        public KarmaReportKey(Guid subId, KarmaType type)
        {
            _subId = subId;
            _type = type;
        }

        public Guid SubId { get { return _subId; } }

        public KarmaType Type { get { return _type; } }

        public override bool Equals(object obj)
        {
            if (!(obj is KarmaReportKey)) return false;

            var key = (KarmaReportKey) obj;

            if (!key.SubId.Equals(SubId)) return false;
            if (!key.Type.Equals(Type)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return SubId.GetHashCode() + Type.GetHashCode();
        }
    }
}
