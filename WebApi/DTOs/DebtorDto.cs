
using WebApi.Models;

namespace WebApi.DTOs;

public class CreateDebtorRequest : UpdateDebtorContactInfoRequest
{
    public required string Name { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public string? ZipCode { get; init; }
}

public class UpdateDebtorContactInfoRequest
{
    public required string Phone { get; init; }
    public required string Email { get; init; }
}

public class UpdateDebtorAddressRequest
{
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public string? ZipCode { get; init; }
}

public class DebtorResponse(Debtor debtor)
{
    public string DebtorId { get; } = debtor.Id;
    public string Name { get; } = debtor.FullName;
    public string Phone { get; } = debtor.Phone;
    public string Email { get; } = debtor.Email;
    public Address Address { get; } = debtor.Address;
    public DateTime CreatedAt { get; } = debtor.CreatedAt;
}