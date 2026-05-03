using backend.Enums;

namespace backend.Services.Mappers
{
    public static class QueueMappers
    {
        public static string ToRedisKey(this QueueKey key)
            => key switch
            {
                QueueKey.Bullet_1_0 => "chess:queue:1:0",
                QueueKey.Bullet_1_1 => "chess:queue:1:1",
                QueueKey.Bullet_1_2 => "chess:queue:1:2",
                QueueKey.Bullet_2_0 => "chess:queue:2:0",
                QueueKey.Bullet_2_1 => "chess:queue:2:1",
                QueueKey.Bullet_2_2 => "chess:queue:2:2",

                QueueKey.Blitz_3_0 => "chess:queue:3:0",
                QueueKey.Blitz_3_1 => "chess:queue:3:1",
                QueueKey.Blitz_3_2 => "chess:queue:3:2",
                QueueKey.Blitz_5_0 => "chess:queue:5:0",
                QueueKey.Blitz_5_3 => "chess:queue:5:3",
                QueueKey.Blitz_5_5 => "chess:queue:5:5",

                QueueKey.Rapid_10_0 => "chess:queue:10:0",
                QueueKey.Rapid_10_2 => "chess:queue:10:2",
                QueueKey.Rapid_15_0 => "chess:queue:15:0",
                QueueKey.Rapid_15_5 => "chess:queue:15:5",
                QueueKey.Rapid_30_0 => "chess:queue:30:0",
                QueueKey.Rapid_30_15 => "chess:queue:30:15",
                _ => throw new ArgumentOutOfRangeException()
            };
        public static int GetTime(this QueueKey key) => key switch
        {
            QueueKey.Bullet_1_0 => 1,
            QueueKey.Bullet_1_1 => 1,
            QueueKey.Bullet_1_2 => 1,
            QueueKey.Bullet_2_0 => 2,
            QueueKey.Bullet_2_1 => 2,
            QueueKey.Bullet_2_2 => 2,

            QueueKey.Blitz_3_0 => 3,
            QueueKey.Blitz_3_1 => 3,
            QueueKey.Blitz_3_2 => 3,
            QueueKey.Blitz_5_0 => 5,
            QueueKey.Blitz_5_3 => 5,
            QueueKey.Blitz_5_5 => 5,

            QueueKey.Rapid_10_0 => 10,
            QueueKey.Rapid_10_2 => 10,
            QueueKey.Rapid_15_0 => 15,
            QueueKey.Rapid_15_5 => 15,
            QueueKey.Rapid_30_0 => 30,
            QueueKey.Rapid_30_15 => 30,
            _ => throw new ArgumentOutOfRangeException()
        };
        public static int GetIncrement(this QueueKey key) => key switch
        {
            QueueKey.Bullet_1_0 => 0,
            QueueKey.Bullet_1_1 => 1,
            QueueKey.Bullet_1_2 => 2,
            QueueKey.Bullet_2_0 => 0,
            QueueKey.Bullet_2_1 => 1,
            QueueKey.Bullet_2_2 => 2,

            QueueKey.Blitz_3_0 => 0,
            QueueKey.Blitz_3_1 => 1,
            QueueKey.Blitz_3_2 => 2,
            QueueKey.Blitz_5_0 => 0,
            QueueKey.Blitz_5_3 => 3,
            QueueKey.Blitz_5_5 => 5,

            QueueKey.Rapid_10_0 => 0,
            QueueKey.Rapid_10_2 => 2,
            QueueKey.Rapid_15_0 => 0,
            QueueKey.Rapid_15_5 => 5,
            QueueKey.Rapid_30_0 => 0,
            QueueKey.Rapid_30_15 => 15,
            _ => throw new ArgumentOutOfRangeException()
        };


    }
}