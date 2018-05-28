﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DomainDrivenDesign.Core.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DomainDrivenDesign.Core.EventSourcingRepository
{
    public class CqrsEventSourcingRepository<TAggregate> : ICqrsEventSourcingRepository<TAggregate>
        where TAggregate : AggregateRoot, new()
    {
        //EventSourcingDbContext _eventSourcingDbContext = new EventSourcingDbContext();

        private static IDatabaseInitializer<EventSourcingDbContext> _databaseInitializer =
            new CreateDatabaseIfNotExists<EventSourcingDbContext>();

        private IEventPublisher _eventPublisher;

        public CqrsEventSourcingRepository(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
            _databaseInitializer.InitializeDatabase(new EventSourcingDbContext());
        }

        public TAggregate Get(object aggregateId)
        {
            List<EventSourcingDescription> eventsHistory;
            var xaggregateId = aggregateId.ToString();
            using (var db = new EventSourcingDbContext())
            {
                eventsHistory = db.EventSoucings.AsNoTracking()
                    .Where(i => i.AggregateId.Equals(xaggregateId, StringComparison.OrdinalIgnoreCase)
                                && i.AggregateType.Equals(typeof(TAggregate).AssemblyQualifiedName, StringComparison.OrdinalIgnoreCase)
                    ).OrderBy(i => i.Version).ThenBy(i => i.CreatedDate).ToList();
            }

            if (eventsHistory.Any() == false)
                throw new AggregateNotFoundException(
                    $"Not found AggregateType: {typeof(TAggregate).FullName} with Id: {aggregateId}");

            TAggregate a = new TAggregate();

            List<IEvent> convertedEvents = new List<IEvent>();

            foreach (var e in eventsHistory)
            {
                try
                {
                    var jobj = JsonConvert.DeserializeObject(e.EventData) as JObject;
                    var objectType = Type.GetType(e.EventType, false, true);

                    if (objectType == null || jobj == null) continue;

                    var o = jobj.ToObject(objectType);
                    convertedEvents.Add((IEvent)o);
                }
                catch (Exception ex)
                {
                    //todo:should consider what should do
                    throw new AggregateHistoryBuilderException($"Check event types for AggregateType: {typeof(TAggregate).FullName} with Id: {aggregateId}", ex);
                }
            }

            a.LoadFromHistory(convertedEvents);

            return a;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggregate"></param>
        /// <param name="expectedVersion">-1: automatic get lastest version</param>
        public void Save(TAggregate aggregate, int expectedVersion = -1)
        {
            if (string.IsNullOrEmpty(aggregate.Id)) throw new ArgumentNullException($"Aggregate with Id null");

            List<EventSourcingDescription> eventChanges = new List<EventSourcingDescription>();

            var aggregateChangeds = aggregate.Changes.ToList();
            var xaggregateId = aggregate.Id;
            #region check lastest version

            long lastVersion=0;
            using (var db = new EventSourcingDbContext())
            {
                lastVersion = db.EventSoucings
                    .AsNoTracking()
                    .Where(i => i.AggregateId.Equals(xaggregateId, StringComparison.OrdinalIgnoreCase)
                                && i.AggregateType.Equals(typeof(TAggregate).AssemblyQualifiedName,
                                    StringComparison.OrdinalIgnoreCase))
                    .Select(i => i.Version)
                    .OrderByDescending(i => i)
                    .FirstOrDefault();
            }

            if (expectedVersion >= 0)
            {
                if (lastVersion != expectedVersion)
                {
                    throw new AggregateConflickVersionException(
                        string.Format(
                            "Version conflick for AggregateType: {0} with Id: {1} Latest version: {2} Expected version: {3}"
                            , aggregate.GetType().FullName, aggregate.Id, lastVersion, expectedVersion));
                }
                lastVersion = expectedVersion;
            }
            #endregion

            foreach (var e in aggregateChangeds)
            {
                lastVersion++;
                e.Version = lastVersion;
                //build event data add to event store db
                eventChanges.Add(new EventSourcingDescription()
                {
                    EsdId = Guid.NewGuid(),
                    AggregateId = aggregate.Id.ToString(),
                    AggregateType = typeof(TAggregate).AssemblyQualifiedName,
                    EventData = JsonConvert.SerializeObject(e),
                    EventType = e.GetType().AssemblyQualifiedName,
                    Version = lastVersion,
                    CreatedDate = DateTime.Now
                });
            }

            //save to event store db
            using (var db = new EventSourcingDbContext())
            {
                db.EventSoucings.AddRange(eventChanges);
                db.SaveChanges();
            }

            //publish event
            foreach (var e in aggregateChangeds)
            {
                //todo:publish event  
                //DistributedServices.Publish(new DistributedCommand(e), true);
                _eventPublisher.Publish(e);
            }
        }

        /// <summary>
        /// CreateNew work same as Save(aggregate,-1)
        /// </summary>
        /// <param name="aggregate"></param>
        public void CreateNew(TAggregate aggregate)
        {
            Save(aggregate, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggregateId"></param>
        /// <param name="aggregateDoActionsBeforeSave">Action before save</param>
        /// <param name="expectedVersion">-1: automatic get lastest version</param>
        public void GetDoSave(object aggregateId
            , Action<TAggregate> aggregateDoActionsBeforeSave
            , int expectedVersion = -1)
        {
            var aggregate = Get(aggregateId);
            aggregateDoActionsBeforeSave(aggregate);
            Save(aggregate, expectedVersion);
        }
    }
}