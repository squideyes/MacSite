using System.Data;
using System.Globalization;
using Dapper;

namespace MacSite.Data.Db;

/// <summary>
/// SQLite has no native <c>uuid</c> or <c>timestamptz</c> type, so we store both as TEXT.
/// Dapper can't round-trip <see cref="Guid"/>/<see cref="DateTimeOffset"/> against TEXT columns
/// on its own (it would throw on read), so these handlers do the string conversion explicitly.
/// Registered once in <see cref="DependencyInjection.AddMacSiteData"/>.
/// </summary>
internal sealed class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override Guid Parse(object value) => Guid.Parse((string)value);

    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.DbType = DbType.String;
        parameter.Value = value.ToString();
    }
}

/// <summary>Stores/reads <see cref="DateTimeOffset"/> as a round-trippable ISO-8601 string ("O").</summary>
internal sealed class DateTimeOffsetTypeHandler : SqlMapper.TypeHandler<DateTimeOffset>
{
    public override DateTimeOffset Parse(object value) =>
        DateTimeOffset.Parse((string)value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        parameter.DbType = DbType.String;
        parameter.Value = value.ToString("O", CultureInfo.InvariantCulture);
    }
}
