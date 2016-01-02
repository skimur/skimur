namespace Skimur.App.ReadModel
{
    public class SubWrapped
    {
        private int? _fuzzedNumberOfSubscribers;

        public SubWrapped(Sub sub)
        {
            Sub = sub;
        }

        public Sub Sub { get; private set; }

        public bool IsSubscribed { get; set; }

        public int NumberOfSubscribers
        {
            get
            {
                if (_fuzzedNumberOfSubscribers.HasValue)
                    return _fuzzedNumberOfSubscribers.Value;
                return Sub.NumberOfSubscribers;
            }
        }

        public void FuzzNumberOfSubscribers(int fuzzed)
        {
            if (_fuzzedNumberOfSubscribers.HasValue)
                return;

            _fuzzedNumberOfSubscribers = fuzzed;
        }

        public bool IsNumberOfSubscribersFuzzed
        {
            get { return _fuzzedNumberOfSubscribers.HasValue; }
        }
    }
}
