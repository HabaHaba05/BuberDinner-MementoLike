using BuberDinner.SharedKernel;

using Newtonsoft.Json;

namespace BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;

public class IdentityMap
{
    public List<IdentityMapItem> Map { get; } = new();

    public List<IAggregateRoot> FindByProperties(List<KeyValuePair<string, object?>> properties)
    {
        var result = Map
            .Where(x => properties
                .All(p => JsonConvert.SerializeObject(x.Entity.GetState()[p.Key]) == JsonConvert.SerializeObject(p.Value)))
            .Select(x => x.Entity)
            .ToList();
        return result;
    }

    public void AddAlreadyExisting(IAggregateRoot entity)
    {
        var mapItem = FindByKeys(entity);
        if (mapItem is not null)
        {
            if (!AreIdenticalDictionaries(mapItem.OriginalState!, entity.GetState()))
            {
                mapItem.Entity = entity;
                mapItem.EntityDbState = EntityState.Modified;
            }
        }
        else
        {
            var originalStateStr = JsonConvert.SerializeObject(entity.GetState());
            mapItem = new IdentityMapItem()
            {
                OriginalState = JsonConvert.DeserializeObject<Dictionary<string, object?>>(originalStateStr),
                Entity = entity,
                EntityDbState = EntityState.Unchanged,
            };
            Map.Add(mapItem);
        }
    }

    public bool AddNew(IAggregateRoot entity)
    {
        var mapItem = FindByKeys(entity);
        if (mapItem is not null)
        {
            return false;
        }

        Map.Add(new IdentityMapItem() { Entity = entity, EntityDbState = EntityState.Added });
        return true;
    }

    public bool MarkReadyForDeletion(IAggregateRoot entity)
    {
        var mapItem = FindByKeys(entity);
        if (mapItem is not null)
        {
            mapItem.EntityDbState = EntityState.Deleted;
            return true;
        }

        return false;
    }

    public IdentityMapItem? FindByKeys(IAggregateRoot entity)
    {
        Func<Dictionary<string, object?>, Dictionary<string, object?>> getKeyValues = (st) =>
        {
            var keyValues = new Dictionary<string, object?>();
            ((IEnumerable<string>)st["Keys"]!).ToList().ForEach(k => keyValues.Add(k, st[k]));
            return keyValues;
        };

        var incomingStateKeyValues = getKeyValues(entity.GetState());
        var mapItem = Map.SingleOrDefault(
            x => AreIdenticalDictionaries(incomingStateKeyValues, getKeyValues(x.Entity.GetState())));

        return mapItem;
    }

    private bool AreIdenticalDictionaries(Dictionary<string, object?> state1, Dictionary<string, object?> state2) =>
        JsonConvert.SerializeObject(state1) == JsonConvert.SerializeObject(state2);
}

public class IdentityMapItem
{
    public Dictionary<string, object?>? OriginalState { get; set; }
    public IAggregateRoot Entity { get; set; } = default!;
    public EntityState EntityDbState { get; set; }
}