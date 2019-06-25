using System;

namespace bank.Models
{
  public class Both
  {
    public User user { get; set; }
    public LoginUser login { get; set; }
    public Transaction transactions { get; set; }
  }
}