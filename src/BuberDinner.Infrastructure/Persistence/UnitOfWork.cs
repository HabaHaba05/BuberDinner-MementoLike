using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;

using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Helpers;
using BuberDinner.Infrastructure.Persistence.Repositories;
using BuberDinner.Infrastructure.Persistence.Repositories.Decorators;
using BuberDinner.SharedKernel;

using Dapper;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using static Dapper.SqlMapper;

using EntityState = BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.EntityState;

namespace BuberDinner.Infrastructure.Persistence;
public class UnitOfWork : IUnitOfWork
{
    private readonly string _connectionString;
    private readonly IMediator _mediator;
    private IDbConnection _connection = null!;
    private IDbTransaction _transaction = null!;

    private IAdminRepository? _adminRepository;
    private IUserRepository? _userRepository;
    private IBecomeHostRequestRepository? _becomeHostRequestRepository;
    private IGuestRepository? _guestRepository;
    private IHostRepository? _hostRepository;
    private IMenuRepository? _menuRepository;
    private IMenuReviewRepository? _menuReviewRepository;
    private IBillRepository? _billRepository;
    private IDinnerRepository? _dinnerRepository;

    private List<IdentityMap> _identityMaps = new();

    private bool _disposed;

    public UnitOfWork(IConfiguration configuration, IMediator mediator)
    {
        _mediator = mediator;
        _connectionString = configuration.GetConnectionString("BuberDinner")!;
    }

    public void Begin()
    {
        _connection = new SqlConnection(_connectionString);
        _connection.Open();
        _transaction = _connection.BeginTransaction();
        _identityMaps = new();
    }

    public void Commit()
    {
        try
        {
            PublishDomainEvents().Wait();
            WriteAudit().Wait();
            _transaction.Commit();
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
        finally
        {
            ResetEverything();
        }
    }

    public void Rollback()
    {
        _transaction.Rollback();
        ResetEverything();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void ResetEverything()
    {
        _transaction.Dispose();
        _identityMaps = new();
        ResetRepositories();
    }

    private void ResetRepositories()
    {
        _adminRepository = null;
        _userRepository = null;
        _guestRepository = null;
        _becomeHostRequestRepository = null;
        _menuRepository = null;
        _menuReviewRepository = null;
        _billRepository = null;
        _hostRepository = null;
        _dinnerRepository = null;
    }

    public IAdminRepository AdminRepository =>
    _adminRepository ??= new AdminRepositoryDecorator(CreateIdentityMapAndAssignToList(), new AdminRepository(_transaction));
    public IUserRepository UserRepository =>
        _userRepository ??= new UserRepositoryDecorator(CreateIdentityMapAndAssignToList(), new UserRepository(_transaction));
    public IMenuReviewRepository MenuReviewRepository =>
        _menuReviewRepository ??= new MenuReviewRepositoryDecorator(CreateIdentityMapAndAssignToList(), new MenuReviewRepository(_transaction));

    public IBecomeHostRequestRepository BecomeHostRequestRepository
    {
        get
        {
            if (_becomeHostRequestRepository is null)
            {
                var identityMap = CreateIdentityMapAndAssignToList();
                _becomeHostRequestRepository = new BecomeHostRequestRepositoryDecorator(identityMap, new BecomeHostRequestRepository(identityMap, _transaction));
            }

            return _becomeHostRequestRepository;
        }
    }

    public IGuestRepository GuestRepository
    {
        get
        {
            if (_guestRepository is null)
            {
                var identityMap = CreateIdentityMapAndAssignToList();
                _guestRepository = new GuestRepositoryDecorator(identityMap, new GuestRepository(identityMap, _transaction));
            }

            return _guestRepository;
        }
    }

    public IHostRepository HostRepository
    {
        get
        {
            if (_hostRepository is null)
            {
                var identityMap = CreateIdentityMapAndAssignToList();
                _hostRepository = new HostRepositoryDecorator(identityMap, new HostRepository(identityMap, _transaction));
            }

            return _hostRepository;
        }
    }

    public IDinnerRepository DinnerRepository
    {
        get
        {
            if (_dinnerRepository is null)
            {
                var identityMap = CreateIdentityMapAndAssignToList();
                _dinnerRepository = new DinnerRepositoryDecorator(identityMap, new DinnerRepository(identityMap, _transaction));
            }

            return _dinnerRepository;
        }
    }

    public IMenuRepository MenuRepository
    {
        get
        {
            if (_menuRepository is null)
            {
                var identityMap = CreateIdentityMapAndAssignToList();
                _menuRepository = new MenuRepositoryDecorator(identityMap, new MenuRepository(identityMap, _transaction));
            }

            return _menuRepository;
        }
    }

    public IBillRepository BillRepository
    {
        get
        {
            if (_billRepository is null)
            {
                var identityMap = CreateIdentityMapAndAssignToList();
                _billRepository = new BillRepositoryDecorator(identityMap, new BillRepository(identityMap, _transaction));
            }

            return _billRepository;
        }
    }

    private IdentityMap CreateIdentityMapAndAssignToList()
    {
        var map = new IdentityMap();
        _identityMaps.Add(map);
        return map;
    }

    private async Task WriteAudit()
    {
        if (_identityMaps is null || _identityMaps.Count == 0)
        {
            return;
        }

        var modifiedEntities = _identityMaps
            .SelectMany(x => x.Map)
            .Where(x => x.EntityDbState != EntityState.Unchanged)
            .ToList();

        if (modifiedEntities.Count == 0)
        {
            return;
        }

        var query = "INSERT INTO Audit (ActionType, TableName, PreviousState, NewState, Changes, ActionTime)";
        var parameters = new DynamicParameters();
        var count = 0;
        var actionTime = DateTime.UtcNow;
        parameters.Add("ActionTime", actionTime);
        foreach (var mapItem in modifiedEntities)
        {
            if (count == 0)
            {
                query += " VALUES";
            }

            var rootTableName = mapItem.Entity.GetType().GetProperty(nameof(IAggregateRoot.TableName))!.GetValue(mapItem.Entity)!.ToString()!;
            var changesInTables = StatesComparator.Handle(mapItem.OriginalState ?? new(), mapItem.Entity.GetState(), rootTableName);

            foreach (var (tableName, changes) in changesInTables)
            {
                var tableNameParameterKey = $"{count}_TableName";
                parameters.Add(tableNameParameterKey, tableName);
                foreach (var change in changes)
                {
                    if (!query.EndsWith("VALUES") && query.Last() != ',')
                    {
                        query += ",";
                    }

                    var actionTypeParameterKey = $"{count}_ActionType";
                    var prevStateParameterKey = $"{count}_PreviousState";
                    var newStateParameterKey = $"{count}_NewState";
                    var changesParameterKey = $"{count}_Changes";

                    query += $"(@{actionTypeParameterKey},@{tableNameParameterKey},@{prevStateParameterKey},@{newStateParameterKey},@{changesParameterKey},@ActionTime)";
                    parameters.Add(actionTypeParameterKey, change.ActionType.ToString());
                    parameters.Add(prevStateParameterKey, JsonConvert.SerializeObject(change.PreviousState?.ToObject<Dictionary<string, object?>>()));
                    parameters.Add(newStateParameterKey, JsonConvert.SerializeObject(change.CurrentState?.ToObject<Dictionary<string, object?>>()));
                    parameters.Add(
                        changesParameterKey,
                        change.ActionType == ActionType.UPDATE
                        ? JsonConvert.SerializeObject(change.Changes?.ToObject<Dictionary<string, object?>>())
                        : null);
                    count++;
                }
            }
        }

        query += ";";
        QueryAndParametersLogger.WriteToConsoleQueryAndParameters(query, parameters);

        await _connection.ExecuteAsync(query, parameters, transaction: _transaction);
    }

    private async Task PublishDomainEvents()
    {
        if (_identityMaps is null || _identityMaps.Count == 0)
        {
            return;
        }

        bool hadDomainEvent = false;

        var entitiesWithDomainEvents = _identityMaps
            .SelectMany(x => x.Map)
            .SelectMany(x => GetDomainEvents(x.Entity))
            .ToList();

        foreach (var domainEvent in entitiesWithDomainEvents)
        {
            Console.WriteLine($"Publishing Domain Event: {domainEvent.GetType().Name}");
            hadDomainEvent = true;
            await _mediator.Publish(domainEvent);
        }

        if (hadDomainEvent)
        {
            await PublishDomainEvents();
        }
    }

    private IEnumerable<IDomainEvent> GetDomainEvents(IAggregateRoot entity)
    {
        var result = new List<IDomainEvent>();
        if (entity.DomainEvents.Any())
        {
            result.AddRange(entity.DomainEvents);
            entity.ClearDomainEvents();
        }

        var fields = entity.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        fields = Array.FindAll(fields, field => !Attribute.IsDefined(field, typeof(CompilerGeneratedAttribute)));
        var iHasDomainEventObjects = fields.Where(p =>
            typeof(IHasDomainEvents).IsAssignableFrom(p.FieldType));
        var iHasDomainEventObjectsList = fields.Where(p =>
            p.FieldType.IsGenericType &&
            p.FieldType.GetGenericTypeDefinition() == typeof(List<>) &&
            p.FieldType.GetGenericArguments().Length == 1 &&
            typeof(IHasDomainEvents).IsAssignableFrom(p.FieldType.GetGenericArguments()[0]));

        foreach (var objFieldInfo in iHasDomainEventObjects)
        {
            var iHasDomainEventObj = (IHasDomainEvents)objFieldInfo.GetValue(entity)!;
            result.AddRange(iHasDomainEventObj.DomainEvents);
            iHasDomainEventObj.ClearDomainEvents();
        }

        foreach (var objFieldInfo in iHasDomainEventObjectsList)
        {
            var objectsWithDomainEvents = (IEnumerable<IHasDomainEvents>)objFieldInfo.GetValue(entity)!;
            foreach (var obj in objectsWithDomainEvents)
            {
                result.AddRange(obj.DomainEvents);
                obj.ClearDomainEvents();
            }
        }

        return result;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null!;
                }

                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null!;
                }
            }

            _disposed = true;
        }
    }

    ~UnitOfWork()
    {
        Dispose(false);
    }
}