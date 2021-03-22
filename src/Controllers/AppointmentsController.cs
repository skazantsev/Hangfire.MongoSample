using Hangfire.MongoSample.Mongo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hangfire.MongoSample.Controllers
{
    [ApiController]
    [Route("appointments")]
    public class AppointmentsController : ControllerBase
    {
        private static readonly Random Rng = new();

        private readonly MongoContext _mongo;

        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(MongoContext mongo, ILogger<AppointmentsController> logger)
        {
            _mongo = mongo;
            _logger = logger;
        }

        [HttpGet]
        public List<Appointment> GetList()
        {
            return _mongo.Appointments.Find(x => true).ToList();
        }

        [HttpPost]
        public Appointment Create()
        {
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                Start = DateTime.UtcNow + TimeSpan.FromHours(Rng.Next(1, 5))
            };
            _mongo.Appointments.InsertOne(appointment);

            _logger.LogInformation($"Created appointment {appointment.Id}.");

            var remindIn = Rng.Next(15, 60);
            BackgroundJob.Schedule<RemainderService>(
                x => x.Send(appointment.Id),
                TimeSpan.FromSeconds(remindIn)
            );

            _logger.LogInformation($"Scheduled a reminder in {remindIn} seconds.");

            return appointment;
        }
    }

    public class RemainderService
    {
        private readonly ILogger<RemainderService> _logger;

        public RemainderService(ILogger<RemainderService> logger) =>  _logger = logger;

        public void Send(Guid appointmentId)
        {
            _logger.LogInformation($@"Remainding of {appointmentId}.");
        }
    }
}
