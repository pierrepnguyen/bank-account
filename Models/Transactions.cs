using System;
using System.ComponentModel.DataAnnotations;

namespace bank.Models
{
  public class Transaction
  {
    [Key]
    public int TransactionId { get; set; }

    [Display(Name="Amount")]
    public float Amount { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
  }
}