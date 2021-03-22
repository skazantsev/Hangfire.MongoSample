using System;

namespace Hangfire.MongoSample.Mongo
{
    public class Appointment
    {
        public Guid Id { get; set; }

        public DateTime Start { get; set; }
    }
}
