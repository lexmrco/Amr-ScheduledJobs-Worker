using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Learning.Scheduled
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Scheduled();           
        }

        public async Task Scheduled()
        {
            /// Cron Expressions. Expresión que indica a que hora ejecutar una tarea
            /// Segundos Minutos Horas DíasDelMes Meses DíasDeLaSemana
            /// Ejemplos:
            /// (0 0/5 * * * ?) Cada 5 minutos
            /// (0 0 10 ? * MON-FRI) De lunes a viernes a las 10:00 a.m
            /// (0 45 20 * * ?) Todos los días a las 08:45 P-M

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = await schedulerFactory.GetScheduler();

            IJobDetail jobDetail = JobBuilder.Create<HelloWorldJob>()
                .WithIdentity("TestJob")
                .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .WithCronSchedule("0 16 10 * * ?")
                .WithIdentity("TestTrigger")
                .StartNow()
                .Build();
            await scheduler.ScheduleJob(jobDetail, trigger);
            await scheduler.Start();
        }
    }
}
