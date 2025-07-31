namespace Banking.FanFinancing.Shared.Helpers
{
    public class SnowflakeIdGenerator
    {
        private const long Twepoch = 1941631200000L; // Twitter epoch (can be customized)
        private const int WorkerIdBits = 5;
        private const int DatacenterIdBits = 5;
        private const int SequenceBits = 12;

        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);          // 31
        private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);  // 31

        private const int WorkerIdShift = SequenceBits;
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);         // 4095

        private readonly object _lock = new();
        private long _lastTimestamp = -1L;
        private long _sequence = 0L;

        public long WorkerId { get; }
        public long DatacenterId { get; }

        public SnowflakeIdGenerator(long workerId, long datacenterId)
        {
            if (workerId > MaxWorkerId || workerId < 0)
                throw new ArgumentException($"workerId must be between 0 and {MaxWorkerId}");

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
                throw new ArgumentException($"datacenterId must be between 0 and {MaxDatacenterId}");

            WorkerId = workerId;
            DatacenterId = datacenterId;
        }
        public long NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();

                if (timestamp < _lastTimestamp)
                    throw new Exception($"Clock moved backwards. Refusing to generate id for {_lastTimestamp - timestamp} ms");

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & SequenceMask;
                    if (_sequence == 0)
                        timestamp = TilNextMillis(_lastTimestamp);
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;

                return ((timestamp - Twepoch) << TimestampLeftShift) |
                       (DatacenterId << DatacenterIdShift) |
                       (WorkerId << WorkerIdShift) |
                       _sequence;
            }
        }

        private static long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }
        private static long TimeGen() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    }
}
