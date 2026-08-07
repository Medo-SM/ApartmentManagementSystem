namespace ApartmentManagementSystem.Core.Domain.Entites.Base
{
using System;

public class BaseEntity
{
	public int Id { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.Now;
	public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
}